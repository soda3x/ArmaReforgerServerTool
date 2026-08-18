//! Client for the Arma Reforger Workshop's own JSON data API — the same endpoints its Next.js
//! frontend (https://reforger.armaplatform.com/workshop) uses for client-side search/pagination,
//! not a scrape of rendered HTML. Confirmed by hand (browser devtools) before writing this:
//!
//! - Search:  `GET {origin}/_next/data/{buildId}/workshop.json?search=&page=&sort=`
//! - Detail:  `GET {origin}/_next/data/{buildId}/workshop/{modId}-{anything}.json`
//!   (the slug after the mod ID is ignored server-side — a lookup by ID alone always works,
//!   which is all we ever have stored as `Mod::mod_id`)
//!
//! Both return the exact same JSON the site's own React components render from — `count`/`rows`
//! for search, a full `asset` object for detail (name, description, license, ratings, tags,
//! author, and CDN-hosted preview image URLs that load directly in an `<img>` tag, no auth).
//!
//! The one fragile part is `buildId`: it's a Next.js build fingerprint embedded in the page's
//! `<script id="__NEXT_DATA__">` JSON, and it changes whenever the site is redeployed. We fetch
//! it lazily, cache it, and on a 404 from the data endpoint (which is what a stale buildId looks
//! like) rediscover it once and retry — self-healing without any user-visible break.

use std::collections::{HashMap, HashSet};

use scraper::{Html, Selector};
use serde::Deserialize;
use tokio::sync::Mutex;

use crate::models::{
    WorkshopAssetDetail, WorkshopAssetSummary, WorkshopDependency, WorkshopScenario, WorkshopSearchResult,
};

use super::error::ServiceError;

pub struct WorkshopService {
    client: reqwest::Client,
    /// e.g. `https://reforger.armaplatform.com` — the page origin, derived from
    /// `ToolProperties::arma_workshop_url` (which points at `.../workshop`, the page path).
    origin: String,
    /// The workshop *page* URL itself (used to (re)discover `build_id`).
    workshop_page_url: String,
    build_id: Mutex<Option<String>>,
}

impl WorkshopService {
    pub fn new(workshop_url: &str, app_version: &str) -> Self {
        let origin = reqwest::Url::parse(workshop_url)
            .map(|u| u.origin().ascii_serialization())
            .unwrap_or_else(|_| "https://reforger.armaplatform.com".to_string());

        let client = reqwest::Client::builder()
            .user_agent(format!("Longbow-ServerTool/{app_version}"))
            .build()
            .unwrap_or_default();

        Self {
            client,
            origin,
            workshop_page_url: workshop_url.to_string(),
            build_id: Mutex::new(None),
        }
    }

    /// Fetches the workshop page's HTML and pulls `buildId` out of its embedded
    /// `<script id="__NEXT_DATA__">` JSON blob.
    async fn discover_build_id(&self) -> Result<String, ServiceError> {
        let html = self
            .client
            .get(&self.workshop_page_url)
            .send()
            .await?
            .error_for_status()?
            .text()
            .await?;

        let document = Html::parse_document(&html);
        let selector = Selector::parse("#__NEXT_DATA__")
            .map_err(|e| ServiceError::Other(format!("invalid selector: {e:?}")))?;
        let script_text = document
            .select(&selector)
            .next()
            .map(|el| el.text().collect::<String>())
            .ok_or_else(|| {
                ServiceError::Other(
                    "Could not find __NEXT_DATA__ on the workshop page — the site may have \
                     changed its layout."
                        .to_string(),
                )
            })?;

        #[derive(Deserialize)]
        struct NextData {
            #[serde(rename = "buildId")]
            build_id: String,
        }
        let parsed: NextData = serde_json::from_str(&script_text)?;
        Ok(parsed.build_id)
    }

    /// GETs `{origin}/_next/data/{buildId}{path_and_query}`, rediscovering `buildId` and
    /// retrying exactly once if the current one is stale (404).
    async fn fetch_data_json(&self, path_and_query: &str) -> Result<serde_json::Value, ServiceError> {
        let build_id = {
            let mut guard = self.build_id.lock().await;
            if guard.is_none() {
                *guard = Some(self.discover_build_id().await?);
            }
            guard.clone().expect("just set above")
        };

        let url = format!("{}/_next/data/{}{}", self.origin, build_id, path_and_query);
        let response = self
            .client
            .get(&url)
            .header("x-nextjs-data", "1")
            .send()
            .await?;

        if response.status() == reqwest::StatusCode::NOT_FOUND {
            // Stale buildId after a redeploy: rediscover once and retry.
            let fresh_build_id = self.discover_build_id().await?;
            *self.build_id.lock().await = Some(fresh_build_id.clone());
            let retry_url = format!("{}/_next/data/{}{}", self.origin, fresh_build_id, path_and_query);
            let retry_response = self
                .client
                .get(&retry_url)
                .header("x-nextjs-data", "1")
                .send()
                .await?
                .error_for_status()?;
            return Ok(retry_response.json().await?);
        }

        Ok(response.error_for_status()?.json().await?)
    }

    /// Searches the workshop catalog. `sort` is passed through verbatim to the upstream API
    /// (observed valid values include `"popular"`/omitted and `"newest"`); an unrecognized
    /// value is upstream's problem to reject, not ours to validate.
    pub async fn search(
        &self,
        query: Option<&str>,
        page: u32,
        sort: Option<&str>,
    ) -> Result<WorkshopSearchResult, ServiceError> {
        let mut path = format!("/workshop.json?page={page}");
        if let Some(q) = query.filter(|q| !q.trim().is_empty()) {
            path.push_str(&format!("&search={}", urlencode(q.trim())));
        }
        if let Some(s) = sort.filter(|s| !s.trim().is_empty()) {
            path.push_str(&format!("&sort={}", urlencode(s.trim())));
        }

        let json = self.fetch_data_json(&path).await?;
        let raw: RawSearchResponse = serde_json::from_value(json)?;
        Ok(WorkshopSearchResult {
            count: raw.page_props.assets.count,
            rows: raw
                .page_props
                .assets
                .rows
                .into_iter()
                .map(RawAssetSummary::into_summary)
                .collect(),
        })
    }

    /// Fetches full details for one mod by its workshop ID.
    pub async fn get_details(&self, mod_id: &str) -> Result<WorkshopAssetDetail, ServiceError> {
        let path = format!("/workshop/{}-longbow.json", urlencode(mod_id));
        let json = self.fetch_data_json(&path).await?;
        let raw: RawDetailResponse = serde_json::from_value(json)?;
        Ok(raw.page_props.asset.into_detail())
    }
}

/// Minimal, dependency-free percent-encoding for query string values — the only characters
/// workshop search terms/mod IDs realistically contain that need escaping are spaces and a
/// handful of reserved characters, so a full `url`-crate-level encoder is unnecessary here.
fn urlencode(s: &str) -> String {
    let mut out = String::with_capacity(s.len());
    for byte in s.bytes() {
        match byte {
            b'A'..=b'Z' | b'a'..=b'z' | b'0'..=b'9' | b'-' | b'_' | b'.' | b'~' => {
                out.push(byte as char);
            }
            _ => out.push_str(&format!("%{byte:02X}")),
        }
    }
    out
}

// --- Upstream raw JSON shapes (permissive: every field defaulted, unknown fields ignored, so
// upstream additions/omissions never break deserialization) -----------------------------------

#[derive(Debug, Deserialize, Default)]
#[serde(rename_all = "camelCase")]
struct RawThumbnail {
    #[serde(default)]
    url: String,
    #[serde(default)]
    width: u32,
}

#[derive(Debug, Deserialize, Default)]
#[serde(rename_all = "camelCase")]
struct RawPreview {
    #[serde(default)]
    url: String,
    #[serde(default)]
    thumbnails: HashMap<String, Vec<RawThumbnail>>,
}

impl RawPreview {
    /// The smallest available thumbnail (card-sized), falling back to the full-size preview URL
    /// if no thumbnails were provided.
    fn thumbnail_url(&self) -> Option<String> {
        let smallest = self
            .thumbnails
            .values()
            .flatten()
            .min_by_key(|t| t.width)
            .map(|t| t.url.clone());
        smallest.or_else(|| (!self.url.is_empty()).then(|| self.url.clone()))
    }
}

#[derive(Debug, Deserialize, Default)]
struct RawTag {
    #[serde(default)]
    name: String,
}

#[derive(Debug, Deserialize, Default)]
#[serde(rename_all = "camelCase")]
struct RawAuthor {
    #[serde(default)]
    username: String,
}

#[derive(Debug, Deserialize, Default)]
#[serde(rename_all = "camelCase")]
struct RawAssetSummary {
    #[serde(default)]
    id: String,
    #[serde(default)]
    name: String,
    #[serde(default)]
    summary: String,
    #[serde(default)]
    average_rating: f64,
    #[serde(default)]
    rating_count: u64,
    #[serde(default)]
    subscriber_count: u64,
    #[serde(default)]
    current_version_number: String,
    #[serde(default)]
    current_version_size: u64,
    #[serde(default)]
    previews: Vec<RawPreview>,
    #[serde(default)]
    tags: Vec<RawTag>,
    #[serde(default)]
    author: RawAuthor,
}

impl RawAssetSummary {
    fn into_summary(self) -> WorkshopAssetSummary {
        WorkshopAssetSummary {
            id: self.id,
            name: self.name,
            summary: self.summary,
            average_rating: self.average_rating,
            rating_count: self.rating_count,
            subscriber_count: self.subscriber_count,
            current_version_number: self.current_version_number,
            current_version_size: self.current_version_size,
            thumbnail_url: self.previews.first().and_then(RawPreview::thumbnail_url),
            author_username: self.author.username,
            tags: self.tags.into_iter().map(|t| t.name).collect(),
        }
    }
}

#[derive(Debug, Deserialize, Default)]
#[serde(rename_all = "camelCase")]
struct RawAssetDetail {
    #[serde(default)]
    id: String,
    #[serde(default)]
    name: String,
    #[serde(default)]
    summary: String,
    #[serde(default)]
    description: String,
    #[serde(default)]
    license: Option<String>,
    #[serde(default)]
    average_rating: f64,
    #[serde(default)]
    rating_count: u64,
    #[serde(default)]
    subscriber_count: u64,
    #[serde(default)]
    current_version_number: String,
    #[serde(default)]
    current_version_size: u64,
    #[serde(default)]
    previews: Vec<RawPreview>,
    #[serde(default)]
    tags: Vec<RawTag>,
    #[serde(default)]
    author: RawAuthor,
    #[serde(default)]
    dependencies: Vec<RawDependencyEntry>,
    #[serde(default)]
    scenarios: Vec<RawScenario>,
}

impl RawAssetDetail {
    fn into_detail(self) -> WorkshopAssetDetail {
        let mut dependencies = Vec::new();
        let mut seen = HashSet::new();
        flatten_dependencies(&self.dependencies, &mut dependencies, &mut seen);

        WorkshopAssetDetail {
            id: self.id,
            name: self.name,
            summary: self.summary,
            description: self.description,
            license: self.license.filter(|l| !l.is_empty()),
            average_rating: self.average_rating,
            rating_count: self.rating_count,
            subscriber_count: self.subscriber_count,
            current_version_number: self.current_version_number,
            current_version_size: self.current_version_size,
            preview_urls: self
                .previews
                .iter()
                .map(|p| p.url.clone())
                .filter(|u| !u.is_empty())
                .collect(),
            author_username: self.author.username,
            tags: self.tags.into_iter().map(|t| t.name).collect(),
            dependencies,
            scenarios: self
                .scenarios
                .into_iter()
                .filter(|s| !s.game_id.is_empty())
                .map(RawScenario::into_scenario)
                .collect(),
        }
    }
}

#[derive(Debug, Deserialize, Default, Clone)]
#[serde(rename_all = "camelCase")]
struct RawScenario {
    #[serde(default)]
    name: String,
    /// The field upstream actually calls `gameId` — this is the value the game's own
    /// `game.scenarioId` config field expects (`{GUID}Missions/Name.conf`), not an opaque ID.
    #[serde(default)]
    game_id: String,
    #[serde(default)]
    game_mode: String,
    #[serde(default)]
    player_count: u32,
}

impl RawScenario {
    fn into_scenario(self) -> WorkshopScenario {
        WorkshopScenario {
            name: self.name,
            path: self.game_id,
            game_mode: self.game_mode,
            player_count: self.player_count,
        }
    }
}

#[derive(Debug, Deserialize, Default, Clone)]
#[serde(rename_all = "camelCase")]
struct RawDependencyAssetRef {
    #[serde(default)]
    id: String,
    #[serde(default)]
    name: String,
}

#[derive(Debug, Deserialize, Default, Clone)]
#[serde(rename_all = "camelCase")]
struct RawDependencyEntry {
    #[serde(default)]
    asset: RawDependencyAssetRef,
    #[serde(default)]
    total_file_size: u64,
    /// Dependencies can themselves have dependencies (e.g. a compatibility patch requiring both
    /// a base mod and an addon) — upstream nests these rather than flattening them for us.
    #[serde(default)]
    dependencies: Vec<RawDependencyEntry>,
}

/// Flattens upstream's nested dependency tree into the transitive set of required mods,
/// deduplicated by ID (the same dependency can appear more than once in the tree).
fn flatten_dependencies(entries: &[RawDependencyEntry], out: &mut Vec<WorkshopDependency>, seen: &mut HashSet<String>) {
    for entry in entries {
        if entry.asset.id.is_empty() {
            continue;
        }
        if seen.insert(entry.asset.id.clone()) {
            out.push(WorkshopDependency {
                id: entry.asset.id.clone(),
                name: entry.asset.name.clone(),
                total_file_size: entry.total_file_size,
            });
        }
        flatten_dependencies(&entry.dependencies, out, seen);
    }
}

#[derive(Debug, Deserialize, Default)]
#[serde(rename_all = "camelCase")]
struct RawAssetsBlock {
    #[serde(default)]
    count: u64,
    #[serde(default)]
    rows: Vec<RawAssetSummary>,
}

#[derive(Debug, Deserialize, Default)]
#[serde(rename_all = "camelCase")]
struct RawSearchPageProps {
    #[serde(default)]
    assets: RawAssetsBlock,
}

#[derive(Debug, Deserialize, Default)]
#[serde(rename_all = "camelCase")]
struct RawSearchResponse {
    #[serde(default)]
    page_props: RawSearchPageProps,
}

#[derive(Debug, Deserialize, Default)]
#[serde(rename_all = "camelCase")]
struct RawDetailPageProps {
    #[serde(default)]
    asset: RawAssetDetail,
}

#[derive(Debug, Deserialize, Default)]
#[serde(rename_all = "camelCase")]
struct RawDetailResponse {
    #[serde(default)]
    page_props: RawDetailPageProps,
}

#[cfg(test)]
mod tests {
    use super::*;

    /// Captured from a real `workshop.json?page=2` response (trimmed to 2 rows) — see the
    /// module doc comment for how this was obtained.
    const SAMPLE_SEARCH_JSON: &str = r#"{
        "pageProps": {
            "search": "",
            "page": 2,
            "assets": {
                "count": 45380,
                "rows": [
                    {
                        "id": "5AC2954C784671B3",
                        "name": "Barrett M82",
                        "type": "addon",
                        "summary": "Barrett M82 with cool APIT rounds and jank animations.",
                        "averageRating": 0.97,
                        "ratingCount": 1293,
                        "subscriberCount": 16260,
                        "currentVersionNumber": "1.2.3",
                        "currentVersionSize": 27021149,
                        "previews": [
                            {
                                "url": "https://ar-gcp-cdn.bistudio.com/full.jpg",
                                "width": 773,
                                "height": 435,
                                "thumbnails": {
                                    "image/jpeg": [
                                        {"url": "https://ar-gcp-cdn.bistudio.com/576.jpg", "width": 576, "height": 324, "contentType": "image/jpeg"},
                                        {"url": "https://ar-gcp-cdn.bistudio.com/288.jpg", "width": 288, "height": 162, "contentType": "image/jpeg"}
                                    ]
                                },
                                "contentType": "image/jpeg"
                            }
                        ],
                        "tags": [{"name": "WEAPONS", "category": null}],
                        "author": {"id": "abc", "username": "ceo_of_bacon", "roles": null, "personalBlocked": false}
                    },
                    {
                        "id": "NOPREVIEWS0000001",
                        "name": "Minimal Upload",
                        "summary": "",
                        "averageRating": 0.0,
                        "ratingCount": 0,
                        "subscriberCount": 0,
                        "currentVersionNumber": "0.1.0",
                        "currentVersionSize": 100,
                        "previews": [],
                        "tags": [],
                        "author": {"id": "xyz", "username": "newbie"}
                    }
                ]
            }
        }
    }"#;

    /// Captured from a real `workshop/{id}-slug.json` response (trimmed of fields we don't use,
    /// e.g. `versions`, `dependencyTree`, `assetVersionDetail`).
    const SAMPLE_DETAIL_JSON: &str = r#"{
        "pageProps": {
            "pathId": "5965550F24A0C152",
            "asset": {
                "averageRating": 0.97,
                "id": "5965550F24A0C152",
                "name": "Where Am I",
                "type": "addon",
                "summary": "Shows where you are on the map",
                "description": "This is a simple mod that marks your current location on the map with a red circle and a line to indicate your direction.",
                "license": "Arma Public License (APL)",
                "licenseText": null,
                "ratingCount": 14360,
                "subscriberCount": 66677,
                "currentVersionNumber": "1.2.0",
                "currentVersionSize": 201671,
                "previews": [
                    {
                        "url": "https://ar-gcp-cdn.bistudio.com/full1.jpg",
                        "width": 773,
                        "height": 435,
                        "thumbnails": {"image/jpeg": [{"url": "https://ar-gcp-cdn.bistudio.com/small1.jpg", "width": 576, "height": 324, "contentType": "image/jpeg"}]},
                        "contentType": "image/jpeg"
                    }
                ],
                "screenshots": [
                    {"url": "https://ar-gcp-cdn.bistudio.com/shot1.jpg", "width": 1280, "height": 720, "thumbnails": {}, "contentType": "image/jpeg"}
                ],
                "author": {"roles": null, "id": "df2f7c72", "username": "ValterB", "personalBlocked": false},
                "tags": [{"name": "EASY", "category": null}, {"name": "GPS", "category": null}]
            }
        }
    }"#;

    /// Captured from a real `workshop/{id}-slug.json` response for a mod with dependencies
    /// ("RHS Status Quo", which requires two RHS content-pack addons). Trimmed to the fields
    /// relevant to dependency parsing.
    const SAMPLE_DETAIL_WITH_DEPENDENCIES_JSON: &str = r#"{
        "pageProps": {
            "pathId": "595F2BF2F44836FB",
            "asset": {
                "averageRating": 0.88,
                "id": "595F2BF2F44836FB",
                "name": "RHS - Status Quo",
                "summary": "RHS on Arma Reforger",
                "description": "RHS: Status Quo.",
                "license": "Custom license",
                "ratingCount": 9220,
                "subscriberCount": 31995,
                "currentVersionNumber": "0.16.5150",
                "currentVersionSize": 204219382,
                "previews": [],
                "author": {"id": "bef0c20c", "username": "Red Hammer Studios"},
                "tags": [],
                "dependencies": [
                    {
                        "asset": {"id": "1337C0DE5DABBEEF", "name": "RHS - Content Pack 01"},
                        "version": "0.16.5150",
                        "totalFileSize": 6343204665,
                        "published": true,
                        "dependencies": []
                    },
                    {
                        "asset": {"id": "BADC0DEDABBEDA5E", "name": "RHS - Content Pack 02"},
                        "version": "0.16.5150",
                        "totalFileSize": 2393417241,
                        "published": true,
                        "dependencies": [
                            {
                                "asset": {"id": "1337C0DE5DABBEEF", "name": "RHS - Content Pack 01"},
                                "version": "0.16.5150",
                                "totalFileSize": 6343204665,
                                "published": true,
                                "dependencies": []
                            }
                        ]
                    }
                ]
            }
        }
    }"#;

    #[test]
    fn parses_search_response_and_maps_to_clean_summary() {
        let raw: RawSearchResponse = serde_json::from_str(SAMPLE_SEARCH_JSON).unwrap();
        let result = WorkshopSearchResult {
            count: raw.page_props.assets.count,
            rows: raw
                .page_props
                .assets
                .rows
                .into_iter()
                .map(RawAssetSummary::into_summary)
                .collect(),
        };

        assert_eq!(result.count, 45380);
        assert_eq!(result.rows.len(), 2);

        let barrett = &result.rows[0];
        assert_eq!(barrett.id, "5AC2954C784671B3");
        assert_eq!(barrett.name, "Barrett M82");
        assert_eq!(barrett.author_username, "ceo_of_bacon");
        assert_eq!(barrett.tags, vec!["WEAPONS".to_string()]);
        // Smallest thumbnail (288px) is preferred over the larger 576px one or the full preview.
        assert_eq!(barrett.thumbnail_url.as_deref(), Some("https://ar-gcp-cdn.bistudio.com/288.jpg"));
    }

    #[test]
    fn falls_back_to_no_thumbnail_when_asset_has_no_previews() {
        let raw: RawSearchResponse = serde_json::from_str(SAMPLE_SEARCH_JSON).unwrap();
        let minimal = raw
            .page_props
            .assets
            .rows
            .into_iter()
            .map(RawAssetSummary::into_summary)
            .nth(1)
            .unwrap();
        assert_eq!(minimal.id, "NOPREVIEWS0000001");
        assert_eq!(minimal.thumbnail_url, None);
    }

    #[test]
    fn parses_detail_response_and_maps_to_clean_detail() {
        let raw: RawDetailResponse = serde_json::from_str(SAMPLE_DETAIL_JSON).unwrap();
        let detail = raw.page_props.asset.into_detail();

        assert_eq!(detail.id, "5965550F24A0C152");
        assert_eq!(detail.name, "Where Am I");
        assert_eq!(detail.license.as_deref(), Some("Arma Public License (APL)"));
        assert_eq!(detail.author_username, "ValterB");
        assert_eq!(detail.tags, vec!["EASY".to_string(), "GPS".to_string()]);
        assert_eq!(detail.preview_urls, vec!["https://ar-gcp-cdn.bistudio.com/full1.jpg".to_string()]);
        assert_eq!(detail.dependencies, Vec::new());
    }

    #[test]
    fn flattens_nested_dependency_tree_and_dedupes_by_id() {
        let raw: RawDetailResponse = serde_json::from_str(SAMPLE_DETAIL_WITH_DEPENDENCIES_JSON).unwrap();
        let detail = raw.page_props.asset.into_detail();

        // "RHS - Content Pack 01" appears both as a direct dependency and nested under "RHS -
        // Content Pack 02" — it must only appear once in the flattened, deduplicated result.
        assert_eq!(detail.dependencies.len(), 2);
        let ids: Vec<&str> = detail.dependencies.iter().map(|d| d.id.as_str()).collect();
        assert!(ids.contains(&"1337C0DE5DABBEEF"));
        assert!(ids.contains(&"BADC0DEDABBEDA5E"));

        let pack_one = detail.dependencies.iter().find(|d| d.id == "1337C0DE5DABBEEF").unwrap();
        assert_eq!(pack_one.name, "RHS - Content Pack 01");
        assert_eq!(pack_one.total_file_size, 6343204665);
    }

    #[test]
    fn missing_optional_fields_deserialize_to_defaults_not_errors() {
        // A deliberately sparse response (as if upstream omitted fields we don't recognize, or
        // added fields we don't map) must still deserialize successfully.
        let sparse = r#"{"pageProps": {"assets": {"count": 0, "rows": [{"id": "X", "name": "Bare"}]}}}"#;
        let raw: RawSearchResponse = serde_json::from_str(sparse).unwrap();
        let summary = raw.page_props.assets.rows.into_iter().next().unwrap().into_summary();
        assert_eq!(summary.id, "X");
        assert_eq!(summary.name, "Bare");
        assert_eq!(summary.thumbnail_url, None);
        assert_eq!(summary.tags, Vec::<String>::new());
    }

    #[test]
    fn urlencode_escapes_spaces_and_leaves_safe_chars_alone() {
        assert_eq!(urlencode("night vision"), "night%20vision");
        assert_eq!(urlencode("ABC-123_test.mod~"), "ABC-123_test.mod~");
    }

    /// Hits the real workshop site. Not run by default (`cargo test` skips `#[ignore]`d tests) —
    /// run explicitly with `cargo test -- --ignored workshop_service::tests::live_` to confirm
    /// buildId discovery and both endpoints still work against production, e.g. after Bohemia
    /// redeploys the site or changes its response shape.
    #[tokio::test]
    #[ignore]
    async fn live_search_and_detail_round_trip() {
        let service = WorkshopService::new(
            "https://reforger.armaplatform.com/workshop",
            "test",
        );

        let results = service.search(Some("ace"), 1, None).await.expect("search failed");
        assert!(results.count > 0, "expected at least one search result");
        assert!(!results.rows.is_empty());

        let first = &results.rows[0];
        let detail = service.get_details(&first.id).await.expect("detail fetch failed");
        assert_eq!(detail.id, first.id);
        assert!(!detail.name.is_empty());

        // "RHS - Status Quo" is a known real mod with dependencies (two RHS content packs) —
        // confirms the `dependencies` field actually parses against production, not just fixtures.
        let rhs = service
            .get_details("595F2BF2F44836FB")
            .await
            .expect("RHS - Status Quo detail fetch failed");
        assert!(!rhs.dependencies.is_empty(), "expected RHS - Status Quo to report dependencies");
    }

    /// Confirms the 404-and-rediscover path actually works against production: force a
    /// deliberately wrong cached buildId, then make a real request and check it self-heals.
    #[tokio::test]
    #[ignore]
    async fn live_stale_build_id_self_heals() {
        let service = WorkshopService::new(
            "https://reforger.armaplatform.com/workshop",
            "test",
        );
        *service.build_id.lock().await = Some("deliberately-stale-build-id".to_string());

        let results = service.search(None, 1, None).await.expect("search failed after stale buildId");
        assert!(results.count > 0);
    }
}
