#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
namespace CampusGlitch.Editor {
 public static class CG2DRenderSetup {
   const string Root="Assets/CampusGlitch/Settings";
   static Type Find(string full){
     Type t=Type.GetType(full+", Unity.RenderPipelines.Universal.Runtime",false);
     if(t!=null)return t;
     foreach(var a in AppDomain.CurrentDomain.GetAssemblies()){
       t=a.GetType(full,false);if(t!=null)return t;
     }
     return null;
   }
   [MenuItem("Campus Glitch/01 · Validar y configurar Universal 2D")]
   public static void Configure(){
     if(EditorApplication.isPlaying){Debug.LogWarning("Detén Play primero");return;}
     // A real Hub-created Universal 2D project ALREADY has a URP asset and Renderer2D.
     var existing=GraphicsSettings.defaultRenderPipeline;
     if(existing!=null){
       var so=new SerializedObject(existing);
       var prop=so.FindProperty("m_RendererDataList");
       if(prop!=null&&prop.arraySize>0&&prop.GetArrayElementAtIndex(0).objectReferenceValue!=null){
         var renderer=prop.GetArrayElementAtIndex(0).objectReferenceValue;
         if(renderer.GetType().Name.Contains("Renderer2D")){
           QualitySettings.renderPipeline=existing;
           Debug.Log("[Campus Glitch] Universal 2D detectado y activo: "+renderer.name);
           return;
         }
       }
     }
     Type rendererType=Find("UnityEngine.Rendering.Universal.Renderer2DData");
     Type urpType=Find("UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset");
     if(rendererType==null||urpType==null){
       Debug.LogWarning("[Campus Glitch] Unity todavía no ha cargado URP. Revisa Packages/manifest.json y espera que Package Manager termine de resolver com.unity.render-pipelines.universal 17.3.0. Si continúa, CREA un proyecto Universal 2D con Unity Hub e importa el contenido de Assets/CampusGlitch mediante Tools/IMPORTAR_EN_UNIVERSAL_2D.bat.");
       return;
     }
     Directory.CreateDirectory(Root);AssetDatabase.Refresh();
     const string renderPath=Root+"/Renderer2D.asset";
     const string pipelinePath=Root+"/CampusGlitchURP2D.asset";
     var rend=AssetDatabase.LoadAssetAtPath<ScriptableObject>(renderPath);
     if(rend==null){rend=ScriptableObject.CreateInstance(rendererType);AssetDatabase.CreateAsset(rend,renderPath);}
     var pipe=AssetDatabase.LoadAssetAtPath<ScriptableObject>(pipelinePath);
     if(pipe==null){pipe=ScriptableObject.CreateInstance(urpType);AssetDatabase.CreateAsset(pipe,pipelinePath);}
     var serialized=new SerializedObject(pipe);
     var list=serialized.FindProperty("m_RendererDataList");
     if(list==null){Debug.LogWarning("[Campus Glitch] Este URP no expone el renderer esperado; usa el proyecto Hub Universal 2D sin sobrescribir sus Settings.");return;}
     list.arraySize=1;list.GetArrayElementAtIndex(0).objectReferenceValue=rend;
     var selected=serialized.FindProperty("m_DefaultRendererIndex");if(selected!=null)selected.intValue=0;
     serialized.ApplyModifiedPropertiesWithoutUndo();
     GraphicsSettings.defaultRenderPipeline=pipe as RenderPipelineAsset;
     QualitySettings.renderPipeline=pipe as RenderPipelineAsset;
     AssetDatabase.SaveAssets();
     Debug.Log("[Campus Glitch] Renderer2D asignado. Abre SC_Bootstrap para jugar.");
   }
 }
}
#endif
