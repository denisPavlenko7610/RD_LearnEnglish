# English / Everyday

Personal English learning for adults. Russian interface and explanations; English exercises. One integrated learning
flow with contextual vocabulary, grammar, conversation, phrasal verbs, idioms, occasional proverbs and spaced review.

The current pilot contains 72 topics and 344 exercises across A1–B2 with a C1 preview. Twenty scenario lessons move
from contextual choices and sentence assembly to typed recall, short written replies and reading. Scheduled review
reuses selected expressions in a new situation.

The offline A1–C1 vocabulary catalog contains 8,693 reference entries and 121 entries with contextual practice
and saved recall status. The catalog searches English headwords and Russian glosses and surfaces entries due for review.

- [GDD](docs/GDD.md): product and learning design, research references and intended full-course scope.
- [Roadmap](docs/ROADMAP.md): implementation and remaining work.
- [Course delivery plan](docs/COURSE_PLAN.md): A1–C1 curriculum, Android milestones and release checks.
- [Validation](docs/VALIDATION.md): executed checks and known limits.
- [Assets](docs/ASSETS.md): source provenance and generated illustration.

## Open and run

Requires Unity 7000.0.0a16 and a sibling UnityTemplates checkout. Before opening a fresh clone:

```powershell
powershell -ExecutionPolicy Bypass -File Tools/Link-UnityTemplates.ps1
```

Open the project, then use **Learn English > Prepare Boot and App** if intentionally recreating scene setup. Save your
current scene first. Play starts from `Assets/_Project/Scenes/Boot.unity`, which composes services and loads App.
UI hierarchies and styles are editable in UI Builder under `Assets/_Project/Runtime/UI`.

Build the development APK with the Unity Android module, SDK, NDK and JDK installed. Run the batch build against
a project that is closed in the Unity Editor. An isolated project copy can be used to keep the working Editor open.

```powershell
unity build . --target Android --android-export-type apk --execute-method LearnEnglish.Editor.EnglishProjectSetup.BuildAndroidPilot --output TestResults/Android/EnglishEveryday-Pilot.apk
```

```powershell
python Tools/Build-Content.py
python Tools/Validate-Content.py
```

Shared Runtime/Editor folders are junctions recreated by the link script, excluded from Git. Do not edit them for
learning
policy. Preserve their upstream licenses. The application owns its curriculum, save schema and all learning policy.

Local progress uses RD.Save in `Application.persistentDataPath/EnglishEveryday/adult-english-v1.json`, independently
from
HelpReader. Invalid or unsupported saves are refused and preserved. Automated checks use isolated TestResults folders.

Before running PlayMode tests, set `EditorSceneManager.playModeStartScene = null`; restore Boot afterwards. Tests refuse
to
continue if an application profile is already open. Never run them inside a live user session.
