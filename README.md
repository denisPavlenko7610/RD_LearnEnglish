# English / Everyday

Personal English learning for adults. Russian interface and explanations; English exercises. One integrated learning
flow with contextual vocabulary, grammar, conversation, phrasal verbs, idioms, occasional proverbs and spaced review.

The current vertical slice contains 22 original pilot topics and 88 exercises across A1–B2 with a C1 preview. It is not
a complete B2/C1 course. No supplementary trainers, rewards, lives, accounts, advertising or runtime network dependency.

- [GDD](docs/GDD.md): product and learning design, research references and intended full-course scope.
- [Roadmap](docs/ROADMAP.md): implementation and remaining work.
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