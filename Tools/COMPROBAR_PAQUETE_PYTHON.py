from pathlib import Path
import json
p=Path(__file__).resolve().parents[1]
assert (p/'ProjectSettings/ProjectVersion.txt').read_text().find('6000.3.16f1')!=-1
m=json.loads((p/'Packages/manifest.json').read_text())
assert m['dependencies']['com.unity.render-pipelines.universal']=='17.3.0'
assert m['dependencies']['com.unity.2d.aseprite']=='3.0.2'
assert (p/'Assets/CampusGlitch/Scripts/CGGame.cs').exists()
assert (p/'Assets/CampusGlitch/Scenes/SC_Bootstrap.unity').exists()
for n in range(1,10):assert (p/f'Assets/CampusGlitch/Scenes/SC_Level{n:02}.unity').exists()
assert (p/'Assets/CampusGlitch/Scenes/SC_Extra_Subsuelo.unity').exists()
for i in range(17):assert (p/f'Assets/CampusGlitch/Resources/CG/Playable/Classic_{i:02}.png').exists()
print('Integridad básica OK. NO certifica compilación ni juego ejecutado.')
