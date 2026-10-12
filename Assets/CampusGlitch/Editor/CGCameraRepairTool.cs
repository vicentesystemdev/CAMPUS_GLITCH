#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace CampusGlitch.Editor
{
    public static class CGCameraRepairTool
    {
        private const string SceneFolder = "Assets/CampusGlitch/Scenes";
        private static readonly string[] Required = {
            "SC_Bootstrap", "SC_MainMenu", "SC_LevelMap", "SC_Customization",
            "SC_Settings", "SC_Controls", "SC_Help", "SC_Credits",
            "SC_Level01", "SC_Level02", "SC_Level03", "SC_Level04", "SC_Level05",
            "SC_Level06", "SC_Level07", "SC_Level08", "SC_Level09", "SC_Extra_Subsuelo"
        };

        [MenuItem("Campus Glitch/CAMARA 2D/01 - Reparar TODAS las escenas y luces")]
        public static void RepairAll()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Campus Glitch", "Deten Play y espera a que Unity compile antes de reparar.", "Cerrar");
                return;
            }
            var current = SceneManager.GetActiveScene();
            if (!Application.isBatchMode && current.isDirty && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            string oldPath = current.path;
            var log = new StringBuilder();
            bool rpOkay = EnsureURP2D(log);
            int fixedScenes = 0;
            foreach (string sceneName in Required)
            {
                string scenePath = SceneFolder + "/" + sceneName + ".unity";
                if (!File.Exists(scenePath)) { log.AppendLine("FALTA: " + scenePath); continue; }
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                Camera cam = RepairCurrentScene(false, log);
                if (cam == null) { log.AppendLine("ERROR: sin camara en " + sceneName); continue; }
                EditorSceneManager.MarkSceneDirty(scene);
                if (EditorSceneManager.SaveScene(scene)) fixedScenes++;
                else log.AppendLine("ERROR guardando " + sceneName);
            }
            string dest = !string.IsNullOrEmpty(oldPath) && File.Exists(oldPath)
                ? oldPath : SceneFolder + "/SC_Level01.unity";
            EditorSceneManager.OpenScene(dest, OpenSceneMode.Single);
            SceneView view = SceneView.lastActiveSceneView;
            if (view != null) { view.in2DMode = true; view.orthographic = true; view.Repaint(); }
            log.Insert(0, "CAMERA 2D CAMPUS GLITCH - UNITY " + Application.unityVersion + "\n" +
                "Escenas reparadas: " + fixedScenes + " de " + Required.Length + "\n" +
                "Pipeline Universal 2D: " + (rpOkay ? "SI" : "NO / REVISA DIAGNOSTICO") + "\n\n");
            SaveReport(log.ToString());
            Debug.Log("[Campus Glitch] Reparadas " + fixedScenes + " escenas. Revisa Docs/CAMARA_2D_DIAGNOSTICO.txt");
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Camara 2D Campus Glitch",
                "Reparadas " + fixedScenes + "/18 escenas.\n" +
                (rpOkay ? "URP Renderer2D detectado." : "No se encontro Renderer2D valido; mira el reporte.") +
                "\nAbre la pestana GAME y pulsa Play.", "Entendido");
        }

        [MenuItem("Campus Glitch/CAMARA 2D/02 - Reparar escena ABIERTA")]
        public static void RepairOpen()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) { Debug.LogWarning("Deten Play primero."); return; }
            var log = new StringBuilder();
            EnsureURP2D(log);
            Camera cam = RepairCurrentScene(true, log);
            if (cam != null) EditorSceneManager.SaveOpenScenes();
            SaveReport(log.ToString());
            Debug.Log("[Campus Glitch] Camara y Global Light 2D revisadas en la escena abierta.");
        }

        private static Camera RepairCurrentScene(bool focus, StringBuilder log)
        {
            Camera[] all = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            Camera camera = Camera.main;
            if (camera == null && all.Length > 0) camera = all[0];
            camera = CGCamera2DRuntime.ConfigureCamera(camera);
            foreach (Camera c in all)
            {
                if (c != camera && c.enabled && c.gameObject.activeInHierarchy)
                {
                    c.enabled = false;
                    log.AppendLine("Camara duplicada desactivada: " + c.name);
                }
            }
            Component light = CGCamera2DRuntime.EnsureGlobalLight();
            int sprites = UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None).Length;
            log.AppendLine(SceneManager.GetActiveScene().name + " | camara=" + camera.name +
                " ortografica=" + camera.orthographic +
                " cullingMask=" + camera.cullingMask +
                " position=" + camera.transform.position +
                " SpriteRenderers=" + sprites +
                " GlobalLight2D=" + (light != null));
            if (focus)
            {
                var view = SceneView.lastActiveSceneView;
                if (view != null) { view.in2DMode = true; view.orthographic = true; view.LookAt(camera.transform.position + Vector3.forward * 10f); view.Repaint(); }
            }
            return camera;
        }

        private static bool Is2DAsset(RenderPipelineAsset pipeline, out string rendererInfo)
        {
            rendererInfo = "No hay renderizador";
            if (pipeline == null) return false;
            var so = new SerializedObject(pipeline);
            SerializedProperty list = so.FindProperty("m_RendererDataList");
            if (list == null || list.arraySize == 0) return false;
            for (int i = 0; i < list.arraySize; i++)
            {
                var renderer = list.GetArrayElementAtIndex(i).objectReferenceValue;
                if (renderer == null) continue;
                rendererInfo = renderer.name + " (" + renderer.GetType().Name + ")";
                if (renderer.GetType().Name.Contains("Renderer2D")) return i == 0;
            }
            return false;
        }

        private static bool EnsureURP2D(StringBuilder log)
        {
            var current = QualitySettings.renderPipeline != null
                ? QualitySettings.renderPipeline : GraphicsSettings.defaultRenderPipeline;
            string info;
            if (Is2DAsset(current, out info))
            {
                log.AppendLine("Pipeline 2D actual: " + current.name + " / " + info);
                GraphicsSettings.defaultRenderPipeline = current;
                ApplyQualityPipeline(current);
                return true;
            }
            log.AppendLine("Renderizador actual NO es 2D: " + info);
            foreach (string guid in AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                RenderPipelineAsset candidate = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(path);
                if (Is2DAsset(candidate, out info))
                {
                    GraphicsSettings.defaultRenderPipeline = candidate;
                    ApplyQualityPipeline(candidate);
                    log.AppendLine("Pipeline 2D asignado: " + path + " / " + info);
                    Debug.Log("[Campus Glitch] URP 2D activado: " + path);
                    return true;
                }
            }
            log.AppendLine("NO SE ENCONTRO UN RENDERER2D. Abre un proyecto creado con Universal 2D en Unity Hub y ejecuta de nuevo la reparacion.");
            Debug.LogError("[Campus Glitch] No hay UniversalRenderer2D valido. La iluminacion podria verse negra.");
            return false;
        }

        private static void ApplyQualityPipeline(RenderPipelineAsset pipeline)
        {
            int original = QualitySettings.GetQualityLevel();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = pipeline;
            }
            QualitySettings.SetQualityLevel(original, false);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Campus Glitch/CAMARA 2D/03 - Diagnosticar SIN modificar escenas")]
        public static void Diagnose()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Unity: " + Application.unityVersion);
            sb.AppendLine("Escena: " + SceneManager.GetActiveScene().name);
            sb.AppendLine("Graphics pipeline: " + (GraphicsSettings.defaultRenderPipeline != null ? GraphicsSettings.defaultRenderPipeline.name : "null"));
            sb.AppendLine("Quality pipeline: " + (QualitySettings.renderPipeline != null ? QualitySettings.renderPipeline.name : "null"));
            foreach (Camera c in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
            {
                sb.AppendLine("CAM: " + c.name + " tag=" + c.tag + " on=" + c.enabled +
                    " ortho=" + c.orthographic + " size=" + c.orthographicSize +
                    " clear=" + c.clearFlags + " mask=" + c.cullingMask +
                    " display=" + c.targetDisplay + " z=" + c.transform.position.z +
                    " targetTexture=" + (c.targetTexture != null));
            }
            int sprites = UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None).Length;
            Type lightType = CGCamera2DRuntime.GetLight2DType();
            int lights = lightType == null ? 0 : UnityEngine.Object.FindObjectsByType(lightType, FindObjectsSortMode.None).Length;
            sb.AppendLine("SpriteRenderers: " + sprites + " ; Light2D: " + lights);
            string info;
            sb.AppendLine("Pipeline2D: " + Is2DAsset(QualitySettings.renderPipeline != null ? QualitySettings.renderPipeline : GraphicsSettings.defaultRenderPipeline, out info));
            sb.AppendLine("Renderer: " + info);
            SaveReport(sb.ToString());
            Debug.Log(sb.ToString());
            EditorUtility.DisplayDialog("Diagnostico camara", "Reporte escrito en Docs/CAMARA_2D_DIAGNOSTICO.txt. Tambien se muestra en Console.", "OK");
        }

        [MenuItem("Campus Glitch/CAMARA 2D/04 - Insertar patron visible de PRUEBA")]
        public static void AddVisibleTest()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            Camera cam = Camera.main;
            if (cam == null) { Debug.LogError("No se encontro Main Camera. Primero usa 02 - Reparar escena abierta."); return; }
            GameObject before = GameObject.Find("CG_CAMARA_PRUEBA_ELIMINAR");
            if (before != null) UnityEngine.Object.DestroyImmediate(before);
            Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            for (int y = 0; y < 16; y++) for (int x = 0; x < 16; x++)
                texture.SetPixel(x, y, (x + y) % 2 == 0 ? Color.magenta : Color.yellow);
            texture.Apply();
            string path = "Assets/CampusGlitch/CG_PRUEBA_CAMARA.png";
            File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp != null)
            {
                imp.textureType = TextureImporterType.Sprite;
                imp.spritePixelsPerUnit = 16f;
                imp.filterMode = FilterMode.Point;
                imp.mipmapEnabled = false;
                imp.SaveAndReimport();
            }
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            GameObject go = new GameObject("CG_CAMARA_PRUEBA_ELIMINAR");
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 30000;
            Shader unlit = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (unlit == null) unlit = Shader.Find("Sprites/Default");
            if (unlit != null)
            {
                const string materialPath = "Assets/CampusGlitch/CG_PRUEBA_CAMARA_UNLIT.mat";
                Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (material == null)
                {
                    material = new Material(unlit);
                    AssetDatabase.CreateAsset(material, materialPath);
                }
                renderer.sharedMaterial = material;
            }
            go.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
            go.transform.localScale = new Vector3(2f, 2f, 1f);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("[Campus Glitch] Patron magenta-amarillo frente a la camara. Se debe ver en la pestaña Game al pulsar Play. Luego usa 05 para retirarlo.");
        }

        [MenuItem("Campus Glitch/CAMARA 2D/05 - Retirar patron de prueba")]
        public static void RemoveVisibleTest()
        {
            var go = GameObject.Find("CG_CAMARA_PRUEBA_ELIMINAR");
            if (go != null) { UnityEngine.Object.DestroyImmediate(go); EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene()); }
            AssetDatabase.DeleteAsset("Assets/CampusGlitch/CG_PRUEBA_CAMARA.png");
            AssetDatabase.DeleteAsset("Assets/CampusGlitch/CG_PRUEBA_CAMARA_UNLIT.mat");
            Debug.Log("[Campus Glitch] Patron eliminado. Guarda la escena.");
        }

        private static void SaveReport(string body)
        {
            Directory.CreateDirectory("Docs");
            File.WriteAllText("Docs/CAMARA_2D_DIAGNOSTICO.txt", body, Encoding.UTF8);
            AssetDatabase.Refresh();
        }
    }
}
#endif
