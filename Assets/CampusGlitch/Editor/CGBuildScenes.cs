#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Reporting;

namespace CampusGlitch.Editor {
 public static class CGBuildScenes {
   private const string ScenesRoot = "Assets/CampusGlitch/Scenes";
   private const string PreviewName = "CG_EDITOR_PREVIEW_2D";
   private static readonly string[] FloorNames = {
     "Floor_ExteriorStone","Floor_GrayVinyl","Floor_Technical","Floor_WhiteClinic","Floor_Technical",
     "Floor_GrayVinyl","Floor_WoodGray","Floor_GreenTurf","Floor_WhiteClinic","Floor_WhiteClinic"
   };

   [MenuItem("Campus Glitch/Reparar escenas y mostrar en 2D")]
   public static void GenerateAll() {
     if(EditorApplication.isPlaying || EditorApplication.isCompiling)
       throw new InvalidOperationException("Detén Play y espera la compilación antes de reparar las escenas.");
     // Template Universal 2D remains authoritative; do not overwrite its Renderer2D.
     // A wrong render pipeline must be fixed in the imported Universal 2D project itself.
     Directory.CreateDirectory(ScenesRoot);
     CGAnimationBuilder.Create();
     var names=new List<string>{"SC_Bootstrap","SC_MainMenu","SC_LevelMap","SC_Customization","SC_Settings","SC_Controls","SC_Credits","SC_Help"};
     for(int i=0;i<10;i++) names.Add(CGLevels.Scene(i));
     var included=new List<EditorBuildSettingsScene>();
     foreach(string name in names) {
       string path=ScenesRoot+"/"+name+".unity";
       Scene scene=File.Exists(path)?EditorSceneManager.OpenScene(path,OpenSceneMode.Single)
                                    :EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
       int level=name=="SC_Bootstrap"?-1:FindIndex(name);
       CGGame controller=EnsureController(level);
       controller.initialPage = name=="SC_Bootstrap"?"Splash":
         name=="SC_MainMenu"?"Main":name=="SC_LevelMap"?"Map":
         name=="SC_Customization"?"Customization":name=="SC_Settings"?"Settings":
         name=="SC_Controls"?"Controls":name=="SC_Credits"?"Credits":
         name=="SC_Help"?"Help":"Splash";
       EnsureCamera(level);
       CGCamera2DRuntime.EnsureGlobalLight();
       RebuildScenePreview(level,controller);
       EditorSceneManager.MarkSceneDirty(scene);
       if(!EditorSceneManager.SaveScene(scene,path))
         throw new IOException("No se pudo guardar la escena: "+path);
       included.Add(new EditorBuildSettingsScene(path,true));
       Debug.Log("[Campus Glitch 2D] Reparada: "+name+" | Cámara ortográfica + arte lateral");
     }
     EditorBuildSettings.scenes=included.ToArray();
     AssetDatabase.SaveAssets();
     AssetDatabase.Refresh();
     EditorSceneManager.OpenScene(ScenesRoot+"/SC_Level01.unity",OpenSceneMode.Single);
     SceneView sceneView=SceneView.lastActiveSceneView;
     if(sceneView!=null) {
       sceneView.in2DMode=true;
       sceneView.LookAtDirect(new Vector3(8f,1.6f,0f),Quaternion.identity,11f);
       sceneView.Repaint();
     }
     Debug.Log("Campus Glitch: 18 escenas listas; 10 escenarios 2D y 8 pantallas. Pulsa Play en Game.");
   }
   static int FindIndex(string name) {
     for(int i=0;i<10;i++) if(CGLevels.Scene(i)==name)return i;
     return -1;
   }
   static CGGame EnsureController(int level) {
     CGGame existing=UnityEngine.Object.FindFirstObjectByType<CGGame>();
     if(existing==null)existing=new GameObject("CampusGlitch_"+(level<0?"Bootstrap":"Level"+level)).AddComponent<CGGame>();
     existing.sceneLevel=level;
     return existing;
   }
   static Camera EnsureCamera(int level) {
     Camera cam=Camera.main;
     if(cam==null)cam=UnityEngine.Object.FindFirstObjectByType<Camera>();
     if(cam==null) {
       GameObject cameraGo=new GameObject("Main Camera 2D");
       cam=cameraGo.AddComponent<Camera>();
     }
     cam=CGCamera2DRuntime.ConfigureCamera(cam);
     cam.tag="MainCamera";
     cam.orthographic=true;
     cam.orthographicSize=5.7f;
     cam.backgroundColor=new Color(.035f,.075f,.14f,1);
     cam.clearFlags=CameraClearFlags.SolidColor;
     cam.transform.rotation=Quaternion.identity;
     cam.transform.position=new Vector3(level<0?0:8f,level<0?1f:2.1f,-10f);
     AudioListener[] listeners=UnityEngine.Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
     if(listeners.Length==0)cam.gameObject.AddComponent<AudioListener>();
     else for(int i=1;i<listeners.Length;i++)listeners[i].enabled=false;
     return cam;
   }
   static void AddSprite(Transform parent,string name, string asset,float x,float y,float h,int order) {
     Sprite s=Resources.Load<Sprite>("CG/"+asset);
     if(s==null) {Debug.LogWarning("Sprite no disponible para vista previa: "+asset);return;}
     GameObject o=new GameObject(name);
     o.transform.SetParent(parent,false);
     o.transform.localPosition=new Vector3(x,y,0);
     SpriteRenderer r=o.AddComponent<SpriteRenderer>();
     r.sprite=s;r.sortingOrder=order;
     float scale=h/Mathf.Max(.01f,s.bounds.size.y);
     o.transform.localScale=Vector3.one*scale;
   }
   static void RebuildScenePreview(int level,CGGame controller) {
     GameObject old=GameObject.Find(PreviewName);
     if(old!=null)UnityEngine.Object.DestroyImmediate(old);
     GameObject preview=new GameObject(PreviewName);
     Transform root=preview.transform;
     int n=level<0?0:level;
     string bg="Backgrounds/Background_"+n.ToString("00");
     AddSprite(root,"PAISAJE_LA_PAZ_2D",bg,level<0?0:8,2f,11,-100);
     if(level<0) {
       AddSprite(root,"LOBO_GUARDIAN_OFICIAL","Playable/Classic_00",-2f,-.2f,3.3f,25);
       AddSprite(root,"ASISTENTE_DIGITAL","Playable/Assistant_idle_00",.4f,.5f,1.5f,26);
       return;
     }
     // The editor displays the SAME modular playable composition as runtime.
     // It is removed by CGGame.Start before the actual game scene is constructed.
     CGStore.Load();
     var previewWorld=preview.AddComponent<CGWorld>();
     previewWorld.Setup(controller,level);
     var spawn=previewWorld.hero!=null?previewWorld.hero.transform.position:new Vector3(2,0,0);
     Camera cam=Camera.main;
     if(cam!=null)cam.transform.position=new Vector3(spawn.x+7,2.2f,-10);
   }
   [MenuItem("Campus Glitch/Mostrar Scene View en 2D")]
   static void Activate2D(){
     SceneView sv=SceneView.lastActiveSceneView;
     if(sv!=null) {sv.in2DMode=true;sv.LookAtDirect(new Vector3(8f,1f,0f),Quaternion.identity,11f);sv.Repaint();}
   }
   [MenuItem("Campus Glitch/Crear build Windows x64")]
   public static void BuildWindows() {
     if(EditorBuildSettings.scenes==null || EditorBuildSettings.scenes.Length<18) GenerateAll();
     string root=Directory.GetCurrentDirectory();
     string buildDir=Path.Combine(root,"Build_Windows");Directory.CreateDirectory(buildDir);
     string[] scenePaths=Array.ConvertAll(EditorBuildSettings.scenes,s=>s.path);
     var options=new BuildPlayerOptions{
       scenes=scenePaths,
       locationPathName=Path.Combine(buildDir,"CampusGlitch.exe"),
       target=BuildTarget.StandaloneWindows64,
       options=BuildOptions.None
     };
     BuildReport report=BuildPipeline.BuildPlayer(options);
     Debug.Log("CampusGlitch Windows x64: "+report.summary.result+"; errores "+report.summary.totalErrors);
     if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build falló. Mira Editor.log");
   }
 }
 public class CGPixelImporter:AssetPostprocessor {
   void OnPreprocessTexture() {
     if(!assetPath.StartsWith("Assets/CampusGlitch/Resources/CG/",StringComparison.Ordinal) || !assetPath.EndsWith(".png",StringComparison.OrdinalIgnoreCase))return;
     TextureImporter importer=(TextureImporter)assetImporter;
     importer.textureType=TextureImporterType.Sprite;
     importer.spriteImportMode=SpriteImportMode.Single;
     importer.spritePixelsPerUnit=32;
     importer.filterMode=FilterMode.Point;
     importer.mipmapEnabled=false;
     importer.textureCompression=TextureImporterCompression.Uncompressed;
     importer.alphaIsTransparency=true;
   }
 }
}
#endif
