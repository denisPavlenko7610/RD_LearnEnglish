# RD Learn English project rules

Think in English and respond in Russian. Code, documentation, tooling and commit titles use English.

## Product and documentation

- `docs/GDD.md` owns product, curriculum, learning and feedback rules. Update it before changing those rules.
- `docs/ROADMAP.md` distinguishes implemented features from planned scope. `docs/VALIDATION.md` records actual checks
  and limits; `docs/ASSETS.md` records provenance and licenses.
- Personal English learning for adults. A1 through B2 is the core path; C1 is an optional extension. Russian UI and
  explanations, English learning material. Strings live in localization/content assets.
- Use practical, neutral adult topics: daily life, work, technology, travel and hobbies. Exclude sexual content, LGBT
  relationship/identity themes, political campaigning and religious persuasion from authored lessons. Never use
  disparaging descriptions of people.
- No lives, energy, ads, purchases, compulsory streaks, punishment for absence, mascots or collectible economies. Hints
  are unlimited and voluntary; assisted answers are never independent mastery.
- CEFR labels describe intended content difficulty, never certify proficiency. Do not call a sample corpus a complete B2
  or C1 course. Listening and speaking require separate content and validation.

- One integrated learning flow, no supplementary trainers. Integrate vocabulary, conversation, collocations, phrasal verbs, idioms and occasional proverbs.
- Progress records recognition, independent recall and delayed recall separately. Pictures are optional instructional context, never rewards.

## Architecture

- Inspect sibling HelpReader and UnityTemplates before inventing reusable infrastructure. Link shared Runtime/Editor
  sources using `Tools/Link-UnityTemplates.ps1`; never copy or fork shared services. Do not edit linked sources for
  project-specific needs.
- `Assets/_Project/Runtime`: Core is pure C# without Unity references; Application owns orchestration and ports;
  Infrastructure adapts RD.Save/RD.Localization; UI owns presentation; Bootstrap owns composition and RD.SceneFlow.
  Enforce assembly boundaries.
- Keep RD.DependencyInjection with explicit factories at composition boundaries. No service locator in
  domain/presenters. Dispose services with the persistent Boot owner. Use Unity Awaitable, not Task, in new runtime
  async code; catch exceptions in async void lifecycle/UI callbacks.
- Boot initializes services and saves; App owns presentation. No static mutable runtime state. Preserve correct repeated
  Play behavior without domain reload.
- UI Toolkit only. Author hierarchy in UXML and styling in USS, editable in UI Builder. No runtime tree construction,
  CloneTree, uGUI or TMP. Query and bind authored controls in C#.
- PC keyboard/mouse and Android touch; landscape in either direction, resizable 1280x720 default. Keep long content
  scrollable. Validate software keyboard and safe area on devices before release.
- Stable content IDs, authored alternative answers, meaningful explanations. Normalize case, whitespace and terminal
  punctuation only; never silently accept spelling/grammar mistakes via fuzzy matching.
- Validate complete versioned saves before applying. Save detached snapshots with RD.Save atomic storage. Preserve
  unsupported/damaged files. Failed writes retry persistence only, never replay an answer mutation. No save migration
  before first release.

## Changes and checks

- Read applicable nested AGENTS.md files. Inspect Git status first and preserve pre-existing work, including staged
  changes. Verify existence before writing. Preserve GUIDs/.meta files. Never edit Library, Temp, generated solutions or
  package caches.
- Follow Allman braces, tabs, PascalCase members and _camelCase fields. No new code comments/XML comments; express
  intent through names and documentation.
- Run focused learning, review, persistence/failing-write and content tests. Verify compilation and actual Play in Unity
  7 (7000.x). Check UI import warnings and build scene paths. Use isolated save directories for automated/interactive
  validation.
- Run git diff --check. Record unrun checks honestly. Never claim course completion, native review, device verification
  or Play coverage without evidence.
- Do not commit unless requested. Commit messages: one concise English title only; no body, trailers, signatures,
  Co-Authored-By, Signed-off-by or AI attribution.