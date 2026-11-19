# Validation record

19 November 2026 · Updated the bus announcement for online-first playback. Unity 7000.0.0a16 compiled with zero
errors and warnings. All 25 EditMode and 6 PlayMode tests passed with isolated saves. The bus test checked the
online playback label, retry availability and transcript access after a simulated provider failure. Forced UXML
reimport emitted no warnings. Content validation passed for 72 topics and 344 exercises. A separate request for the
authored driver message returned HTTP 200 and an MP3 response. The PlayMode suite used a substitute speech provider.
Built the updated ARM64 development APK at `TestResults/Android/EnglishEveryday-Pilot-Online.apk`. Verified its
signature, Internet permission and embedded curriculum and UI strings. ADB listed no phone. Device playback,
microphone use and installation are still pending.

19 November 2026 · Added eight A1–A2 vocabulary lessons and the bus announcement and route-transfer exercises.
Content validation passed for 72 topics, 344 exercises and 121 practice vocabulary entries. Unity 7000.0.0a16
compiled the update and passed 25 EditMode and 6 PlayMode tests with isolated saves. The bus test completed
all eight cards, checked the spoken model request and transcript fallback, and verified the saved written
answers. The vocabulary tests checked automatic mistake spacing, the alternate sentence on review, and saved
recall after restart. No UXML or USS import warnings were found in the checked editor log. The tests used a
substitute speech provider. Actual microphone capture, playback and the latest content on Android remain
untested, and the previous development APK was not rebuilt for this content batch.

19 November 2026 · Added a short written reply and alternate review context to the A1 bus scenario. The content audit
passed for 64 topics and 302 exercises. A comparison against the staged curriculum verified that all existing topics
and exercises kept their content and IDs. Unity 7000.0.0a16 compiled without errors or warnings in the live Editor,
and all 24 EditMode and 6 PlayMode tests passed. The bus PlayMode test accepted the full form of the reply and
verified independent recall under its new exercise ID in an isolated save. Replaced an unsupported selector in
`Everyday.uss` and verified that forced reimport emitted no new warnings.

Unity 7000.0.0a16 built the Android development APK from an isolated project copy. Android build tools verified the
APK signature, `com.rd.learnenglish.pilot` application ID, ARM64 architecture and minimum API 26. The packaged
curriculum matched the workspace JSON, including the new exercise. The clean Android build emitted upstream Unity
package warnings, including Burst direct-call warnings. ADB listed no phone, so installation, device launch and
interaction are pending.

19 November 2026 · The A1 bus vocabulary lesson added six practice entries with alternate review contexts. The
content audit passed for 64 topics, 301 exercises and 81 vocabulary practice entries. Unity 7000.0.0a16 passed
24 EditMode and 6 PlayMode tests. The vocabulary UI test verified that the pronunciation action sends the
selected term and reports provider failure while practice and save/restart remain available. A separate request
to the HelpReader web voice for `bus stop` returned HTTP 200 and a decodable MP3. This confirms the source
response and the UI integration separately. An isolated Unity Android build produced the signed ARM64 development
APK at `TestResults/Android/EnglishEveryday-Pilot.apk`. APK inspection confirmed minimum API 26, target API 36,
the landscape orientation setting and Internet permission. ADB found no connected phone. The available Pixel 9a
emulator configuration has no installed system image, so Android installation and playback remain untested.

19 November 2026 · Dark-first redesign with five switchable themes, validated in the live Unity 7000.0.0a16 Editor.
Strict compilation returned zero errors and warnings. All 24 EditMode tests passed, including five new settings
tests (theme validation, retry of a failed write without losing the choice, file round trip, and an unreadable
settings file that falls back to Midnight and is never overwritten). All 6 PlayMode tests passed, including a new
one that opens Appearance, switches to Daylight, checks the saved `settings-v1.json` and restores the theme after a
Boot restart. PlayMode ran with the automatic Boot start scene temporarily disabled and restored afterwards. Content
validation passed. Actual Play captures at 1280×720 covered home in all five themes, the Appearance screen, lesson
intro, typed recall, correct and incorrect feedback, and marked choice options. A real 600×720 Game View covered the
stacked home and Appearance layouts. Runs used isolated save folders under TestResults. A root-height defect that
exposed the default background below short screens was found and fixed during these captures. Light-theme lesson
captures were checked at 1280×720 only; the narrow layout was not captured in every theme. Physical Android devices,
touch input, safe areas, a contrast audit with measurement tools and review by users with low vision remain pending.

19 November 2026 · Vocabulary import produced 8,693 A1–C1 reference entries, including 7,703 with Russian glosses.
The content audit passed for 63 topics, 295 exercises and 75 vocabulary practice entries linked to alternate contexts.
Unity 7000.0.0a16 compiled the update in an isolated project copy. All 19 EditMode and 5 PlayMode tests passed.
The vocabulary PlayMode test searched the reference catalog, filtered C1 phrasal verbs, found an authored entry by
its Russian gloss, submitted a mistake, found it in due review, answered correctly, and verified the saved recall
status after restarting Boot. No UXML or USS import warnings were found in the run log. The PlayMode run used
isolated profiles and the temporary copy's test configuration. Visual layout on physical devices and editorial
review of the imported glosses remain pending.

19 November 2026 · The 20 scenario lessons expanded the catalog to 48 topics and 220 exercises. The content audit
passed, including localization, UXML, metadata and build scene paths. Compared the generated catalog with the prior
28 topics and verified that their content and IDs were identical. Unity 7000.0.0a16 compiled the scenario update in an
isolated project copy. All 19 EditMode and 4 PlayMode tests passed. The PlayMode run used isolated profiles, disabled
the editor's automatic Boot scene and enabled background playback in the temporary copy. The bus scenario test
completed all five activities, verified that three typed answers counted as recall and checked that the first choice
returned as typed practice. No UXML or USS import warnings were found in the run log. This check covered the
course update before the concurrent appearance settings work. Visual layout, Android and learning outcomes over
multiple study sessions were not evaluated for the new scenarios.

19 November 2026 · Tested the illustrated home screen and topic preview in Unity 7000.0.0a16. PlayMode tests cover
category accents, opening the rule and resetting it on topic change, and the meter value from saved typed recall.
Visually checked the illustrated dashboard at 1280×720 and an open rule preview at an actual 600×720 Game View size.

19 November 2026 · The vocabulary update passed all 19 EditMode and 3 PlayMode tests on Unity 7000.0.0a16.
The added PlayMode case verifies the vocabulary filters, token ordering, undo, the submission gate and saved guided
success without typed recall. Inspected Play Mode at 1280×720. Content validation passed for 28 topics and 120
exercises,
including six sentence assemblies and nineteen alternate review contexts. Compilation produced no errors. The strict
run reported static-lifecycle warnings in the linked RD.Save sources from UnityTemplates.

19 November 2026 · Validated the redesigned dashboard and written dialogues in Unity 7000.0.0a16. All 19 EditMode
tests and both PlayMode tests passed. The additional PlayMode test checks that the dialogue is visible before an answer,
the Russian explanation appears after submission, and the English answer is saved. Inspected 1280×720 dashboard and
dialogue captures and the feedback at an actual 600×720 Game View size. The content audit passed for 96 exercises.

19 November 2026 · Added 13 alternate review situations. Strict Unity compilation returned zero errors and warnings.
All 19 EditMode tests and the Boot-to-App PlayMode test passed. The new test checks original presentation, due typed
recall in a new situation, evidence under the original exercise ID and the next scheduled return to the original
context. After the passing PlayMode run, the current Editor console reported zero errors and warnings.

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
- The bus activity pilots an optional spoken model, local recording, transcript fallback and a new route task.
  Validated listening and pronunciation assessments, extended writing/dialogue and broader transfer across the
  curriculum remain planned. Current recall evidence concerns authored exercises.
- The review schedule is an explicit heuristic. No longitudinal retention or comparative effectiveness trial of this
  application has been run.
- A clean-clone standalone validation and upstream shared-module lifecycle-warning review remain release work.
