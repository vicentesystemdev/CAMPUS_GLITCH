#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace CampusGlitch.Editor {
public static class CGAnimationBuilder {
 const string Root="Assets/CampusGlitch/Resources/CG/AnimationControllers";
 [MenuItem("Campus Glitch/Crear clips y Animator del Lobo")]
 public static void Create(){
   if(EditorApplication.isPlaying)throw new InvalidOperationException("Sal de Play primero.");
   if(!AssetDatabase.IsValidFolder("Assets/CampusGlitch/Resources/CG/AnimationControllers")){
     Directory.CreateDirectory(Root);AssetDatabase.Refresh();
   }
   CreateWolf();
   CreateWolfOriginal();
   CreateAssistant();
   AssetDatabase.SaveAssets();AssetDatabase.Refresh();
   Debug.Log("[Campus Glitch] Clips .anim nativos y AnimatorControllers generados.");
 }
 static AnimationClip BuildClip(string name, string folder, int a,int b,float framesPerSecond){
   var frames=new List<ObjectReferenceKeyframe>();
   for(int i=a;i<=b;i++){
     Sprite sp=Resources.Load<Sprite>("CG/"+folder+i.ToString("00"));
     if(sp==null)throw new InvalidOperationException("Frame obligatorio no encontrado: "+folder+i.ToString("00"));
     frames.Add(new ObjectReferenceKeyframe{time=(i-a)/framesPerSecond,value=sp});
   }
   frames.Add(new ObjectReferenceKeyframe{time=(b-a+1)/framesPerSecond,value=frames[frames.Count-1].value});
   AnimationClip clip=new AnimationClip();clip.name=name;clip.frameRate=framesPerSecond;
   AnimationUtility.SetObjectReferenceCurve(clip,new EditorCurveBinding{path="",type=typeof(SpriteRenderer),propertyName="m_Sprite"},frames.ToArray());
   var settings=AnimationUtility.GetAnimationClipSettings(clip);
   settings.loopTime=name!="Hurt" && name!="Attack";
   AnimationUtility.SetAnimationClipSettings(clip,settings);
   string path=Root+"/"+name+".anim";
   var old=AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
   if(old!=null)AssetDatabase.DeleteAsset(path);
   AssetDatabase.CreateAsset(clip,path);
   return clip;
 }
 static void CreateWolf(){
   string path=Root+"/LoboBipedo.controller";
   var prev=AssetDatabase.LoadAssetAtPath<AnimatorController>(path);if(prev!=null)AssetDatabase.DeleteAsset(path);
   var controller=AnimatorController.CreateAnimatorControllerAtPath(path);
   var states=new[]{"Idle","Walk","Run","Jump","Fall","Attack","Hurt"};
   var ranges=new int[,] {{0,1},{2,5},{6,9},{10,12},{13,14},{15,18},{19,20}};
   for(int i=0;i<states.Length;i++){
     AnimationClip clip=BuildClip("Lobo_"+states[i],"Playable/Red_",ranges[i,0],ranges[i,1],i==2?12:8);
     var state=controller.layers[0].stateMachine.AddState(states[i]);state.motion=clip;
     if(i==0)controller.layers[0].stateMachine.defaultState=state;
   }
 }
 static void CreateWolfOriginal(){
   var names=new[]{"Idle","Walk","Run","Jump","Power"};
   var starts=new int[]{0,2,6,10,13};
   var ends=new int[]{1,5,9,12,16};
   foreach(bool purple in new[]{false,true}){
     string path=Root+"/LoboOficial"+(purple?"Morado":"Rojo")+".controller";
     var current=AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
     if(current!=null)AssetDatabase.DeleteAsset(path);
     var controller=AnimatorController.CreateAnimatorControllerAtPath(path);
     for(int i=0;i<names.Length;i++){
       var clip=BuildClip("LoboOficial"+(purple?"Morado":"Rojo")+"_"+names[i],
          "Playable/"+(purple?"ClassicPurple_":"Classic_"),starts[i],ends[i],i==2?13:9);
       var state=controller.layers[0].stateMachine.AddState(names[i]);state.motion=clip;
       if(i==0)controller.layers[0].stateMachine.defaultState=state;
     }
   }
 }
 static void CreateAssistant(){
   var kinds=new[]{"idle","talk","alert"};
   foreach(string mode in kinds)BuildClip("Asistente_"+mode,"Playable/Assistant_"+mode+"_",0,2,6);
 }
}
}
#endif
