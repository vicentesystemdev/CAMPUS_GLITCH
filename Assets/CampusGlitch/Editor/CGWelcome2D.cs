#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace CampusGlitch.Editor {
[InitializeOnLoad]
public static class CGWelcome2D {
 const string Boot="Assets/CampusGlitch/Scenes/SC_Bootstrap.unity";
 static CGWelcome2D(){EditorApplication.delayCall+=OnReady;}
 static void OnReady(){
   if(EditorApplication.isPlayingOrWillChangePlaymode||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
   var active=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
   // Respect a developer's actively edited scene. Only replace the untouched Unity default.
   if(active.name=="Untitled" && !active.isDirty && AssetDatabase.LoadAssetAtPath<SceneAsset>(Boot)!=null){
     EditorSceneManager.OpenScene(Boot,OpenSceneMode.Single);
     Debug.Log("[Campus Glitch] Abierta portada 2D. Pulsa Play en Game.");
   }
   var sv=SceneView.lastActiveSceneView;
   if(sv!=null){sv.in2DMode=true;sv.orthographic=true;sv.Repaint();}
   // Respect which scene the developer opened before pressing Play.
   // Forcing the Bootstrap always prevented direct testing of level scenes.
   // The bootstrap is first in Build Settings instead.
 }
 [MenuItem("Campus Glitch/00 · Abrir portada 2D")]
 public static void OpenBoot(){EditorSceneManager.OpenScene(Boot,OpenSceneMode.Single);
   var sv=SceneView.lastActiveSceneView;if(sv!=null){sv.in2DMode=true;sv.Repaint();}}
}
}
#endif
