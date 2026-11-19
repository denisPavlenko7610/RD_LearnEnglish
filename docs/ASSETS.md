# Assets and provenance

- HelpReader: sibling project used as an architectural reference. No child curriculum, characters, audio or artwork
  copied.
- UnityTemplates: sibling shared library linked in place, not vendored. Retain its original notices and licenses. See
  Tools/Link-UnityTemplates.ps1 for source units.
- Curriculum, UI layout and styles: original project material authored for this prototype. Not copied from Duolingo.
- CEFR: Council of Europe framework used as a design reference, not redistributed course content or
  certification. https://www.coe.int/en/web/common-european-framework-reference-languages/cefr-descriptors
- CafeConversation.png: generated with the built-in imagegen tool on 18 November 2026 for this project, without a
  reference image. Used as cafe-topic context. No third-party branding or artwork depicted.
- UI uses the Unity runtime font.
- `EveningFox.png` and `LibraryOwl.png` are original generated editorial illustrations for the home banner and topic
  preview. Created with the built-in imagegen tool on 19 November 2026, without reference images or branding.

## Vocabulary reference data

`Assets/_Project/Content/Resources/VocabularyReference.json` is an adapted offline index. The importer retains
single headwords and parts of speech at A1–C1, normalizes spelling variants to the first listed form, keeps the
earliest level for duplicate headword and part-of-speech pairs, joins available NGSL rankings, and extracts up to
three Russian glosses matched by part of speech. Common function words have project-authored explanations.
Source levels describe entries and can differ across senses. Source glosses are reference material.
The practice cards in `Curriculum.json` contain original authored prompts and explanations.

Download the following files into one directory under the indicated names and pass that directory to
`python Tools/Import-Vocabulary.py`. The inputs used for the current asset were retrieved on 19 November 2026.

| Input          | Source and terms                                                                                                                                                                                                                                                                                                                                             | SHA-256                                                            |
|----------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------|
| `cefrj.csv`    | [CEFR-J Wordlist 1.5](https://github.com/openlanguageprofiles/olp-en-cefrj/blob/master/cefrj-vocabulary-profile-1.5.csv), compiled by Yukio Tono, Tono Laboratory at Tokyo University of Foreign Studies. Reuse with attribution.                                                                                                                            | `b0dd3c635f1c9a4fdf1490c7e5b7c48e8bbe55b652ad0c9860a95f98e10ae498` |
| `octanove.csv` | [Octanove Vocabulary Profile C1/C2 1.0](https://github.com/openlanguageprofiles/olp-en-cefrj/blob/master/octanove-vocabulary-profile-c1c2-1.0.csv) by Octanove Labs, [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/). The index selects C1.                                                                                                  | `18c33a407f2f89f7b8de9671c6d45fe3ea0bce45e7d2d7dcaab48d73e0f7b380` |
| `ngsl.csv`     | [New General Service List 1.2 statistics](https://www.newgeneralservicelist.com/s/NGSL_12_stats.csv) by Charles Browne, Brent Culligan and Joseph Phillips, [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/).                                                                                                                                 | `2098bab8955a120a9766c6282a51d7d578c6cb0a7d946600d2ffb73ba25a0b44` |
| `wikdict.tei`  | [English–Russian FreeDict+WikDict TEI 2025.11.23](https://download.wikdict.com/dictionaries/tei/no-infl/eng-rus.tei), maintained by Karl Bartel, from [Wiktionary contributors](https://en.wiktionary.org/) via [DBnary](https://kaiko.getalp.org/about-dbnary/). The TEI declares [CC BY-SA 3.0 Unported](https://creativecommons.org/licenses/by-sa/3.0/). | `7c48ef3981903361ea90f8ef9a64e58cc7899341d0fb873c2f61198198cba7af` |

Third-party fields in the derived index retain the corresponding attribution and share-alike terms above.

## Development speech playback

Dictionary pronunciation, the bus rehearsal model and the driver's message use the same unofficial Google
Translate web voice as sibling HelpReader. Each request contains only the selected English entry or authored
line. Playback keeps only the latest clip in memory. The repository and APK contain no synthesized clips from
Google. Published builds use a supported or licensed voice source for pronunciation and listening material.
