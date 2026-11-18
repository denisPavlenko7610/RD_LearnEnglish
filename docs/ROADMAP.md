# Implementation and next work

18 November 2026. Product rules are in [GDD](GDD.md).

## Implemented vertical slice

- Russian interface and short explanations, with English learning content. No Ukrainian localization.
- One adult learning flow: read the topic explanation, practice, inspect a specific explanation after every answer,
  continue. The rule and free hint remain available. No supplementary trainers or reward system.
- 22 pilot topics / 88 original exercises: A1 3 topics, A2 5, B1 6, B2 5, C1 3 preview topics. Includes conversational
  clarification, word usage, collocations, phrasal verbs, idioms and proverbs alongside grammar and short reading.
- Choice recognition on first encounter; repeated choice items switch to typed recall. Cloze, short translation and
  reading are typed from the start. Authored alternatives, no fuzzy matching.
- Per-item encounter, independent-success, typed-recall and delayed-recall evidence. Current mistakes and UTC due dates
  survive restart. Future due dates do not advance through early practice. Assisted success never clears a mistake.
- Integrated reviews, mistake spacing and a saved rolling review cap. After all topic items have been recalled, learning
  recommends the following topic; the core path does not automatically enter the optional C1 preview.
- Atomic detached saves with persistence-only retry; schema/ID/evidence validation; invalid files preserved.
- Boot/App scene separation, RD.DependencyInjection factories, RD.SceneFlow, RD.Save and RD.Localization reused from
  UnityTemplates through source junctions. Domain is Unity-free. Shared library files were not edited.
- Authored UI Toolkit UXML/USS, mouse/keyboard and touch controls, responsive compact layout and scrolling.
- One original generated cafe illustration used as instructional context on the cafe topic. No decorative rewards.

## Code map

| Path under Assets/_Project                | Responsibility                                                               |
|-------------------------------------------|------------------------------------------------------------------------------|
| Runtime/Core                              | Content/profile contracts, matching, detached attempts and review policy     |
| Runtime/Application                       | LessonSession, integrated scheduling and repository port                     |
| Runtime/Infrastructure                    | Strict persistence DTO and RD.Save adapter; Russian RD.Localization provider |
| Runtime/UI                                | Everyday/Boot UXML, stylesheet/theme and the bound presentation              |
| Runtime/Bootstrap                         | Persistent composition and App loading                                       |
| Content/Resources, Localization/Resources | Generated pilot JSON and UI strings                                          |
| Art                                       | Original generated cafe scene                                                |
| Editor                                    | Idempotent scene preparation and Play-from-Boot setup                        |
| Tests                                     | Domain/persistence EditMode and actual Boot-to-App PlayMode coverage         |

Tools/Build-Content.py and ContentExpressions.py own original curriculum and strings. Validate-Content.py checks corpus,
UXML structure, project metadata and build scene paths. Link-UnityTemplates.ps1 recreates required source links.

## Next milestones

1. Review existing material with a native English editor. Broaden natural accepted answers and translate all example
   sentences where this materially helps understanding. Pilot some nominally advanced topics are scaffolds, not advanced
   assessments.
2. Expand each target expression into different contexts, connected dialogues and practical response tasks. Add
   sense/skill
   aggregation so several cards about one phrase do not look like several independently learned words.
3. Author and review a complete A1/A2 path, then B1/B2. Add optional placement within the same learning flow.
4. Add licensed local speech, listening comprehension, voluntary speaking and longer writing with transparent rubrics.
   These belong to lessons; do not create separate trainer applications.
5. Tune scheduling against real delayed performance; the initial intervals are explicit heuristics. Add meaningful
   delayed transfer tasks; do not equate deterministic answer matching with free language ability.
6. Physical Android keyboard/safe-area/accessibility checks and Windows standalone build validation. Then the C1
   expansion.