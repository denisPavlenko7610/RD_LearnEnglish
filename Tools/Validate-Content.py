import json
import re
import unicodedata
import xml.etree.ElementTree as ET
from collections import Counter
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]


def normalize(value):
    return re.sub(r'\s+', ' ', unicodedata.normalize('NFC', value).strip()).rstrip('.!?').rstrip().lower()


def validate():
    data = json.loads((ROOT / 'Assets/_Project/Content/Resources/Curriculum.json').read_text(encoding='utf-8'))
    units = data['units']
    cards = [c for u in units for c in u['exercises']]
    assert Counter(u['level'] for u in units) == {'A1': 18, 'A2': 20, 'B1': 15, 'B2': 13, 'C1': 6}
    assert len(cards) == 344, 'Unexpected pilot content size'
    assert Counter(u['track'] for u in units if 'track' in u) == {'words': 22, 'phrasal': 12}
    assembly = [c for c in cards if 'tokens' in c]
    assert len(assembly) == 26
    for card in assembly:
        assert not card['options'] and 2 <= len(card['tokens']) <= 8
        assert normalize(' '.join(card['tokens'])) in {normalize(a) for a in card['answers']}
    dialogues = [c for c in cards if 'dialogue' in c]
    assert len(dialogues) == 28
    for card in dialogues:
        assert not card['options'] and len(card['dialogue'].split('\n\n')) >= 3
    assert len({u['id'] for u in units}) == len(units), 'Duplicate unit ID'
    assert len({c['id'] for c in cards}) == len(cards), 'Duplicate exercise ID'
    assert sum('reviewContext' in c for c in cards) == 161, 'Unexpected review context count'
    vocabulary = [(u['level'], c) for u in units for c in u['exercises'] if 'vocabularyTerm' in c]
    expected = {(level, kind): 5 for level in ('A1', 'A2', 'B1', 'B2', 'C1')
                for kind in ('words', 'expressions', 'phrasal')}
    expected.update({('A1', 'words'): 16, ('A1', 'expressions'): 15, ('A1', 'phrasal'): 10,
                     ('A2', 'words'): 15, ('A2', 'expressions'): 10, ('A2', 'phrasal'): 10})
    assert Counter((level, c['vocabularyKind']) for level, c in vocabulary) == expected
    for u in units:
        if 'modelPhrase' in u:
            assert 0 < len(u['modelPhrase'].strip()) <= 80
    for c in cards:
        if 'speechText' in c:
            assert 0 < len(c['speechText'].strip()) <= 80
            assert 'reviewContext' not in c
    assert len({(level, c['vocabularyTerm']) for level, c in vocabulary}) == len(vocabulary)
    for _, card in vocabulary:
        assert card['answers'][0] == card['vocabularyTerm'] and not card['options']
        assert card['context'].count('___') == 1 and card['vocabularyMeaning']
    reference = json.loads((ROOT / 'Assets/_Project/Content/Resources/VocabularyReference.json')
                           .read_text(encoding='utf-8'))['entries']
    assert len(reference) == 8693
    assert {r['level'] for r in reference} == {'A1', 'A2', 'B1', 'B2', 'C1'}
    assert len({(r['term'].lower(), r['pos']) for r in reference}) == len(reference)
    assert all(r['term'] and r['rank'] >= 0 for r in reference)
    for u in units:
        assert all(u.get(k, '').strip() for k in ('id', 'title', 'goal', 'rule'))
        assert '\n\n' in u['rule'], f'Missing examples: {u["id"]}'
        assert len(u['rule']) <= 750, f'Explanation too long: {u["id"]}'
        assert re.search('[А-Яа-я]', u['rule']), f'Missing Russian explanation: {u["id"]}'
        for c in u['exercises']:
            if 'reviewContext' in c:
                review = c['reviewContext']
                assert all(review.get(k, '').strip() for k in ('context', 'hint', 'explanation'))
                assert review['context'] != c['context'] and review['context'].count('___') == 1
            assert all(c.get(k, '').strip() for k in ('id', 'prompt', 'context', 'hint', 'explanation'))
            assert c['answers'] and all(a.strip() for a in c['answers'])
            assert all(normalize(a) for a in c['answers'])
            assert len({normalize(o) for o in c['options']}) == len(c['options'])
            if c['options']:
                assert 2 <= len(c['options']) <= 4
                assert sum(normalize(o) in {normalize(a) for a in c['answers']} for o in c['options']) == 1
    table = json.loads((ROOT / 'Assets/_Project/Localization/Resources/UIStrings.json').read_text(encoding='utf-8'))[
        'entries']
    strings = {e['key']: e['text'] for e in table}
    assert len(strings) == len(table), 'Duplicate localization key'
    assert all(v.strip() for v in strings.values())
    assert all('vocabularyPos_' + r['pos'] in strings for r in reference)
    for path in (ROOT / 'Assets/_Project/Runtime/UI').glob('*.uxml'):
        tree = ET.parse(path)
        names = [e.attrib['name'] for e in tree.iter() if 'name' in e.attrib]
        assert len(names) == len(set(names)), f'Duplicate UI name: {path}'
        assert not any('style' in e.attrib for e in tree.iter()), 'Inline styles are forbidden'
    for path in (ROOT / 'Assets/_Project').rglob('*'):
        if path.suffix != '.meta':
            assert Path(str(path) + '.meta').exists(), f'Missing meta: {path}'
    guids = []
    for path in (ROOT / 'Assets/_Project').rglob('*.meta'):
        match = re.search(r'^guid: ([0-9a-f]{32})$', path.read_text(encoding='utf-8'), re.M)
        assert match, f'Missing GUID: {path}'
        assert Path(str(path)[:-5]).exists(), f'Orphan meta: {path}'
        guids.append(match.group(1))
    assert len(set(guids)) == len(guids), 'Duplicate project GUID'
    build_settings = (ROOT / 'ProjectSettings/EditorBuildSettings.asset').read_text(encoding='utf-8')
    for scene in re.findall(r'^\s+path: (.+)$', build_settings, re.M):
        assert (ROOT / scene).is_file(), f'Unknown build scene: {scene}'
    print(
        'PASS: content, original IDs, answers, concise Russian explanations, localization, UXML, project metadata and build scenes')
    print('Pilot only:', dict(Counter(u['level'] for u in units)), f'/ {len(cards)} exercises')


if __name__ == '__main__':
    validate()
