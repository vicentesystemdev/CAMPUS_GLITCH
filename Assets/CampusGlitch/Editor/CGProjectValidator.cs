#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace CampusGlitch.Editor {
public static class CGProjectValidator {
 [MenuItem("Campus Glitch/Validar proyecto 2D y arte oficial")]
 public static void Run(){
   var errors=new List<string>();
   if(Application.unityVersion!="6000.3.16f1")errors.Add("Se requiere Unity 6000.3.16f1; encontrado: "+Application.unityVersion);
   var scenes=EditorBuildSettings.scenes;
   if(scenes==null||scenes.Length!=18)errors.Add("Build Settings requiere 18 escenas; encontradas "+(scenes==null?0:scenes.Length));
   if(scenes!=null)foreach(var level in scenes){
     if(!level.enabled||!File.Exists(level.path))errors.Add("Escena ausente o deshabilitada: "+level.path);
   }
   for(int i=0;i<21;i++){
     Validate("CG/Playable/Red_"+i.ToString("00"),errors);
     Validate("CG/Playable/Purple_"+i.ToString("00"),errors);
   }
   foreach(string mode in new[]{"idle","talk","alert"})for(int i=0;i<3;i++)Validate("CG/Playable/Assistant_"+mode+"_"+i.ToString("00"),errors);
   Validate("CG/UI/MenuBackdrop",errors);
   Validate("CG/Environment/L00_Window",errors);
   Validate("CG/Environment/L04_Door",errors);
   for(int k=0;k<10;k++){
      var spec=CGLevels.All[k];
      foreach(string prop in spec.props){
        if(Resources.Load<Sprite>("CG/01_Nuevos_Modulares/"+spec.key+"/"+prop)==null)errors.Add("Falta prop N"+k+": "+prop);
      }
   }
   foreach(string name in new[]{"Checkpoint_Active","Terminal_Screen","Portal_Transition"})Validate("CG/Effects/"+name+"_00",errors);
   if(Resources.Load<RuntimeAnimatorController>("CG/AnimationControllers/LoboBipedo")==null)
       errors.Add("Falta AnimatorController del lobo: ejecuta Campus Glitch > Crear clips y Animator del Lobo");
   if(errors.Count>0){foreach(var error in errors)Debug.LogError("[CG VALIDACIÓN] "+error);throw new InvalidOperationException(errors.Count+" problemas detectados. No publiques en GitHub.");}
   Debug.Log("[CG VALIDACIÓN] OK: 18 escenas, sprites rojo/morado (42), Asistente (9), ambiente, prefabs de recursos, Animator nativo.");
 }
 static void Validate(string path,List<string> errors){if(Resources.Load<Sprite>(path)==null && Resources.Load<Texture2D>(path)==null)errors.Add("Resource ausente: "+path);}
}
}
#endif
