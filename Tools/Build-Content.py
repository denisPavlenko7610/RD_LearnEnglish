import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
units = []


def unit(uid, level, title, goal, rule, cards):
    topic = dict(id=uid, level=level, title=title, goal=goal, rule=rule, exercises=[
        dict(id=f'{uid}.{i + 1:02}', prompt=p, context=c, answers=a if isinstance(a, list) else [a],
             options=o, hint=h, explanation=e)
        for i, (p, c, a, o, h, e) in enumerate(cards)])
    units.append(topic)
    return topic


CHOICE = 'Выберите подходящий вариант'
GAP = 'Введите пропущенное слово или выражение'
TRANSLATE = 'Переведите на английский'
READ = 'Прочитайте и ответьте по-английски'

unit('a1.introductions', 'A1', 'Знакомство и работа', 'Представиться и рассказать, чем вы занимаетесь.',
     'Для I используйте am, для he/she/it — is, для you/we/they — are. В вопросе be стоит перед подлежащим.\n\nI am an engineer. — Я инженер.\nAre you ready? — Вы готовы?',
     [
         (CHOICE, 'I ___ a software developer.', 'am', ['are', 'am', 'is', 'be'],
          'Сначала найдите подлежащее: здесь это I.', 'После I нужна форма am: I am a software developer.'),
         (GAP, 'My colleague ___ from London.', 'is', [], 'My colleague можно заменить местоимением he или she.',
          'Единственное число третьего лица требует is.'),
         (TRANSLATE, 'Мы готовы.', ["We are ready", "We're ready"], [], 'Глагол be нужен и там, где в русском его нет.',
          'We are ready. Сокращённая форма: We’re ready.'),
         (READ, 'My name is Alex. I am an engineer. I work in Kyiv.\n\nWhat is Alex’s job? Write the job only.',
          ['an engineer', 'engineer'], [], 'Найдите предложение с I am.', 'Alex говорит: I am an engineer.')])

unit('a1.routines', 'A1', 'Обычный рабочий день', 'Описать привычки и задать простой вопрос.',
     'Present Simple описывает привычки. После he/she/it добавляйте -s. В вопросах используйте do/does; после does глагол без -s.\n\nI work from home.\nDoes she work on Fridays?',
     [
         (CHOICE, 'She ___ work at nine every day.', 'starts', ['start', 'starting', 'starts', 'is start'],
          'Every day указывает на привычку.', 'She — третье лицо: starts.'),
         (GAP, '___ you work from home?', 'Do', [], 'Это вопрос о привычке с подлежащим you.',
          'Вопрос Present Simple: Do you work from home?'),
         (TRANSLATE, 'Я работаю каждый день.', 'I work every day', [],
          'Для регулярного действия используйте Present Simple.', 'I work every day. После I окончание -s не нужно.'),
         (READ,
          'Dan works from home on Mondays. On Tuesdays he goes to the office.\n\nWhen does he work from home? Write the day.',
          ['Monday', 'Mondays', 'on Mondays', 'on Monday'], [], 'Нужен день работы из дома, а не поездки в офис.',
          'В тексте: from home on Mondays.')])

unit('a1.services', 'A1', 'Кафе и простые просьбы', 'Вежливо попросить нужное и понять цену.',
     'Can I have ...? — простая вежливая просьба. Для одного исчисляемого предмета нужен a/an. Перед гласным звуком — an.\n\nCan I have a coffee, please?\nI would like an apple.',
     [
         (CHOICE, 'I would like ___ apple.', 'an', ['a', 'an', 'two', 'many'],
          'Ориентируйтесь на первый звук следующего слова.', 'Apple начинается с гласного звука, поэтому an apple.'),
         (GAP, 'Can I ___ a coffee, please?', 'have', [], 'Это устойчивый способ попросить что-то.',
          'Can I have a coffee, please?'),
         (TRANSLATE, 'Спасибо.', ['Thank you', 'Thanks'], [], 'Подойдёт и полная, и короткая повседневная форма.',
          'Thank you и Thanks — стандартные способы поблагодарить.'),
         (READ,
          'Tea costs three pounds. Coffee costs four pounds.\n\nHow much is the coffee? Write the amount in words.',
          'four pounds', [], 'Найдите цену coffee.', 'Coffee costs four pounds.')])

unit('a2.past', 'A2', 'Как прошёл день', 'Рассказать о завершённых событиях.',
     'Past Simple нужен для завершённого прошлого. В отрицании did not / didn’t + начальная форма. Неправильные формы учим в контексте.\n\nI finished the report yesterday.\nI didn’t go to the office.',
     [
         (CHOICE, 'Yesterday I ___ the report.', 'finished', ['finish', 'finished', 'have finish', 'finishing'],
          'Yesterday задаёт завершённое прошлое.', 'Finished — Past Simple от finish.'),
         (GAP, 'I did not ___ to the office yesterday.', 'go', [],
          'После did not глагол возвращается к начальной форме.', 'Did not go, а не did not went.'),
         (TRANSLATE, 'Я работал вчера.', 'I worked yesterday', [], 'Work — правильный глагол.',
          'I worked yesterday. Для Past Simple добавлено -ed.'),
         (READ, 'I called the hotel on Monday and arrived on Wednesday.\n\nWhen did I arrive? Write the day.',
          ['Wednesday', 'on Wednesday'], [], 'Различайте called и arrived.',
          'Arrived on Wednesday — приехал в среду.')])

unit('a2.plans', 'A2', 'Планы и договорённости', 'Обсудить намерение и уже назначенную встречу.',
     'Be going to + глагол — намерение. Present Continuous с будущим временем — договорённость. Will часто выражает решение в момент речи.\n\nI am going to study tonight.\nWe are meeting the client tomorrow.',
     [
         (CHOICE, 'We have booked a table. We ___ the client tomorrow.', 'are meeting',
          ['met', 'are meeting', 'has met', 'meeting'], 'Бронирование уже сделано: встреча согласована.',
          'Present Continuous подходит для будущей договорённости.'),
         (GAP, 'I am going ___ study tonight.', 'to', [], 'После going в этой конструкции нужна связка с инфинитивом.',
          'Be going to + study.'),
         (TRANSLATE, 'Я собираюсь работать завтра.', ["I am going to work tomorrow", "I'm going to work tomorrow"], [],
          'Речь о намерении: be going to.', 'I am going to work tomorrow.'),
         (READ,
          'The meeting was planned for ten, but the client moved it to eleven.\n\nWhat time is the meeting now? Write the time in words.',
          ['eleven', 'at eleven', "eleven o'clock", "at eleven o'clock"], [], 'Нужно новое время после переноса.',
          'The client moved it to eleven.')])

unit('a2.experience', 'A2', 'Опыт и результат', 'Сказать, что уже сделано, и сравнить варианты.',
     'Present Perfect: have/has + past participle. Он связывает прошлое с настоящим. При точном завершённом времени нужен Past Simple.\n\nI have sent the email.\nI sent it yesterday.',
     [
         (CHOICE, 'She ___ already sent the email.', 'has', ['have', 'has', 'did', 'is'],
          'Нужна форма вспомогательного глагола для she.', 'She has sent — Present Perfect.'),
         (GAP, 'I have never ___ to London. Use the participle of be.', 'been', [], 'Нужна третья форма be.',
          'Have been to означает опыт посещения места.'),
         (TRANSLATE, 'Я уже закончил.', ["I have already finished", "I've already finished"], [],
          'Already обычно стоит между have и причастием.', 'I have already finished.'),
         (READ,
          'I have used both laptops. The black one is faster, but the silver one is cheaper.\n\nWhich one costs less? Write the colour only.',
          'silver', [], 'Costs less соответствует cheaper.', 'The silver one is cheaper.')])

unit('b1.duration', 'B1', 'Опыт работы и длительность', 'Различить начало периода и его продолжительность.',
     'For — продолжительность; since — момент начала. Present Perfect Continuous подчёркивает продолжающуюся деятельность.\n\nI have worked here since 2020.\nI have been studying for two hours.',
     [
         (CHOICE, 'I have worked here ___ 2020.', 'since', ['for', 'since', 'during', 'until'],
          '2020 — точка начала, а не длительность.', 'Since обозначает начало периода.'),
         (GAP, 'I have been studying ___ two hours.', 'for', [], 'Two hours — длительность.',
          'For two hours — в течение двух часов.'),
         (TRANSLATE, 'Я работаю здесь с 2020 года.',
          ["I have worked here since 2020", "I've worked here since 2020", "I have been working here since 2020",
           "I've been working here since 2020"], [], 'Действие началось в прошлом и связано с настоящим.',
          'Оба варианта Present Perfect допустимы для этой ситуации.'),
         (READ,
          'Mia joined the team in 2021. She still works there and leads the support group.\n\nWhich year did she join? Write the year in digits.',
          '2021', [], 'Найдите момент начала работы.', 'Mia joined the team in 2021.')])

unit('b1.conditions', 'B1', 'Реальные и возможные решения', 'Обсудить условие и предположить другой вариант.',
     'First conditional: if + Present Simple, will + глагол — реальная возможность. Second conditional: if + Past Simple, would + глагол — гипотеза.\n\nIf it rains, I will stay home.\nIf I had more time, I would study more.',
     [
         (CHOICE, 'If the build passes, we ___ it tomorrow.', 'will release',
          ['released', 'will release', 'would released', 'releasing'], 'Условие реально и относится к будущему.',
          'First conditional: if + present, will + infinitive.'),
         (GAP, 'If I had more time, I ___ study more.', 'would', [], 'Had здесь вводит гипотетическую ситуацию.',
          'Second conditional использует would в результате.'),
         (TRANSLATE, 'Если пойдёт дождь, я останусь дома.',
          ["If it rains, I will stay at home", "If it rains, I'll stay at home", "If it rains, I will stay home",
           "If it rains, I'll stay home"], [], 'Не ставьте will в части после if.',
          'If it rains, I will stay at home.'),
         (READ,
          'If the delivery arrives before noon, we will install the equipment today. Otherwise, we will install it tomorrow.\n\nIt arrives at 10 a.m. When will installation happen? Write today or tomorrow.',
          'today', [], '10 a.m. — до полудня.', 'Условие before noon выполнено, значит today.')])

unit('b1.messages', 'B1', 'Рабочие сообщения', 'Понять просьбу и описать действие в пассиве.',
     'Passive: be + past participle. Он полезен, когда важен результат, а исполнитель второстепенен. Could you ...? смягчает просьбу.\n\nThe file was deleted.\nCould you send the report?',
     [
         (CHOICE, 'The file ___ yesterday.', 'was deleted', ['was deleted', 'has delete', 'is deleting', 'delete'],
          'Файл не выполняет действие сам; время — вчера.', 'Past Simple Passive: was deleted.'),
         (GAP, 'Could you ___ me the report? Use send.', 'send', [], 'После could нужен глагол без to.',
          'Could you send me the report?'),
         (TRANSLATE, 'Отчёт был отправлен вчера.', 'The report was sent yesterday', [],
          'Нужен пассив в прошлом; send — sent — sent.', 'The report was sent yesterday.'),
         (READ,
          'Please send the draft by Friday. The final version is due next Tuesday.\n\nWhat is due on Friday? Write draft or final version.',
          ['draft', 'the draft'], [], 'У черновика и финальной версии разные сроки.', 'By Friday относится к draft.')])

unit('b2.hypotheses', 'B2', 'Ошибки прошлого и выводы', 'Обсудить нереальное прошлое и обоснованный вывод.',
     'Third conditional: if + had + participle, would have + participle. Must have + participle — уверенный вывод о прошлом.\n\nIf we had tested it, we would have found the bug.\nShe must have missed the email.',
     [
         (CHOICE, 'If we had tested it earlier, we ___ the bug.', 'would have found',
          ['will find', 'would have found', 'had find', 'would finding'], 'Нереальное условие относится к прошлому.',
          'Результат third conditional: would have found.'),
         (GAP, 'She must ___ missed the email.', 'have', [],
          'Для вывода о прошлом нужна конструкция modal + have + participle.',
          'Must have missed — должно быть, пропустила.'),
         (TRANSLATE, 'Если бы я знал, я бы помог.',
          ["If I had known, I would have helped", "If I had known, I'd have helped",
           "If I'd known, I would have helped", "If I'd known, I'd have helped"], [],
          'Обе части относятся к несостоявшемуся прошлому.', 'If I had known, I would have helped.'),
         (READ,
          'The server logs show successful requests until 14:05. No requests were recorded after the power failure at 14:06.\n\nWhen did the power fail? Use HH:MM.',
          '14:06', [], 'Отделите время последнего запроса от времени сбоя.', 'The power failure произошёл at 14:06.')])

unit('b2.decisions', 'B2', 'Сравнение решений', 'Построить аргумент и указать на ограничение.',
     'Although вводит придаточное с подлежащим и глаголом. Despite ставится перед существительным или -ing. However связывает самостоятельные утверждения.\n\nAlthough it is costly, it saves time.\nDespite the cost, we chose it.',
     [
         (CHOICE, '___ the higher cost, we chose the safer option.', 'Despite',
          ['Although', 'Despite', 'However', 'Even'], 'После пропуска — именная группа the higher cost.',
          'Despite + noun phrase. Although требует придаточного.'),
         (GAP, 'Although the tool ___ expensive, it saves time.', 'is', [], 'В придаточном нужен полноценный глагол.',
          'Although the tool is expensive — грамматически полное придаточное.'),
         (TRANSLATE, 'Несмотря на стоимость, мы выбрали это.',
          ['Despite the cost, we chose it', 'In spite of the cost, we chose it'], [],
          'Перед существительным подходят despite или in spite of.', 'Despite the cost, we chose it.'),
         (READ,
          'Option A is cheaper initially, but requires monthly maintenance. Option B costs more upfront and needs no monthly maintenance.\n\nWhich option needs monthly maintenance? Write A or B.',
          ['A', 'option A'], [], 'Сравните текущие расходы, а не начальную цену.',
          'Option A requires monthly maintenance.')])

unit('b2.reporting', 'B2', 'Передача информации', 'Передать сообщение и сформулировать косвенный вопрос.',
     'В косвенном вопросе порядок слов утвердительный: where the file is. В прошедшем reported speech времена часто сдвигаются назад, если контекст этого требует.\n\nCould you tell me where the file is?\nHe said that he was busy at that moment.',
     [
         (CHOICE, 'Could you tell me ___?', 'where the file is',
          ['where is the file', 'where the file is', 'where does the file', 'where file is the'],
          'После вводной просьбы не нужна инверсия прямого вопроса.', 'Косвенный вопрос: where the file is.'),
         (GAP, 'He said he ___ busy at that moment. Use the past form of be.', 'was', [],
          'Подлежащее he, время — прошедшее.', 'He said he was busy at that moment.'),
         (TRANSLATE, 'Не могли бы вы сказать мне, где файл?',
          ['Could you tell me where the file is', 'Could you please tell me where the file is'], [],
          'Сохраните прямой порядок слов внутри косвенного вопроса.', 'Could you tell me where the file is?'),
         (READ,
          'The manager said the launch had been delayed because testing was incomplete. A new date has not been announced.\n\nHas a new date been announced? Write yes or no.',
          'no', [], 'Различайте причину задержки и информацию о новой дате.', 'A new date has not been announced.')])

unit('c1.precision', 'C1', 'Точность и осторожные выводы', 'Ознакомиться со смягчением категоричных утверждений.',
     'Предварительный модуль C1. Hedging помогает соотнести уверенность с доказательствами: appears to, may, is likely to. Не подменяйте вероятность гарантией.\n\nThe change appears to have improved performance.\nThe evidence suggests a possible link.',
     [
         (CHOICE, 'Preliminary data ___ that the change may improve reliability.', 'suggest',
          ['prove beyond doubt', 'guarantee', 'suggest', 'make certain'],
          'Preliminary data — основание для осторожного, а не окончательного вывода.',
          'Suggest соответствует предварительным данным и may.'),
         (GAP, 'The change appears ___ have improved performance.', 'to', [],
          'После appears перед have нужен инфинитивный маркер.', 'Appears to have improved — перфектный инфинитив.'),
         (TRANSLATE, 'Это может быть полезно.',
          ['This may be useful', 'This might be useful', 'It may be useful', 'It might be useful'], [],
          'Сохраните неопределённость: may или might.', 'May/might выражают возможность, а не гарантию.'),
         (READ,
          'The sample was small and drawn from one team. The results suggest a benefit, but broader testing is required.\n\nCan the results be treated as conclusive? Write yes or no.',
          'no', [], 'Обратите внимание на ограничения выборки.',
          'Малая выборка и необходимость дальнейших тестов исключают окончательный вывод.')])

unit('c1.emphasis', 'C1', 'Акцент в формальной речи', 'Ознакомиться с инверсией после ограничительных выражений.',
     'Предварительный модуль C1. После Only then / Not until в начале предложения возникает инверсия во главной части.\n\nOnly then did we understand the risk.\nNot until Friday did we receive confirmation.',
     [
         (CHOICE, 'Only then ___ the full impact.', 'did we understand',
          ['we understood', 'did we understand', 'we did understood', 'understood we did'],
          'Выражение Only then вынесено в начало.', 'Инверсия: did + subject + base verb.'),
         (GAP, 'Not until Friday ___ we receive confirmation.', 'did', [],
          'Инверсия в Past Simple требует вспомогательного глагола.', 'Not until Friday did we receive confirmation.'),
         (TRANSLATE, 'Только тогда мы поняли риск. Начните с Only then.', 'Only then did we understand the risk', [],
          'После did используйте начальную форму understand.', 'Only then did we understand the risk.'),
         (READ,
          'Only after the audit did the team discover the discrepancy. Before the audit, the figures had appeared consistent.\n\nWas the discrepancy discovered before the audit? Write yes or no.',
          'no', [], 'Only after указывает, когда стало известно о расхождении.',
          'Расхождение обнаружили только после аудита.')])

unit('c1.synthesis', 'C1', 'Сопоставление источников', 'Ознакомиться с точным сравнением выводов.',
     'Предварительный модуль C1. Whereas подчёркивает различие, both — совпадение, neither — отрицание для двух объектов. При обобщении сохраняйте ограничения источников.\n\nBoth reports identify a delay.\nThe first focuses on cost, whereas the second examines reliability.',
     [
         (CHOICE, 'The first report examines cost, ___ the second focuses on reliability.', 'whereas',
          ['because of', 'whereas', 'despite', 'in spite'], 'Нужен союз для противопоставления двух полных частей.',
          'Whereas связывает два различающихся утверждения.'),
         (GAP, '___ reports identify a delay. Use the word meaning the two together.', 'Both', [],
          'Нужен определитель, включающий два отчёта.', 'Both reports — оба отчёта.'),
         (TRANSLATE, 'Оба отчёта указывают на задержку.',
          ['Both reports indicate a delay', 'Both reports identify a delay', 'Both reports point to a delay'], [],
          'Не заменяйте совпадающий вывод причинным объяснением.', 'Both reports indicate a delay.'),
         (READ,
          'Report A attributes the delay to staffing. Report B identifies a delay too, but does not establish its cause.\n\nDo both reports establish staffing as the cause? Write yes or no.',
          'no', [], 'Совпадение по факту задержки не означает совпадение по причине.',
          'Только report A связывает задержку со staffing.')])

strings = {
    'brand': 'АНГЛИЙСКИЙ / ПРАКТИКА', 'tagline': 'Английский для реальной жизни', 'homeNav': 'Обучение',
    'reviewNav': 'Повторение', 'eyebrow': 'ЛИЧНЫЙ КУРС',
    'headline': 'Практика\nанглийского',
    'subtitle': 'Слова, фразовые глаголы и практика в контексте.',
    'continueButton': 'Продолжить обучение', 'reviewButton': 'Повторить трудное',
    'courseLabel': 'ВЫБЕРИТЕ ТЕМУ', 'previousUnit': 'Назад', 'nextUnit': 'Далее',
    'startButton': 'Разобрать тему', 'beginButton': 'Перейти к практике', 'homeButton': 'К темам',
    'hintButton': 'Подсказка',
    'ruleButton': 'Правило', 'checkButton': 'Проверить', 'advanceButton': 'Дальше',
    'retryButton': 'Повторить сохранение', 'dismissButton': 'Понятно', 'answerLabel': 'Ваш ответ',
    'pilotNotice': 'Пилотный курс · A1–B2 + предварительные темы C1',
    'noPressure': 'Без жизней и штрафов. Возвращайтесь, когда удобно.',
    'bootLoading': 'Готовим пространство для практики…', 'bootRetry': 'Повторить запуск',
    'bootError': 'Не удалось загрузить данные. Файлы прогресса сохранены. Проверьте доступ к папке приложения и повторите запуск.',
    'progress': '{0} из {1} заданий пройдено самостоятельно', 'reviewCount': 'К повторению: {0}',
    'unitCounter': 'Тема {0} из {1}', 'correct': 'Верно', 'incorrect': 'Разберём этот ответ',
    'assisted': 'С подсказкой · самостоятельное закрепление ещё впереди',
    'expected': 'Вариант ответа: {0}', 'saved': 'Прогресс сохранён',
    'saveError': 'Ответ учтён один раз. Не удалось сохранить: проверьте доступ к диску и повторите сохранение.',
    'saving': 'Сохраняем…', 'emptyReview': 'На сегодня всё повторено. Можно выбрать новую тему.',
    'emptyAnswer': 'Сначала введите ответ.',
    'singleReview': 'Повторение · если других заданий пока нет, карточка может вернуться сразу.',
    'levelPreview': 'Предварительная тема C1', 'lessonLabel': 'ПРАКТИКА', 'ruleTitle': 'Коротко о главном',
    'hintTitle': 'Подсказка',
    'ruleAssisted': 'Обращение к правилу отмечает текущий ответ как выполненный с подсказкой.',
    'statSeenLabel': 'Встречалось', 'statRecallLabel': 'По памяти', 'statDelayedLabel': 'Через время',
    'evidenceNew': 'Новое задание · сначала разберитесь в смысле',
    'evidenceMistake': 'Нужно повторить · раньше здесь была ошибка',
    'evidenceRetained': 'Вспомнилось через время · это ещё не проверка свободной речи',
    'evidenceRecall': 'Воспроизведено по памяти · предстоит проверка через время',
    'evidenceSeen': 'Уже встречалось · теперь попробуйте без вариантов ответа',
    'recallPrompt': 'Вспомните ответ и введите по-английски без вариантов',
    'contextReviewPrompt': 'Повторение в новой ситуации. Заполните пропуск по-английски.',
    'dialoguePrompt': 'Прочитайте диалог и ответьте по-английски.',
    'progressTitle': 'Ваш прогресс',
    'topicDialogue': 'В теме есть диалог',
    'filterAll': 'Все темы', 'filterWords': 'Слова', 'filterPhrasal': 'Фразовые глаголы',
    'assemblyEmpty': 'Нажимайте на слова в нужном порядке', 'assemblyUndo': 'Убрать последнее',
    'assemblyRecall': 'Напишите фразу по-английски без подсказки из слов.',
    'topicEvidence': 'По памяти: {0} из {2} · Через время: {1} из {2}',
    'openPreviewRule': 'Краткое правило', 'closePreviewRule': 'Свернуть правило',
    'wordTopicLabel': '{0} · СЛОВА', 'phrasalTopicLabel': '{0} · ФРАЗОВЫЕ ГЛАГОЛЫ',
    'generalTopicLabel': '{0} · УРОК',
    'settingsButton': 'Оформление', 'settingsTitle': 'Оформление',
    'settingsSubtitle': 'Выберите цветовую тему. Она меняет только внешний вид и сохраняется на этом устройстве.',
    'settingsDone': 'Готово', 'themeSaved': 'Тема сохранена',
    'themeSaveError': 'Тема применена, но не сохранилась на диск. Нажмите на неё ещё раз, чтобы повторить сохранение.',
    'themeSelected': 'Выбрано',
    'themeMidnightName': 'Полночь', 'themeMidnightNote': 'Тёмная · индиго',
    'themeOceanName': 'Океан', 'themeOceanNote': 'Тёмная · бирюза',
    'themeForestName': 'Лес', 'themeForestNote': 'Тёмная · зелень',
    'themeEmberName': 'Уголь', 'themeEmberNote': 'Тёмная · янтарь',
    'themeDaylightName': 'День', 'themeDaylightNote': 'Светлая · фиолетовый',
    'vocabularyButton': 'Словарь', 'vocabularyTitle': 'Словарь A1–C1', 'vocabularyBack': 'К темам',
    'vocabularySearch': 'Поиск по-английски или по-русски', 'vocabularyLevel': 'Уровень',
    'vocabularyKind': 'Тип', 'vocabularyScope': 'Показать', 'vocabularyAllLevels': 'Все уровни',
    'vocabularyAllKinds': 'Все типы', 'vocabularyKind_words': 'Слова', 'vocabularyKind_expressions': 'Выражения',
    'vocabularyKind_phrasal': 'Фразовые глаголы', 'vocabularyWithPractice': 'С заданиями',
    'vocabularyWholeCatalog': 'Весь словарь', 'vocabularyDue': 'К повторению',
    'vocabularyEmpty': 'В этом разделе ничего не найдено. Измените поиск или фильтры.',
    'vocabularyPrevious': 'Назад', 'vocabularyNext': 'Далее', 'vocabularyPage': '{0} / {1} · записей: {2}',
    'vocabularyPractice': 'Проверить себя', 'vocabularyReturn': 'К словарю',
    'wordSpeech': 'Озвучить', 'wordSpeechLoading': 'Загружаем озвучку…',
    'wordSpeechUnavailable': 'Озвучка недоступна. Можно продолжить с текстом.',
    'speakModel': 'Послушать пример', 'speakSource': 'Послушать онлайн',
    'sourceSpeechUnavailable': 'Озвучка недоступна. Попробуйте ещё раз или откройте расшифровку.',
    'showSource': 'Показать расшифровку', 'hideSource': 'Скрыть расшифровку',
    'modelPractice': 'Скажите вслух, чтобы уточнить маршрут:',
    'recordModel': 'Записать себя', 'stopRecording': 'Остановить запись',
    'playRecording': 'Послушать себя', 'clearRecording': 'Удалить запись',
    'recordingActive': 'Идёт запись · до 10 секунд',
    'microphonePermission': 'Разрешите микрофон и снова нажмите «Записать себя».',
    'microphoneUnavailable': 'Микрофон недоступен. Можно произнести фразу вслух.',
    'vocabularyEntry': '{0} · {1} · {2}', 'vocabularyEntryDetail': '{0}\n{1}',
    'vocabularyNew': 'Ещё не проверено', 'vocabularyNeedsPractice': 'Нужно повторить',
    'vocabularyAssisted': 'Самостоятельный ответ ещё впереди', 'vocabularyRecalled': 'Вспомнили самостоятельно',
    'vocabularyRetained': 'Вспомнили через время', 'vocabularyReviewDue': '{0} · пора повторить',
    'vocabularyReferenceOnly': 'Справочник', 'vocabularyNoGloss': 'Перевод пока отсутствует',
    'vocabularySources': 'Уровни: CEFR-J 1.5 (Yukio Tono, TUFS), Octanove 1.0 (CC BY-SA 4.0). Частотность: NGSL 1.2 (Browne, Culligan, Phillips; CC BY-SA 4.0). Переводы: WikDict / Wiktionary / DBnary (CC BY-SA 3.0). Подробнее в docs/ASSETS.md.',
    'vocabularyPos_noun': 'сущ.', 'vocabularyPos_verb': 'глагол', 'vocabularyPos_adjective': 'прил.',
    'vocabularyPos_adverb': 'наречие', 'vocabularyPos_pronoun': 'местоимение', 'vocabularyPos_preposition': 'предлог',
    'vocabularyPos_conjunction': 'союз', 'vocabularyPos_interjection': 'междометие',
    'vocabularyPos_determiner': 'определитель', 'vocabularyPos_number': 'числительное',
    'vocabularyPos_modal auxiliary': 'модальный глагол', 'vocabularyPos_be-verb': 'глагол be',
    'vocabularyPos_do-verb': 'глагол do', 'vocabularyPos_have-verb': 'глагол have',
    'vocabularyPos_infinitive-to': 'частица', 'vocabularyPos_': 'слово',
}

from ContentExpressions import extend, review_contexts
from ContentDialogues import DIALOGUES
from ContentScenarios import extend_scenarios
from ContentVocabulary import extend_vocabulary
from ContentWordLessons import extend_word_lessons

extend(unit, CHOICE, GAP, TRANSLATE, READ)
extend_word_lessons(unit, CHOICE, GAP)
exercises_by_id = {card['id']: card for topic in units for card in topic['exercises']}
for card_id, context, hint, explanation in review_contexts:
    exercises_by_id[card_id]['reviewContext'] = dict(context=context, hint=hint, explanation=explanation)
units_by_id = {topic['id']: topic for topic in units}
for key in ('a2.word-use', 'b1.collocations'):
    units_by_id[key]['track'] = 'words'
for key in ('b1.phrasal-actions', 'b1.phrasal-work'):
    units_by_id[key]['track'] = 'phrasal'
for topic, key, lines, question, answers, hint, explanation in DIALOGUES:
    units_by_id[topic]['exercises'].append(dict(
        id=f'{topic}.dialogue.{key}', prompt=strings['dialoguePrompt'], context=question, answers=answers,
        options=[], hint=hint, explanation=explanation,
        dialogue='\n\n'.join(f'{name}: {line}' for name, line in lines)))
extend_scenarios(unit, CHOICE, GAP, READ, strings['dialoguePrompt'])
extend_vocabulary(unit)
units.sort(key=lambda x: ['A1', 'A2', 'B1', 'B2', 'C1'].index(x['level']))
bus_index = next(index for index, topic in enumerate(units) if topic['id'] == 'a1.bus-stop')
for index, topic_id in enumerate(('a1.bus-vocabulary', 'a1.public-transport-actions'), start=1):
    topic = next(unit for unit in units if unit['id'] == topic_id)
    units.remove(topic)
    units.insert(bus_index + index, topic)

if __name__ == '__main__':
    targets = {
        ROOT / 'Assets/_Project/Content/Resources/Curriculum.json': dict(units=units),
        ROOT / 'Assets/_Project/Localization/Resources/UIStrings.json': dict(
            entries=[dict(key=k, text=v) for k, v in strings.items()]),
    }
    for path, data in targets.items():
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(json.dumps(data, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    print(f'Built {len(units)} units / {sum(len(u["exercises"]) for u in units)} exercises; C1 is a preview.')
