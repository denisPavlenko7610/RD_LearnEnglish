# Implementation and next work

19 November 2026. Product rules are in [GDD](GDD.md). The [course delivery plan](COURSE_PLAN.md) defines the full
A1–C1 Android release and the checks for each stage.

## Implemented vertical slice

- Russian interface and short explanations, with English learning content. No Ukrainian localization.
- One adult learning flow: read the topic explanation, practice, inspect a specific explanation after every answer,
  continue. The rule and free hint remain available. No supplementary trainers or reward system.
- 72 pilot topics / 344 original exercises: A1 18 topics, A2 20, B1 15, B2 13, C1 6 preview topics. Includes
  conversational clarification, vocabulary, collocations, phrasal verbs, idioms, grammar and short reading.
- Choice recognition on first encounter; repeated choice items switch to typed recall. Cloze, short translation and
  reading are typed from the start. Authored alternatives, no fuzzy matching.
- 161 exercises have a second situation with a hint and explanation. Due review alternates with the original
  context under the same exercise ID. An eligible vocabulary mistake returns in its alternate context.
- Twenty-eight short written dialogues sit inside A1–B2 lessons. The dashboard marks topics with a dialogue.
  Twenty scenario lessons combine a contextual choice, sentence assembly, a typed gap, a dialogue reply and reading.
  Situations include public transport, shopping, booking changes, troubleshooting and workplace negotiation.
- The A1 bus scenario includes a spoken model with optional local recording and playback, a driver message with
  online playback and a transcript, a short written reply, and a transfer question with a new route map. Two short
  vocabulary
  topics follow it in the A1 route.
- The offline vocabulary catalog has 8,693 reference entries and 121 authored practice entries across A1–C1, with
  English/Russian search and filters for level, kind, and due review. The authored entries link to 24 short lessons
  covering words, expressions and phrasal verbs. The first and later contexts share saved recall and mistake evidence.
- Development builds can pronounce a catalog entry or vocabulary answer on demand using the same English web voice
  as HelpReader. Each request sends only the selected term. Text practice works without the service.
- The topic chooser also surfaces word and phrasal-verb lessons through dedicated filters. Assembly offers undo and
  changes to typed practice after the first attempt.
- The home screen uses original fox and owl illustrations. Topic cards identify the level and category, show real
  typed recall progress, and offer a collapsible rule preview that resets when the topic changes.
- Per-item encounter, independent-success, typed-recall and delayed-recall evidence. Current mistakes and UTC due dates
  survive restart. Future due dates do not advance through early practice. Assisted success never clears a mistake.
- Integrated reviews, mistake spacing and a saved rolling review cap. After all topic items have been recalled, learning
  recommends the following topic. The current pilot keeps its C1 preview manually selectable.
- Atomic detached saves with persistence-only retry; schema/ID/evidence validation; invalid files preserved.
- Boot/App scene separation, RD.DependencyInjection factories, RD.SceneFlow, RD.Save and RD.Localization reused from
  UnityTemplates through source junctions. Domain is Unity-free. Shared library files were not edited.
- Authored UI Toolkit UXML/USS, mouse/keyboard and touch controls, responsive compact layout and scrolling.
- A development APK builds for ARM64 Android. The package signature and embedded curriculum have been checked.
- Dark-first visual system built on USS variables, with five switchable themes (Midnight default, Ocean, Forest, Ember,
  Daylight) chosen in an Appearance screen and saved in a separate versioned settings slot. The dashboard shows
  encountered, recalled and delayed-recall counts as separate tiles, and answered choices are marked.
- One original generated cafe illustration used as instructional context on the cafe topic. No decorative rewards.

## Code map

| Path under Assets/_Project                | Responsibility                                                              |
|-------------------------------------------|-----------------------------------------------------------------------------|
| Runtime/Core                              | Content/profile contracts, matching, detached attempts and review policy    |
| Runtime/Application                       | LessonSession, SettingsSession, integrated scheduling and repository ports  |
| Runtime/Infrastructure                    | Profile and settings repositories on RD.Save; Russian localization provider |
| Runtime/UI                                | Everyday/Boot UXML, stylesheet/theme and the bound presentation             |
| Runtime/Bootstrap                         | Persistent composition and App loading                                      |
| Content/Resources, Localization/Resources | Generated pilot JSON and UI strings                                         |
| Art                                       | Original generated cafe scene                                               |
| Editor                                    | Idempotent scene preparation and Play-from-Boot setup                       |
| Tests                                     | Domain/persistence EditMode and actual Boot-to-App PlayMode coverage        |

Tools/Build-Content.py and ContentExpressions.py own original curriculum and strings. Validate-Content.py checks corpus,
UXML structure, project metadata and build scene paths. Link-UnityTemplates.ps1 recreates required source links.

## Next milestones

1. Run the bus reference unit through both landscape orientations on a physical Android phone. Verify playback,
   microphone permission, interruption, text fallback, writing and delayed review.
2. Complete and review A1–A2, including placement, vocabulary-sense evidence and independent checkpoints.
3. Complete B1–B2 with extended listening and reading, connected writing, spoken interaction and mediation.
4. Complete C1 and the independent capstone, validate on target Android devices, and package the release.