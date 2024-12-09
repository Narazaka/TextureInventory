using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDKBase;

namespace Narazaka.VRChat.TextureInventory.Editor
{
    public class TextureInventoryBuilder : IProcessSceneWithReport
    {
        public int callbackOrder => -2048;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            var basePath = UdonSharp.Updater.UdonSharpLocator.IntermediatePrefabPath;
            if (!System.IO.Directory.Exists(basePath)) System.IO.Directory.CreateDirectory(basePath);

            var textureInventories = Object.FindObjectsByType<TextureInventory>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var textureInventory in textureInventories)
            {
                var textureCount = textureInventory.TextureCount;
                var baseName = $"TextureInventory_{textureInventory.GetInstanceID()}";
                var so = new SerializedObject(textureInventory);
                so.Update();
                var templateTextureProperty = so.FindProperty("_templateTexture");
                var templateTexture = templateTextureProperty.objectReferenceValue as RenderTexture;
                templateTextureProperty.objectReferenceValue = null;
                var descriptor = templateTexture.descriptor;
                var renderTextures = new RenderTexture[textureCount];
                var textures = so.FindProperty("_textures");
                textures.arraySize = textureCount;
                for (var i = 0; i < textureCount; ++i)
                {
                    textures.GetArrayElementAtIndex(i).objectReferenceValue = renderTextures[i] = new RenderTexture(descriptor) { name = $"{baseName}_{i}" };
                }
                so.ApplyModifiedProperties();

                var obj = ScriptableObject.CreateInstance<ScriptableObject>();
                AssetDatabase.CreateAsset(obj, System.IO.Path.Join(basePath, $"{baseName}.asset"));
                foreach (var renderTexture in renderTextures)
                {
                    AddObjectToAsset(renderTexture, obj);
                }
                AssetDatabase.SaveAssets();
            }
        }

        void AddObjectToAsset(Object obj, Object asset)
        {
            obj.hideFlags = HideFlags.None;
            AssetDatabase.AddObjectToAsset(obj, asset);
        }
    }
}
