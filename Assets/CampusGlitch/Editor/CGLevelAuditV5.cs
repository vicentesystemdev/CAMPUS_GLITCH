#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace CampusGlitch.Editor {
public static class CGLevelAuditV5 {
 [MenuItem("Campus Glitch/02 · Auditar arte y 18 escenas")]
 public static void Run(){
   int errors=0; int count=0;
   for(int l=0;l<10;l++)for(int block=0;block<6;block++){
     string p="CG/Themes/Level"+l.ToString("00")+"_Panel"+block;
     if(Resources.Load<Sprite>(p)==null){Debug.LogError("Falta panel 2D: "+p);errors++;}else count++;
   }
   for(int k=0;k<21;k++)for(int variant=0;variant<2;variant++){
     string p="CG/Playable/"+(variant==0?"Red_":"Purple_")+k.ToString("00");
     if(Resources.Load<Sprite>(p)==null){Debug.LogError("Falta frame del lobo: "+p);errors++;}
   }
   string[] ui={"SC_Bootstrap","SC_MainMenu","SC_LevelMap","SC_Customization","SC_Settings","SC_Controls","SC_Credits","SC_Help"};
   foreach(string n in ui)if(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/CampusGlitch/Scenes/"+n+".unity")==null){Debug.LogError("Falta UI: "+n);errors++;}
   for(int i=1;i<=9;i++)if(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/CampusGlitch/Scenes/SC_Level"+i.ToString("00")+".unity")==null){errors++;Debug.LogError("Falta nivel "+i);}
   if(AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/CampusGlitch/Scenes/SC_Extra_Subsuelo.unity")==null)errors++;
   if(Camera.main!=null&&!Camera.main.orthographic){Debug.LogError("Main Camera no es ortografica");errors++;}
   Debug.Log("[CAMPUS GLITCH V5] "+count+" paneles tematicos comprobados; "+errors+" problemas de integridad. No sustituye PlayMode ni tests físicos.");
 }
}
}
#endif
