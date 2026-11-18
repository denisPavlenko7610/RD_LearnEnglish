# RD Learn English — Game Design Document

Version 0.2 · 18 November 2026 · Design authority.

## 1. Product

A personal, calm English learning application for an adult Russian-speaking learner. Working title: **English /
Everyday**.
Use Duolingo only as a reference for approachable interaction. Understanding and practical use take priority. Retain the
offline foundation from sibling HelpReader.
Rewrite the curriculum, progression, screens and tone for an adult. Reuse UnityTemplates infrastructure through source
links.

The core course aims at practical independent use through B2. C1 is a separate optional expansion for nuance,
professional communication and longer material. This is a learning tool, not a proficiency certificate or an exam
guarantee.
Course completion and demonstrated ability are separate measures. No promised learning time or vocabulary-count
equivalence.

Initial assumptions: Russian interface/explanations; English content; Windows first, Android landscape next; one local
profile; offline operation; no account, subscription or backend. These can change without rewriting the learning domain.

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

| Level       | Main situations                                                           | Grammar and language focus                                                                                                 | Demonstration target                                                           |
|-------------|---------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------|
| A1          | Introductions, routines, cafe, shopping, directions, time                 | be, pronouns, articles, plurals, present simple, questions, there is/are, can, basic prepositions                          | Introduce yourself, ask simple questions and handle simple transactions        |
| A2          | Travel problems, appointments, home, work updates, plans                  | past simple/continuous, present continuous, future forms, comparatives, quantifiers, present perfect introduction          | Describe experiences, arrange plans and exchange routine information           |
| B1          | Projects, explanations, opinions, troubleshooting, correspondence         | tense contrasts, present perfect continuous, first/second conditionals, modals, passive, relative clauses, reported speech | Explain a problem, narrate connected events and write a clear everyday message |
| B2          | Meetings, negotiations, technical discussions, decisions, longer articles | third/mixed conditionals, modal deduction, complex passive/reporting, discourse markers, collocations, register            | Follow extended arguments, justify a decision and produce clear connected text |
| C1 optional | Professional writing, complex talks, synthesis, implied meaning           | hedging, emphasis/inversion, register shifts, idiom in context, precision and discourse                                    | Express nuanced meaning and synthesize several substantial sources             |

This is a project curriculum proposal mapped to the Council of Europe framework, not a Council-endorsed syllabus.
Reference: [CEFR descriptors](https://www.coe.int/en/web/common-european-framework-reference-languages/cefr-descriptors).
Use the framework to review reading, listening, production, interaction and mediation separately. Grammar quizzes alone
cannot establish B2 or C1 competence.

Full-course production target, subject to editorial review: A1 24 units, A2 28, B1 32, B2 36; approximately 8–12 skills
per unit and 8–15 distinct exercises per skill. C1 expansion: initially 24 units. These are planning targets, not
present
content counts. Rewordings do not count as new vocabulary or distinct communicative objectives.

## 6. Exercise catalogue

| Type                  | Learner action                                                                | Evaluation                                                                         |
|-----------------------|-------------------------------------------------------------------------------|------------------------------------------------------------------------------------|
| Context choice        | Choose a grammatically and semantically suitable continuation                 | Exactly one authored valid choice                                                  |
| Cloze                 | Type the missing word or phrase in a meaningful sentence                      | Authored alternatives, including legitimate contractions                           |
| Translation           | Type a short useful English sentence from Russian                             | Explicit equivalents; no fuzzy matching                                            |
| Sentence construction | Build a sentence from tokens, then type freely on later reviews               | Authored order/alternatives, duplicates supported                                  |
| Reading               | Read a message/article and answer factual/inference questions                 | Evidence and explanation anchored to the text                                      |
| Listening             | Hear native speech, optionally replay, answer without seeing transcript first | Licensed local audio with transcript revealed after answer                         |
| Speaking              | Listen, repeat, record and compare voluntarily                                | No automatic accuracy claims without a validated recognizer                        |
| Writing / dialogue    | Write a connected reply, role-play or summary                                 | Authored rubric and model answer; self-review clearly separated from scored recall |

First implementation includes context choices, typed cloze, short translation and reading comprehension using the same
answer engine. Token interaction, audio, recording, extended writing and dialogue are later milestones. Never show a
silent button or textual imitation as if listening/speaking is implemented.

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

Calm editorial workspace: warm off-white, dark ink, teal action colour, restrained amber accents, generous whitespace,
readable typography, outlined cards, no childish mascot. English sentences are the strongest visual focus.

- Home: Continue, quiet review count, evidence summary, A1/A2/B1/B2/C1 selectors and selected topic card.
- Topic browser: list/pager of topic names, can-do goals and honest progress; no locked trail or artificial gates.
- Lesson: topic/level, task instruction, English context, typed field or up to four choices, Hint, Check, feedback and
  Next.
- Reference: the topic rule/examples in a dismissible panel; assisted status is visible if consulted during an attempt.
- Settings later: text size, sound/voice, interface language, local export/import with explicit safeguards.

Use authored UXML/USS with reusable classes. No runtime-built visual tree. Keyboard order follows visual order, focus is
visible, feedback uses text plus colour, buttons have comfortable touch targets, paragraphs wrap. Long content can
scroll.
No background music by default. Sound is optional; speech is pedagogical media, separate from interface feedback.

Default PC 1280×720 resizable; verify 1920×1080, 16:10, 4:3 and narrow windows. Android landscape in both directions;
respect safe area and test software keyboard. Portrait is outside the initial scope.

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

No runtime API keys, cloud dependency, analytics or downloaded speech in the slice. Licensed recorded speech is the
preferred release path. Content data is local JSON; deterministic generation and validators live in Tools.

## 10. Milestones and acceptance

M0: inspect references, GDD, project AGENTS and source-link setup.

M1 — playable vertical slice: Boot/App, adult home/lesson UI, original pilot material at A1–B2 plus a clearly marked C1
preview, choices/typed answers/reading, rules and hints, persistent progress, mistake/due review, focused automated
tests.
Accept only after Unity compilation, content validation, actual Play smoke and honest remaining-work record.

M2 — learning depth: expand reviewed A1/A2 corpus, skill-level mastery, optional placement, sentence tokens, dictionary,
settings and import/export. Add native reviewed local audio and listening. Test retention over real study sessions.

M3 — complete core: author/review B1/B2, extended texts, dialogue/writing rubrics and voluntary speaking workflow.
Audit every unit against communicative goals and actual assessment coverage, then physical Android and standalone
builds.

M4 — C1 extension: substantial content, synthesis, register and advanced production. No C1 completion claim from pilot
cards.

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
before describing the application as a complete B2 learning product.

## 12. Efficient adult learning

One integrated course, without extra trainers. Combine a useful dialogue/text, explanation, active recall, contextual
application and later revisiting. Previously encountered choice items switch to typed recall without visible options.
That is evidence of cued recall, not proof of spontaneous conversational ability. Later content adds transfer tasks
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