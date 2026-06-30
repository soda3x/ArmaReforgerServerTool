# DevOps Subagent Workflow

**Purpose:** Document how to use the subagent team for coordinated development, testing, and deployment.

**Available agents:** architect, implementer, code-reviewer, qa-test-runner, security-auditor, docs-scribe, devops-pipeline

---

## Workflow Pattern: Feature from Design to Ship

### 1. Architecture & Design (architect)
**When:** Before implementing non-trivial features  
**What:** Propose design, flag risks, return ADR  
**Input:** Requirements, constraints, context  
**Output:** Design decision + guardrails (does NOT write code)

```
Use when: Planning Phase 1B multi-server dashboard
Expect: High-level design, arch trade-offs, security considerations
```

### 2. Implementation (implementer)
**When:** After architect approves design  
**What:** Write code + tests, run them, return summary  
**Input:** Architecture/design, specific files to change  
**Output:** Code changes + test results

```
Use when: Building the multi-server features
Expect: Code written, tests passing, diff summary
```

### 3. Code Review (code-reviewer)
**When:** Right after implementer writes code  
**What:** Deep review for bugs, security, style  
**Input:** Recent diff, codebase context  
**Output:** Severity-ranked findings

```
Use when: implementer returns
Expect: 🔴 bugs, 🟡 improvements, 🟢 good patterns
```

### 4. Security Audit (security-auditor)
**When:** After code review, before merge  
**What:** Scan for secrets, vulns, injection, auth issues  
**Input:** Diff, dependency changes  
**Output:** Risk-ranked security findings

```
Use when: Before opening PR
Expect: Hardcoded secrets, injection vectors, auth gaps, leaked creds
```

### 5. Test Runner (qa-test-runner)
**When:** After code review passes  
**What:** Run test suite, report pass/fail + failures  
**Input:** Test command from package.json or config  
**Output:** Test status + failure details

```
Use when: Implementing new features
Expect: Pass/fail status + stack traces for failures
```

### 6. QA Gates (/qa skill)
**When:** Before merge to main  
**What:** Run mechanical checks → admin gate → reviewer gate → polish gate  
**Input:** Current working tree  
**Output:** PASS/FAIL + blocking issues (if any)

```
Use when: Ready to merge
Options: /qa quick (reviewer only) or /qa full (all gates)
```

### 7. Documentation (docs-scribe)
**When:** After feature lands  
**What:** Update README, CHANGELOG, HANDOFF.md  
**Input:** Changed code, feature description  
**Output:** Documentation updates

```
Use when: Feature is done and in repo
Expect: Docs synced with code changes
```

### 8. DevOps Pipeline (devops-pipeline)
**When:** CI/CD issues or deploy needs  
**What:** Fix .gitlab-ci.yml, debug runner, manage deploys  
**Input:** Pipeline failure, deploy target  
**Output:** Pipeline fixed, deployment complete

```
Use when: CI failing or need to deploy
Expect: Pipeline debugging, deploy orchestration
```

---

## Full Workflow Example: Phase 1B Multi-Server Feature

### Phase: Design
```
1. Spawn architect
   → "Design Phase 1B multi-server dashboard architecture"
   → Get ADR: database schema, API routes, state management
   → Identify risks: scale to 50+ servers, concurrent updates, audit trail
```

### Phase: Implement
```
2. Spawn implementer
   → "Build multi-server dashboard per the architecture ADR"
   → Code is written, tests run
   → Get: diff summary, test results, feature summary
```

### Phase: Review
```
3. Spawn code-reviewer
   → "Review recent diff for correctness, security, style"
   → Get: 🔴 blocking issues, 🟡 improvements, 🟢 good patterns
   
4. Spawn security-auditor
   → "Scan diff for secrets, injection, auth, vulns"
   → Get: Risk-ranked security findings
```

### Phase: Test
```
5. Spawn qa-test-runner
   → "Run test suite and report results"
   → Get: Pass/fail + failure details (if any)
```

### Phase: QA Gate
```
6. Run /qa quick
   → "Run code review gate only (fast)"
   → Get: PASS or blocking issues
   
   OR /qa full
   → "Run all gates: admin, reviewer, polish"
   → Get: comprehensive QA findings
```

### Phase: Merge & Docs
```
7. Create PR, merge to main
   → git add / git commit / git push
   
8. Spawn docs-scribe
   → "Update docs to reflect multi-server feature"
   → Get: README, CHANGELOG, HANDOFF updates
```

### Phase: Deploy
```
9. Spawn devops-pipeline (if needed)
   → "Deploy Phase 1B to staging/production"
   → Get: Deployment complete, rollout status
```

---

## Decision Tree: Which Agent to Use?

```
Is it a design question?
  → architect (before you code)

Do you have code to write?
  → implementer (after architecture approved)

Did implementer finish?
  → code-reviewer (right after)
  → security-auditor (after code-review passes)

Need to run tests?
  → qa-test-runner (after implementation)

Ready to merge?
  → /qa skill (before merge)

Need to update docs?
  → docs-scribe (after merge)

CI/CD broken or need deploy?
  → devops-pipeline (when needed)
```

---

## Key Rules

1. **Sequential, not parallel** — architect → implementer → code-reviewer → security-auditor → tests → QA gates → merge
2. **Each agent owns their domain** — don't ask implementer to review code; don't ask reviewer to write code
3. **Leverage prior findings** — code-reviewer reads code-reviewer findings; security-auditor knows about code issues
4. **Push to GitHub after each phase** — no work left on local machine
5. **/qa before every merge** — gates catch issues before they land on main

---

## Pattern: Parallel Reviews (When Safe)

Some agents can run in parallel:

```
Parallel OK:
- code-reviewer + security-auditor (both read-only)
- qa-test-runner + code-reviewer (independent)

Sequential required:
- implementer → code-reviewer (reviewer needs code first)
- code-reviewer → security-auditor (auditor reads review findings)
- All reviews → /qa gates (gates need all feedback)
```

---

## Session Protocol: Always End with Push

**Every session MUST end with:**
1. Stage all changes: `git add -A`
2. Commit: `git commit -m "..."`
3. Push: `git push origin main`
4. Update HANDOFF.md in repo
5. Save to GitHub before disconnecting

**Why:** Hotspot outages, power loss, hardware failure — all lose work if not pushed.

---

## Handoff Pattern Between Sessions

**At session end:** Update HANDOFF.md with:
- What was accomplished
- Current state of the code
- What's next for the next session
- Any blocked/pending decisions

**At session start:** Load HANDOFF.md, resume from last checkpoint.

---

**Updated:** 2026-06-29  
**Status:** Active workflow for Phase 1B planning
