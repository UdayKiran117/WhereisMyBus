"""Static source package checks. These do not replace .NET/Android/MySQL builds."""
from pathlib import Path
import xml.etree.ElementTree as ET
import json,re
root=Path(__file__).resolve().parents[1]
count=0
for p in root.rglob('*'):
    if not p.is_file() or any(part in ('bin','obj','TestResults') for part in p.parts): continue
    if p.suffix in ('.csproj','.props','.targets','.xaml','.xml','.svg'):
        ET.parse(p);count+=1
    if p.suffix=='.json': json.loads(p.read_text(encoding='utf-8'));count+=1
for p in root.rglob('*.csproj'):
    for ref in ET.parse(p).getroot().iter('ProjectReference'):
        target=(p.parent/ref.attrib['Include']).resolve()
        assert target.is_file(),f'Missing project reference: {p}: {target}'
        if 'Client' in p.parts: assert 'Server' not in target.parts,'Client references server'
for area in ('Server','Client'):
    sln=root/area/f'WhereIsMyBus.{area}.sln'
    for path in re.findall(r'"([^"\n]+\.csproj)"',sln.read_text()):
        assert (sln.parent/path.replace('\\','/')).is_file(),path
sql=(root/'Server/database/002_CreateTables.sql').read_text()
assert len(re.findall(r'CREATE TABLE ',sql))==15
for p in root.rglob('*.cs'):
    if any(part in ('bin','obj') for part in p.parts): continue
    assert 'TODO' not in p.read_text(),f'TODO implementation: {p}'
print(f'PASS: {count} XML/JSON files, project references, solution paths, 15 tables and no TODO source stubs.')
