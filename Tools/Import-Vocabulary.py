import argparse
import csv
import json
import re
import xml.etree.ElementTree as ET
from collections import defaultdict
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
LEVELS = ('A1', 'A2', 'B1', 'B2', 'C1')
TEI = '{http://www.tei-c.org/ns/1.0}'
PARTS_OF_SPEECH = {
    'noun': 'n', 'verb': 'v', 'adjective': 'adj', 'adverb': 'adv', 'pronoun': 'pronoun',
    'preposition': 'preposition', 'conjunction': 'conjunction', 'interjection': 'interjection',
    'determiner': 'determiner', 'number': 'numeral', 'be-verb': 'v', 'do-verb': 'v', 'have-verb': 'v',
    'modal auxiliary': 'v', 'infinitive-to': 'particle',
}
COMMON_GLOSSES = {
    ('the', 'determiner'): 'артикль перед уже известным или конкретным предметом',
    ('a', 'determiner'): 'артикль перед одним ещё не указанным предметом',
    ('an', 'determiner'): 'форма a перед гласным звуком',
    ('be', 'be-verb'): 'быть, находиться',
    ('and', 'conjunction'): 'и',
    ('of', 'preposition'): 'из, о; также связывает предмет с владельцем или частью',
    ('to', 'infinitive-to'): 'частица перед начальной формой глагола',
    ('to', 'preposition'): 'к, в, до — направление или предел',
    ('in', 'preposition'): 'в, внутри; также перед месяцем или годом',
    ('have', 'have-verb'): 'иметь; также вспомогательный глагол',
    ('do', 'do-verb'): 'делать; также вспомогательный глагол',
    ('it', 'pronoun'): 'это; он, она или оно о предмете',
    ('house', 'noun'): 'дом, здание',
}


def rows(path):
    with path.open(encoding='utf-8-sig', newline='') as stream:
        yield from csv.DictReader(stream)


def translations(path, terms):
    results = defaultdict(list)
    for _, entry in ET.iterparse(path, events=('end',)):
        if entry.tag != TEI + 'entry':
            continue
        term = entry.findtext(f'{TEI}form/{TEI}orth', '').lower()
        if term in terms:
            part = entry.findtext(f'{TEI}gramGrp/{TEI}pos', '')
            values = results[term, part]
            for quote in entry.findall(f'.//{TEI}cit[@type="trans"]/{TEI}quote'):
                text = ' '.join(''.join(quote.itertext()).split())
                if 0 < len(text) <= 36 and re.search('[А-Яа-яЁё]', text) and text not in values:
                    values.append(text)
        entry.clear()
    return results


def build(source):
    ranks = {row['Lemma'].lower(): int(row['SFI Rank']) for row in rows(source / 'ngsl.csv')}
    entries = {}
    for filename in ('cefrj.csv', 'octanove.csv'):
        for row in rows(source / filename):
            level, pos = row['CEFR'].strip(), row['pos'].strip()
            if level not in LEVELS:
                continue
            term = re.split(r'\s*/\s*', row['headword'].strip(), maxsplit=1)[0]
            if not re.fullmatch(r"[A-Za-z]+(?:[-'][A-Za-z]+)*", term):
                continue
            key = (term.lower(), pos)
            if key not in entries or LEVELS.index(level) < LEVELS.index(entries[key]['level']):
                entries[key] = dict(term=term, pos=pos, level=level, rank=ranks.get(term.lower(), 0))

    glosses = translations(source / 'wikdict.tei', {term for term, _ in entries})
    for (term, pos), entry in entries.items():
        values = glosses.get((term, PARTS_OF_SPEECH.get(pos, '')), [])
        entry['meaning'] = COMMON_GLOSSES.get((term, pos), ', '.join(values[:3]))

    return sorted(entries.values(), key=lambda e: (LEVELS.index(e['level']), e['rank'] or 100_000,
                                                   e['term'].lower(), e['pos']))


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('source', type=Path, help='Directory containing the four source files named in docs/ASSETS.md')
    entries = build(parser.parse_args().source)
    path = ROOT / 'Assets/_Project/Content/Resources/VocabularyReference.json'
    contents = ',\n'.join('    ' + json.dumps(entry, ensure_ascii=False) for entry in entries)
    path.write_text('{\n  "entries": [\n' + contents + '\n  ]\n}\n', encoding='utf-8')
    print(
        f'Imported {len(entries)} reference entries; {sum(bool(e["meaning"]) for e in entries)} have Russian glosses.')
