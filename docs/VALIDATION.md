# Validation record

18 November 2026 · Unity 7000.0.0a16 on Windows. This is a playable pilot, not release approval.

## Executed

- Strict incremental Unity compilation after runtime/editor/test changes: zero errors and zero warnings in the affected
  compilation. The initial shared-source import reported upstream UAL0010/UAL0013 static-lifecycle analyzer warnings;
  shared files were left unchanged. New application-owned static state was removed.
- 18 EditMode tests passed: full 88-card corpus, ambiguous choices, normalization, detached mutations, assisted
  evidence,
  choice versus recall, due-day retention, early-practice scheduling, mistake spacing, schema/ID validation, disk-write
  failure and snapshot-only retry, actual file round-trip, restart, damaged/missing-schema file preservation and rolling
  review limits. Results queried from Unity Pipeline test status.
- One PlayMode integration test passed: real Boot composition → App → topic explanation → incorrect choice → persisted
  feedback → next exercise → hint → assisted typed answer → destroy owners → restart Boot → restored evidence.
  The test verifies only one persistent Boot owner. Scene-load callbacks route saves into unique TestResults folders.
- Tools/Validate-Content.py passed: unique content IDs, accepted answers and one valid choice, concise Russian
  explanations,
  all five level labels, localization keys, UXML structure, project GUID/meta integrity and existing build scene paths.
- Actual Play visual inspection of home, explanation, choice exercise and cafe-topic illustration. Game View was
  explicitly
  switched to 1280×720, 800×600, 1920×1080 and a narrow 600×720 window; Screen.width/height were inspected. Earlier
  resized output captures alone were not treated as evidence of viewport responsiveness.
- Narrow-window tab/title compression was fixed with natural content height and vertical scrolling. Illustration uses
  scale-to-fit so the cafe scene is not cropped into an unreadable strip. Header and explanations wrap.
- Post-fix runtime console reported no current errors/warnings. Screenshots and isolated test saves are under ignored
  TestResults. The application's real progress directory was not written by automated or visual checks.
- git diff --check and git diff --cached --check passed. Unity YAML's expected empty-value whitespace is covered by
  .gitattributes; existing staged scaffold files were preserved. No commit was created.

## Limits and remaining checks

- No standalone player build or physical Android test yet. Safe areas, both landscape rotations, touch keyboard, device
  font size and accessibility still need verification. 16:10, wide-phone and 4:3 coverage is not complete acceptance.
- English material has not received native/pedagogical review. Open-text alternatives are deliberately authored and can
  still reject an equally natural phrasing. C1 topics are short previews; no full A1–B2 or C1 corpus is claimed.
- Listening, speech recordings, pronunciation assessment, extended writing/dialogue and transfer to new contexts remain
  planned. Current recall evidence concerns authored exercises, not unrestricted speaking or certified CEFR proficiency.
- The review schedule is an explicit heuristic. No longitudinal retention or comparative effectiveness trial of this
  application has been run.
- A clean-clone standalone validation and upstream shared-module lifecycle-warning review remain release work.