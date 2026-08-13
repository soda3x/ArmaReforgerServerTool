/******************************************************************************
 * File Name:    flags.rs
 * Project:      Longbow / ARST-RUST
 * Description:  Utility for mapping ping-site names to ISO country codes.
 *               Ported from Utils/FlagUtils.cs (image generation dropped —
 *               the frontend renders flags from the ISO code).
 ******************************************************************************/

/// Looks up the ISO 3166-1 alpha-2 country code for a given (case-insensitive)
/// ping site name, e.g. "Frankfurt" -> Some("de").
pub fn ping_site_to_country_code(ping_site: &str) -> Option<&'static str> {
    match ping_site.to_lowercase().as_str() {
        // North America
        "new_york" => Some("us"),
        "washington" => Some("us"),
        "los_angeles" => Some("us"),
        "miami" => Some("us"),
        "chicago" => Some("us"),
        "dallas" => Some("us"),
        "seattle" => Some("us"),
        "atlanta" => Some("us"),
        "montreal" => Some("ca"),
        "toronto" => Some("ca"),

        // Europe
        "frankfurt" => Some("de"),
        "london" => Some("gb"),
        "paris" => Some("fr"),
        "amsterdam" => Some("nl"),
        "stockholm" => Some("se"),
        "warsaw" => Some("pl"),
        "madrid" => Some("es"),

        // Oceania & Asia
        "sydney" => Some("au"),
        "melbourne" => Some("au"),
        "singapore" => Some("sg"),
        "tokyo" => Some("jp"),
        "hong_kong" => Some("hk"),
        "seoul" => Some("kr"),

        // South America & Africa
        "sao_paulo" => Some("br"),
        "johannesburg" => Some("za"),

        _ => None,
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn looks_up_known_sites_case_insensitively() {
        assert_eq!(ping_site_to_country_code("frankfurt"), Some("de"));
        assert_eq!(ping_site_to_country_code("Frankfurt"), Some("de"));
        assert_eq!(ping_site_to_country_code("SYDNEY"), Some("au"));
        assert_eq!(ping_site_to_country_code("sao_paulo"), Some("br"));
    }

    #[test]
    fn returns_none_for_unknown_site() {
        assert_eq!(ping_site_to_country_code("moon_base"), None);
    }
}
