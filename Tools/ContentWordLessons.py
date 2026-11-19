def extend_word_lessons(unit, choice, gap):
    fix = 'Исправьте английскую фразу целиком.'
    assemble = 'Соберите английскую фразу из слов.'

    def lesson(key, level, track, title, goal, rule, cards, sentence_tokens, review):
        topic = unit(key, level, title, goal, rule, cards)
        topic['track'] = track
        topic['exercises'][-1]['tokens'] = sentence_tokens.split()
        context, hint, explanation = review
        topic['exercises'][1]['reviewContext'] = dict(context=context, hint=hint, explanation=explanation)

    lesson('a2.word-choice', 'A2', 'words', 'Цена и ценность', 'Выбирать price, cost и worth по смыслу.',
           'Price — цена, которую назначил продавец. Cost — стоимость или расход, который несёте вы. Worth описывает ценность: be worth + сумма или действие с -ing.\n\nThe price is fifty pounds. — Цена пятьдесят фунтов.\nIt is worth repairing. — Это стоит починить.',
           [
               (choice, 'The shop displayed a ___ of fifty pounds on the tag.', 'price',
                ['price', 'worth', 'fee', 'budget'], 'Речь о цене на ярлыке.', 'Price — цена, указанная продавцом.'),
               (gap, 'These shoes will ___ me sixty pounds. (Use the verb meaning «стоить».)', 'cost', [],
                'Здесь после слова идут человек и сумма.', 'Cost me sixty pounds — стоить мне шестьдесят фунтов.'),
               (fix, 'It is worth repair. Replace only the form of repair.', 'It is worth repairing', [],
                'После worth действие выражают формой на -ing.',
                'Worth repairing — «стоит починить». После worth здесь нужна форма repairing.'),
               (assemble, 'Это стоит попробовать. Используйте worth и it.', ['It is worth trying', "It's worth trying"],
                [], 'Начните с it + be. Try поставьте в форму на -ing.',
                'It is worth trying. Worth + -ing означает, что действие имеет смысл.'),
           ], 'It is worth trying', ('The service will ___ us twenty pounds. (Use the verb meaning «стоить».)',
                                     'Нужен глагол перед us и суммой.',
                                     'Cost us twenty pounds — обойтись нам в двадцать фунтов.'))

    lesson('a2.phrasal-morning', 'A2', 'phrasal', 'Проснуться, встать и выйти', 'Различать wake up, get up и head out.',
           'Wake up — проснуться. Get up — встать с кровати. Head out — отправиться куда-то, выйти; звучит разговорно и нейтрально.\n\nI wake up at six, but I get up at seven.\nWe need to head out soon. — Нам скоро пора выходить.',
           [
               (choice, 'I hear the alarm at six, but I stay in bed. At six I ___.', 'wake up',
                ['wake up', 'get up', 'head out', 'come back'], 'Человек ещё лежит в кровати.',
                'Wake up — перестать спать. Get up — уже встать.'),
               (gap, 'I wake up at six, but I don’t ___ until seven. (Use two words for getting out of bed.)', 'get up',
                [], 'Сон уже закончился. Теперь речь о том, чтобы встать.', 'Get up — встать с кровати.'),
               (fix, 'I get up at six, but I stay in bed until seven. Replace get up with the phrase for waking.',
                'I wake up at six, but I stay in bed until seven', [], 'Событие в шесть — пробуждение.',
                'Wake up подходит, потому что человек остаётся в кровати.'),
               (assemble, 'Нам нужно скоро выходить. Используйте head out.', 'We need to head out soon', [],
                'После need используйте to + действие.', 'We need to head out soon — нам скоро пора отправляться.'),
           ], 'We need to head out soon',
           ('I’m awake, but I don’t want to ___ yet. (Use two words for leaving the bed.)',
            'Человек уже проснулся, но ещё не встал.', 'Get up в этой ситуации — встать с кровати.'))

    lesson('b1.word-deadlines', 'B1', 'words', 'Дедлайны: by, until и within',
           'Указывать срок, длительность и окно времени.',
           'By Friday — не позднее пятницы. Until Friday — действие продолжается до пятницы. Within two days — в течение двух дней от указанного момента.\n\nSend the file by Friday. — Пришлите до пятницы включительно.\nI’m away until Friday. — Меня не будет до пятницы.',
           [
               (choice, 'Finish no later than Friday: Send the draft ___ Friday.', 'by',
                ['until', 'by', 'within', 'during'], 'Нужна конечная дата для завершения действия.',
                'By задаёт срок: отправить можно и раньше.'),
               (gap,
                'You’ll receive a reply ___ two business days of your request. (Use one word for the time window.)',
                'within', [], 'Счёт идёт от момента получения запроса.',
                'Within two business days — в течение двух рабочих дней.'),
               (fix, 'Send the invoice until Friday. Replace until to mean no later than Friday.',
                'Send the invoice by Friday', [], 'Отправка — однократное действие к сроку.',
                'By Friday означает «не позднее пятницы». Until описывало бы действие, которое длится.'),
               (assemble, 'Мы будем здесь до полудня. Используйте until noon.',
                ['We will be here until noon', "We'll be here until noon"], [],
                'Перед here нужен be. Действие длится до полудня.',
                'Until noon относится к длительности пребывания, поэтому подходит здесь.'),
           ], 'We will be here until noon',
           ('Please respond ___ forty-eight hours of receiving the email. (Use one word for the time window.)',
            'Окно в 48 часов отсчитывается с получения письма.',
            'Within говорит, что ответ должен прийти в пределах этого времени.'))

    lesson('b1.phrasal-arrangements', 'B1', 'phrasal', 'Назначить, перенести или отменить',
           'Пользоваться set up, put off и call off.',
           'Set up a meeting — организовать встречу. Put off a meeting — отложить. Call off a meeting — отменить. У всех трёх местоимение ставится между глаголом и частицей: call it off.\n\nLet’s set up a call. — Давайте назначим звонок.\nWe had to call it off. — Нам пришлось отменить.',
           [
               (choice, 'There won’t be a meeting at all. We need to ___.', 'call it off',
                ['set it up', 'put it off', 'call it off', 'call off it'], 'Встречу полностью отменяют.',
                'Call it off — отменить. Put it off — перенести.'),
               (gap, 'We moved the meeting to Monday. We put it ___. (Use the particle for postponing.)', 'off', [],
                'После put it нужна частица глагола «отложить».',
                'Put it off — отложить встречу на более позднее время.'),
               (fix, 'Can we set up it for Friday? Move only the pronoun.', 'Can we set it up for Friday', [],
                'It стоит между set и up.', 'Set it up — организовать это. Местоимение ставят перед up.'),
               (assemble, 'Давайте отменим встречу. Используйте call off, поставьте частицу в конце.',
                ["Let's call the meeting off", 'Let us call the meeting off'], [], 'После let’s нужен глагол без to.',
                'Let’s call the meeting off — давайте отменим встречу.'),
           ], "Let's call the meeting off", ('We put the launch ___ until June. (Use the particle for postponing.)',
                                             'Запуск всё ещё состоится, только позже.',
                                             'Put the launch off — перенести запуск.'))

    lesson('b2.word-evidence', 'B2', 'words', 'Данные, доказательства и выводы',
           'Различать evidence, proof и conclusion.',
           'Evidence — сведения в пользу вывода, слово неисчисляемое. Proof — убедительное подтверждение. A conclusion — сам вывод. Можно сказать a piece of evidence и draw a conclusion.\n\nWe found no evidence of a leak.\nIt’s too early to draw a conclusion.',
           [
               (choice, 'The report gives some ___ in favour of the theory, but it doesn’t prove it.', 'evidence',
                ['evidence', 'proof', 'conclusion', 'policy'], 'Данные поддерживают теорию, но не доказывают её.',
                'Evidence может поддерживать вывод, не будучи окончательным proof.'),
               (gap, 'It is too early to ___ a conclusion. (Use the common verb starting with d.)', 'draw', [],
                'Нужно устойчивое сочетание «сделать вывод».',
                'Draw a conclusion — сделать вывод из имеющихся данных.'),
               (fix, 'We found an evidence of a leak. Replace an with one piece of.',
                'We found one piece of evidence of a leak', [], 'Evidence нельзя считать напрямую.',
                'One piece of evidence — одно свидетельство. An evidence в этом значении неверно.'),
               (assemble, 'Мы не нашли доказательств. Используйте found и no evidence.', 'We found no evidence', [],
                'No ставится перед неисчисляемым evidence.',
                'We found no evidence — подтверждающих данных мы не нашли.'),
           ], 'We found no evidence',
           ('We shouldn’t ___ a conclusion from one example. (Use the common verb starting with d.)',
            'Одного примера мало для вывода.',
            'Draw a conclusion from something — сделать вывод на основании чего-либо.'))

    lesson('b2.phrasal-followup', 'B2', 'phrasal', 'Разобраться и довести до конца',
           'Освоить look into, follow up on и carry out в рабочем контексте.',
           'Look into — разобраться в проблеме. Follow up on — вернуться к вопросу или проверить, что дальше. Carry out — выполнить задачу, проверку или исследование.\n\nI’ll follow up on the request tomorrow.\nThey carried out the safety checks.',
           [
               (choice, 'We already sent a request, but received no reply. I’ll ___ it.', 'follow up on',
                ['carry out', 'follow up on', 'call off', 'put away'], 'Нужно вернуться к уже отправленной просьбе.',
                'Follow up on a request — уточнить судьбу запроса или продолжить работу по нему.'),
               (gap, 'We need to carry ___ the safety checks. (Use the particle for performing a task.)', 'out', [],
                'Здесь речь о выполнении проверок.', 'Carry out the checks — выполнить проверки.'),
               (fix, 'I’ll follow it up on tomorrow. Move it after the full verb phrase.',
                ["I'll follow up on it tomorrow", 'I will follow up on it tomorrow'], [],
                'После follow up on ставится дополнение.', 'Follow up on it — вернуться к этому вопросу.'),
               (assemble, 'Мы выполним проверки. Используйте carry out.',
                ['We will carry out the checks', "We'll carry out the checks"], [],
                'После will нужен глагол в начальной форме.', 'We will carry out the checks — мы проведём проверки.'),
           ], 'We will carry out the checks',
           ('They carried ___ an independent review. (Use the particle for performing a task.)',
            'Речь о проведённой проверке.', 'Carry out a review — провести проверку или анализ.'))
