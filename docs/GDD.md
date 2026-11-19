# RD Learn English — Game Design Document

Version 0.4 · 19 November 2026 · Design authority.

## 1. Product

A personal, calm English learning application for an adult Russian-speaking learner. Working title: **English /
Everyday**.
Use Duolingo only as a reference for approachable interaction. Understanding and practical use take priority.
Rewrite the curriculum, progression, screens and tone for an adult. Reuse UnityTemplates infrastructure through source
links.

The release goal is a complete A1–C1 learning path for practical independent use, including nuance, professional
communication and longer material at C1. Course completion, demonstrated skills and external proficiency results
are separate measures. Individual progress depends on the starting level, practice, interaction and feedback.
The release promises a complete route, continued access to targeted practice and transparent skill evidence.

Android is the primary release platform. Design for phone landscape in both directions and keep the interface
usable in Android-managed window sizes. Windows remains a development and validation target. Use Russian
interface text and explanations, English learning material and one local profile without an account or
subscription.

## 2. Experience pillars

1. Start learning within one action. Home opens directly; placement is optional, never an onboarding barrier.
2. Explain before testing. A short rule, two examples and a practical purpose precede each topic.
3. Train recall and use, not just recognition. Mix typed responses, comprehension, sentence construction and contextual
   choices.
4. Errors guide practice. Give the correct form and a specific reason, then revisit it without punishment.
5. Respect adult time. A suggested 5–15 minute visit, voluntary stop at any point, no forced round or reward animation.
6. Keep progress honest. Show practiced material and independent successes, not a fabricated CEFR assessment.

## 3. Editorial boundaries

Teach everyday life, work, travel, technology, learning, home, food, services, hobbies and neutral professional
situations.
Do not author sexual material, LGBT relationship/identity themes, political campaigning, religious persuasion or
ideological
debate. Do not turn this restriction into hostile language or commentary about people. Names and contexts serve language
learning.
Avoid childish jokes, surreal translations, patronizing praise, celebrity trivia and vocabulary chosen only to create a
puzzle.
Use original sentences; do not copy Duolingo lessons, assets, branding or proprietary courses.

## 4. Learning loop

Home → continue / choose topic → short explanation → recall or application → explanatory feedback → next.
All vocabulary, conversation, phrasal verbs, idioms and occasional proverbs belong to this one flow.
There are no supplementary trainers. Mistakes and due review are integrated into normal learning.

Each topic starts with a short explanation in plain Russian: meaning/purpose, a usable pattern, two examples and a
common
pitfall where relevant. The explanation remains available during practice. Every exercise has an authored explanation
after the answer, not merely a correct/incorrect marker. English remains the language being learned.
The rule remains available on demand during practice. A requested hint marks the current attempt assisted, including
after
closing/reopening the hint. Hints are unlimited, free, untimed and never spend a resource.

One non-empty submission finalizes an attempt. Whitespace-only text is ignored. After submission, disable the answer
controls and show result, expected answer and authored explanation. Next advances; Enter submits or advances; Escape
returns home. Returning home is always possible after persistence finishes. An unanswered card can be left without
penalty.

Every finalized attempt is saved before navigation continues. A disk failure leaves feedback visible and offers Retry
save;
retry writes the same detached snapshot and cannot count the answer twice. Unanswered input need not survive process
exit.

No hearts, lives, energy, leagues, social pressure, streak loss, coins, pets, loot, paid hints, advertisements or forced
notifications.
Quiet daily practice counts may be introduced later only if useful to the learner and never as an obligation.

## 5. Course structure and target scope

Level → practical unit → small skill → authored exercises. Stable IDs survive wording changes. Each unit has a can-do
goal,
a concise explanation, examples, prerequisites for recommendations (not locks), exercise tags and review metadata.
All published topics remain freely selectable; selecting B1 does not erase A1 or require completing it.

| Level | Main situations                                                           | Grammar and language focus                                                                                                 | Demonstration target                                                           |
|-------|---------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------|
| A1    | Introductions, routines, cafe, shopping, directions, time                 | be, pronouns, articles, plurals, present simple, questions, there is/are, can, basic prepositions                          | Introduce yourself, ask simple questions and handle simple transactions        |
| A2    | Travel problems, appointments, home, work updates, plans                  | past simple/continuous, present continuous, future forms, comparatives, quantifiers, present perfect introduction          | Describe experiences, arrange plans and exchange routine information           |
| B1    | Projects, explanations, opinions, troubleshooting, correspondence         | tense contrasts, present perfect continuous, first/second conditionals, modals, passive, relative clauses, reported speech | Explain a problem, narrate connected events and write a clear everyday message |
| B2    | Meetings, negotiations, technical discussions, decisions, longer articles | third/mixed conditionals, modal deduction, complex passive/reporting, discourse markers, collocations, register            | Follow extended arguments, justify a decision and produce clear connected text |
| C1    | Professional writing, complex talks, synthesis, implied meaning           | hedging, emphasis/inversion, register shifts, idiom in context, precision and discourse                                    | Express nuanced meaning and synthesize several substantial sources             |

This is a project curriculum proposal mapped to the Council of Europe framework, not a Council-endorsed syllabus.
Reference: [CEFR descriptors](https://www.coe.int/en/web/common-european-framework-reference-languages/cefr-descriptors).
Use the framework to review reading, listening, production, interaction and mediation separately. Grammar quizzes alone
cannot establish B2 or C1 competence.

Full-course production estimates, subject to editorial review: A1 24 units, A2 28, B1 32, B2 36 and C1 24.
The CEFR coverage matrix, task variety and independent assessments determine readiness. Rewordings do not count
as new vocabulary or distinct communicative objectives.

## 6. Exercise catalogue

| Type                  | Learner action                                                                | Evaluation                                                                               |
|-----------------------|-------------------------------------------------------------------------------|------------------------------------------------------------------------------------------|
| Context choice        | Choose a grammatically and semantically suitable continuation                 | Exactly one authored valid choice                                                        |
| Cloze                 | Type the missing word or phrase in a meaningful sentence                      | Authored alternatives, including legitimate contractions                                 |
| Translation           | Type a short useful English sentence from Russian                             | Explicit equivalents; no fuzzy matching                                                  |
| Sentence construction | Build a sentence from tokens, then type freely on later reviews               | Authored order/alternatives, duplicates supported                                        |
| Reading               | Read a message/article and answer factual/inference questions                 | Evidence and explanation anchored to the text                                            |
| Listening             | Hear native speech, optionally replay, answer without seeing transcript first | Reviewed, licensed audio may be delivered online. Reveal the transcript after answering. |
| Speaking              | Listen, repeat, record and compare voluntarily                                | No automatic accuracy claims without a validated recognizer                              |
| Writing / dialogue    | Write a connected reply, role-play or summary                                 | Authored rubric and model answer; self-review clearly separated from scored recall       |

The pilot includes context choices, typed cloze, short translation, error correction, written dialogues and sentence
assembly. All use the same answer and review system. Extended writing is a later milestone.

Matching: Unicode normalize, trim, collapse whitespace, ignore case and terminal . ! ?. Preserve apostrophes, inner
punctuation, word order and diacritics. Equivalent contractions/spellings are explicit content. No automatic typo
acceptance.
Show the canonical form after an error. Answers with several valid natural renderings require explicit accepted variants
or narrower task wording before release.

## 7. Mastery and scheduling

Each exercise stores independent successes, review stage, due UTC time, failed state and the global attempt index at
which it was last seen. Assisted correct answers neither clear mistakes nor advance mastery. Incorrect answers become
pending mistakes and reset review stage. Mastery is per exercise in the slice; skill aggregation comes after the corpus
grows.

Review intervals: 1, 3, 7, 14 and 30 days. Only an unassisted success at or after due time advances a stage; early
practice
does not pull a future review forward. A failed item is eligible after two other attempts whenever alternatives exist.
In a single-item pool allow repetition and state the reason instead of deadlocking.

Normal study interleaves at most two scheduled/mistake reviews in any five answers while unseen content remains.
Integrated review works across introduced items, prioritizing eligible mistakes then due items. New lessons remain
accessible despite a backlog. Topic selection recommends that topic with bounded review.

First pass = every exercise in a unit answered independently at least once. Retained = independently recalled again on a
later due day through unassisted typed recall. Choice recognition and typed recall are recorded separately. One correct
answer never means fully learned. These labels describe local learning evidence; neither is a CEFR award. Errors never
revoke a first pass.

## 8. Screens and visual direction

The dashboard groups continuation, saved progress and topic selection in separate cards. Editorial fox and owl
illustrations add atmosphere without affecting the learning flow. The topic preview changes accent by category,
shows independent recall with a progress bar and lets the learner open the rule before entering the lesson.
A centered lesson card gives the English question priority. Narrow layouts stack the dashboard and keep vertical
scrolling.

- Home: Continue, quiet review count, evidence summary, A1/A2/B1/B2/C1 selectors and selected topic card.
- Topic browser: list/pager of topic names, can-do goals and honest progress; no locked trail or artificial gates.
- Lesson: topic/level, task instruction, English context, typed field or up to four choices, Hint, Check, feedback and
  Next.
- Reference: the topic rule/examples in a dismissible panel; assisted status is visible if consulted during an attempt.
- Settings: Appearance offers five colour themes: dark Midnight (default), Ocean, Forest and Ember, and light Daylight.
  A header button opens it from home or from a lesson without losing the active exercise. A choice applies at once, is
  marked by a "Выбрано" tag as well as colour, and is stored in its own versioned `settings-v1` slot, never in learning
  progress. A failed write keeps the theme for the session and offers a retry message. An unreadable settings file
  falls back to Midnight without blocking study and is preserved unchanged.
- Settings later: text size, sound/voice, interface language, local export/import with explicit safeguards.

Visual language: calm layered surfaces, one accent colour per theme reserved for actions and selection, rounded cards,
a segmented level selector and chips for filters. Answer feedback uses text plus a calm green or amber, never an alarm
red, and marks the chosen and correct option after an answer. Illustrations stay optional context. Motion is limited to
short hover and focus transitions; there are no celebratory effects.

Use authored UXML/USS with reusable classes. No runtime-built visual tree. Keyboard order follows visual order, focus is
visible, feedback uses text plus colour, buttons have comfortable touch targets, paragraphs wrap. Long content can
scroll.
No background music by default. Sound is optional; speech is pedagogical media, separate from interface feedback.

Android phone landscape is the primary layout. Use both rotations, respect safe areas, and keep the current
answer and actions visible above the software keyboard. On devices where Android changes orientation or window
size, preserve the active lesson and show a usable scrollable layout. Default PC 1280×720 resizable.

## 9. Technical design

Unity 7, UI Toolkit. Keep HelpReader's five-layer separation, Boot/App scenes and shared-source junction workflow.

| Layer          | Responsibility                                                                            |
|----------------|-------------------------------------------------------------------------------------------|
| Core           | Curriculum/profile validation, answer matching, attempts and scheduling; pure C#          |
| Application    | Lesson session, review selection, transactional orchestration and repository contract     |
| Infrastructure | RD.Save adapter, JSON curriculum/localization, RD.Localization provider                   |
| UI             | Bind authored UXML controls; no curriculum or persistence policy                          |
| Bootstrap      | RD.DependencyInjection explicit factories, persistent lifetime, RD.SceneFlow scene switch |

Link only required shared sources: Foundation, DependencyInjection, Localization, Save, SceneFlow. Add Audio when real
audio content is integrated; do not link unrelated mechanics, demos or tests. Preserve source metadata. Never modify the
shared library for learning policy. Fresh clones recreate links with Tools/Link-UnityTemplates.ps1 and need sibling
UnityTemplates.

Version-1 local profile in its own application directory, never HelpReader's save slot. Validate IDs, bounds,
timestamps,
duplicate records and schema before applying. Preserve damaged/unsupported files; show an actionable load failure and
retry, never silently replace progress. RD.Save supplies atomic writes/backups. Save answer/review state together.
No migrations until a public release; later schema changes need versioning, preserved originals and round-trip tests.

Internet access is acceptable for listening and pronunciation. Offline audio and full offline course completion
are outside release acceptance. Keep progress in the local profile. Use a supported or licensed audio source for
release. Content data is local JSON with deterministic generation and validators in Tools.

Development builds request the same English web voice used in HelpReader for dictionary entries and short
authored lesson lines. Requests run only when the learner presses the playback button. They contain the
selected entry or authored line. Learner answers, recordings and progress stay local.

## 10. Milestones and acceptance

M0: inspect references, GDD, project AGENTS and source-link setup.

M1 — playable vertical slice: Boot/App, adult home/lesson UI, original pilot material at A1–B2 plus a clearly marked C1
preview, choices/typed answers/reading, rules and hints, persistent progress, mistake/due review, focused automated
tests.
Accept only after Unity compilation, content validation, actual Play smoke and honest remaining-work record.

M2 — Android foundation and an end-to-end A1 unit with audio, oral response, writing, recall and delayed review.

M3 — finish and review A1–A2, then B1–B2. Add optional placement, skill-level evidence, verified vocabulary senses,
realistic dialogue and longer production. Run delayed assessments and device regression checks at each level.

M4 — finish and review C1, including sustained listening and reading, flexible interaction, extended writing and
spoken synthesis. Release the complete route once the course and Android pass the acceptance criteria in
[Course delivery plan](COURSE_PLAN.md).

## 11. Quality and risks

Automated checks cover stable/unique IDs, accepted answers, choice uniqueness, nonempty rules/explanations, level
coverage,
save validation, matching edge cases, assisted mastery, UTC scheduling, review bounds, restart and failing-write retry.
Screens must import without UXML/USS warnings. Boot starts in Editor and player; build scenes exist; repeated Play is
safe.
Use an isolated save root for checks; never overwrite the learner's profile.

Native English review and pedagogical sequencing remain release requirements. Open-text alternatives are a major content
risk; keep tasks narrow until reviewed. A small pilot across levels demonstrates architecture, not a complete course.
Recognition-only practice cannot deliver the stated goal: listening, speaking and connected writing must be delivered
before describing the application as a complete A1–C1 learning product.

## 12. Efficient adult learning

### Vocabulary and phrasal verb lessons

After a vocabulary mistake, an eligible review can use the alternate sentence for the same meaning and saved
exercise ID. Failed attempts still follow normal mistake spacing during automatic review. A manual retry from
the dictionary can open the alternate sentence immediately.

The bus reference lesson includes an optional spoken rehearsal and a short driver message. The default action
plays the message using the same online development voice as the dictionary. The learner can reveal the
transcript for support, and the lesson reveals it after submission. A playback failure offers the transcript
and allows another playback attempt. Record general message comprehension for this practice activity.
The rehearsal supports an optional microphone recording, local playback and deletion. Keep the recording in
memory for the rehearsal and clear it when the learner leaves the activity or the app is suspended.
The vocabulary catalog is an offline reference across A1–C1, ordered by general frequency where ranking data is
available. Search covers English headwords and Russian glosses. Word levels are source tags for individual entries.
Catalog entries with authored practice link to their lesson. Each assesses a specific sense through typed recall
in two contexts under the same stable exercise ID. Catalog evidence reads the existing saved attempts, distinguishing
new material, assisted practice, independent recall, delayed recall and material due for review. Imported reference
entries remain searchable and carry no assessment status until a corresponding exercise is authored.

High-utility vocabulary and phrasal verbs are a primary path in the topic picker. Each focused lesson explains a few
related expressions in Russian, gives English examples and contrasts common confusions. Practice combines contextual
choices, typed gaps, correction of common errors, translation and sentence assembly. Assembly uses a shuffled set of
tokens, supports undo, and switches to typed recall after the first recorded attempt. Assembly success counts as guided
recognition and never as independent typed recall. All formats use the same lesson flow and saved review queue. The
topic picker can filter by vocabulary and phrasal verbs. The pilot remains text-only.

### Dialogues inside lessons

Scenario lessons introduce one practical goal at a time. Initial practice moves from a contextual choice and guided
sentence assembly to a short typed gap, a written reply and reading comprehension. A recurring setting gives the
exercises a shared purpose, while each card contains the information needed to answer it on its own. The typed gap
uses an alternate authored situation on selected scheduled reviews. Prompts constrain short replies to a taught
expression so that feedback can explain the specific language decision.

Twenty-eight pilot cards place short written conversations in A1–B2 topics. Learners read the exchange, type a short
English answer, and receive a model answer and a short explanation in Russian. Dialogue cards use the existing review
schedule and assisted-answer semantics.

One integrated course, without extra trainers. Combine a useful dialogue/text, explanation, active recall, contextual
application and later revisiting. Previously encountered choice items switch to typed recall without visible options.
Selected cards use a second authored situation during due review. That situation tests the same target using typed
recall, keeps its own hint and explanation, and retains the original exercise ID and review schedule. Early practice
continues with the original situation. This measures cued recall in context. Later content adds broader transfer tasks
in new situations and connected output with explicit rubrics. No opaque AI score or unvalidated speaking grade.

Teach useful words together with collocations and sentence frames: make a decision, meet a deadline, Could you
clarify...?
Phrasal verbs need a specific sense, object pattern, separability and register: turn it off, look into an issue, put off
a meeting.
Teach other senses later in context, not as an unstructured list. Conversation includes clarification, polite
disagreement,
requests, repair and follow-up questions. Explain neutral, informal, polite and formal tone.

Idioms and proverbs are different. Give meaning, natural context, register, a plain alternative and common mistakes.
Prioritize useful frequent expressions over rare sayings. Proverbs are occasional interest material, never a B2
prerequisite.
Explain when a proverb sounds clich?d or dismissive. Do not claim corpus frequency without a documented source.

Progress shows encountered items, independent answers, typed recall, delayed typed recall and items needing review.
Recognition-only success cannot establish retention. Assisted answers are visible but do not advance independent
mastery.
Generated editorial illustrations are optional instructional context, never rewards or answer keys. A cafe scene
supports
ordering language; abstract grammar needs no decorative picture.

Research basis and limits:

- [Nation: The Four Strands](https://www.wgtn.ac.nz/lals/resources/paul-nations-resources/paul-nations-publications/publications/documents/1996-Four-strands.pdf):
  balance meaningful input, output, deliberate language work and fluency across the course. Our sequence is an
  application of the framework, not an experimental finding.
- [L2 vocabulary retrieval, feedback and spacing study](https://journals.sagepub.com/doi/full/10.1177/13621688211053525):
  supports combining retrieval with feedback and spacing over restudy alone; it does not validate this app or exact
  1/3/7/14/30 intervals.
- [Successful effortful L2 retrieval study](https://www.cambridge.org/core/journals/applied-psycholinguistics/article/abs/testing-the-reminding-account-of-the-lag-effect-in-l2-vocabulary-learning/4D03D1F5169B719D90F14BBE474015F5):
  motivates achievable recall with feedback; results are bounded by the study task.
- [Formulaic sequences: principle and practice](https://doi.org/10.1093/applin/21.4.463): motivates collocations, frames
  and idiomatic expressions, not memorization of proverbs as a shortcut to fluency.