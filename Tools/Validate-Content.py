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
    assert len(units) == 22 and len(cards) == 88, 'Unexpected pilot content size'
    assert len({u['id'] for u in units}) == len(units), 'Duplicate unit ID'
    assert len({c['id'] for c in cards}) == len(cards), 'Duplicate exercise ID'
    assert {u['level'] for u in units} == {'A1', 'A2', 'B1', 'B2', 'C1'}
    for u in units:
        assert all(u.get(k, '').strip() for k in ('id', 'title', 'goal', 'rule'))
        assert '\n\n' in u['rule'], f'Missing examples: {u["id"]}'
        assert len(u['rule']) <= 750, f'Explanation too long: {u["id"]}'
        assert re.search('[А-Яа-я]', u['rule']), f'Missing Russian explanation: {u["id"]}'
        for c in u['exercises']:
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