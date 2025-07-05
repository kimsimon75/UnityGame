#if UNITY_EDITOR
namespace RaycastPro.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RaycastPro;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Icon manager.
    /// </summary>
    public static class IconManager
    {
        /// <summary>
        /// Set the icon for this object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="texture">The icon.</param>
        public static void SetIcon(this Object obj, Texture2D texture)
        {
            try
            {
#if UNITY_2021_2_OR_NEWER && !UNITY_2021_1 && !UNITY_2021_2
                EditorGUIUtility.SetIconForObject(obj, texture);
#else
                var ty = typeof(EditorGUIUtility);
                var method = ty.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);

                method.Invoke(null, new object[] {obj, texture});
#endif

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public static Texture2D GetIconFromName(string name)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(RCProPanel.ResourcePath + $"/{name}.png");
            return texture;
        }

        //private static string Resource_Path = "Assets/Plugins/RaycastPro/Resources";
        public static Texture2D Header => AssetDatabase.LoadAssetAtPath<Texture2D>(RCProPanel.ResourcePath + (Autorun.DarkMode ? "/RaycastPro_Header.png" : "/RaycastPro_Header_Light.png"));
        public static Texture2D Logo => AssetDatabase.LoadAssetAtPath<Texture2D>(RCProPanel.ResourcePath + "/RaycastPro_Logo.png");
        public static string[] TextureGUIDS;
        public static List<Texture2D> texture2Ds;

    }
}
#endif