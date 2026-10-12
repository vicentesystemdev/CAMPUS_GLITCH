using System;
using System.Reflection;
using UnityEngine;

namespace CampusGlitch
{
    /// <summary>Camera and Global Light 2D initialization, safe for a Universal 2D project.</summary>
    public static class CGCamera2DRuntime
    {
        public const string LightObjectName = "CG_LUZ_GLOBAL_2D";
        private static Type cachedLightType;

        public static Type GetLight2DType()
        {
            if (cachedLightType != null) return cachedLightType;
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = a.GetType("UnityEngine.Rendering.Universal.Light2D", false);
                if (t != null) { cachedLightType = t; break; }
            }
            return cachedLightType;
        }

        public static Camera ConfigureCamera(Camera cam)
        {
            if (cam == null)
            {
                var go = new GameObject("Main Camera - Campus Glitch 2D");
                cam = go.AddComponent<Camera>();
            }
            cam.gameObject.tag = "MainCamera";
            cam.gameObject.SetActive(true);
            cam.enabled = true;
            cam.orthographic = true;
            cam.orthographicSize = 5.7f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 200f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.06f, 0.10f, 0.17f, 1f);
            cam.cullingMask = -1;
            cam.rect = new Rect(0f, 0f, 1f, 1f);
            cam.targetTexture = null;
            cam.targetDisplay = 0;
            cam.transform.rotation = Quaternion.identity;
            var p = cam.transform.position;
            if (p.z >= -0.5f || p.z < -100f) cam.transform.position = new Vector3(p.x, p.y, -10f);
            Type extra = FindType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData");
            if (extra != null)
            {
                Component data = cam.GetComponent(extra);
                if (data == null) data = cam.gameObject.AddComponent(extra);
                PropertyInfo renderType = extra.GetProperty("renderType", BindingFlags.Public | BindingFlags.Instance);
                if (renderType != null && renderType.CanWrite)
                {
                    try { renderType.SetValue(data, Enum.Parse(renderType.PropertyType, "Base"), null); }
                    catch (Exception) { /* Older/newer URP uses Base as its default. */ }
                }
            }
            return cam;
        }

        public static Type FindType(string fullName)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = a.GetType(fullName, false);
                if (t != null) return t;
            }
            return null;
        }

        public static Component EnsureGlobalLight()
        {
            Type t = GetLight2DType();
            if (t == null)
            {
                Debug.LogError("[Campus Glitch] No se encontro Light2D. Comprueba URP en Package Manager y la plantilla Universal 2D.");
                return null;
            }
            var found = UnityEngine.Object.FindObjectsByType(t, FindObjectsSortMode.None);
            PropertyInfo typeProp = t.GetProperty("lightType", BindingFlags.Public | BindingFlags.Instance);
            foreach (UnityEngine.Object obj in found)
            {
                if (obj is Component c && c.gameObject.activeInHierarchy && c is Behaviour b && b.enabled)
                {
                    string value = typeProp != null ? Convert.ToString(typeProp.GetValue(c, null)) : "";
                    if (value == "Global") return c;
                }
            }
            GameObject go = new GameObject(LightObjectName);
            Component newLight = go.AddComponent(t);
            if (typeProp != null && typeProp.CanWrite)
            {
                try { typeProp.SetValue(newLight, Enum.Parse(typeProp.PropertyType, "Global"), null); }
                catch (Exception ex) { Debug.LogError("[Campus Glitch] No se pudo configurar Global Light 2D: " + ex.Message); }
            }
            PropertyInfo intensity = t.GetProperty("intensity", BindingFlags.Public | BindingFlags.Instance);
            if (intensity != null && intensity.CanWrite) intensity.SetValue(newLight, 1.15f, null);
            PropertyInfo color = t.GetProperty("color", BindingFlags.Public | BindingFlags.Instance);
            if (color != null && color.CanWrite) color.SetValue(newLight, Color.white, null);
            Debug.Log("[Campus Glitch] Global Light 2D creada. Los Sprite-Lit ahora tienen iluminacion.");
            return newLight;
        }

        public static void PrepareForPlay()
        {
            Camera camera = Camera.main;
            if (camera == null) camera = UnityEngine.Object.FindFirstObjectByType<Camera>();
            camera = ConfigureCamera(camera);
            if (camera.transform.position.z >= -0.5f)
                camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, -10f);
            EnsureGlobalLight();
            if (UnityEngine.Object.FindFirstObjectByType<AudioListener>() == null)
                camera.gameObject.AddComponent<AudioListener>();
        }
    }
}
