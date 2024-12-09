using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;

namespace Narazaka.VRChat.TextureInventory.Editor
{
    [CustomEditor(typeof(TextureInventory))]
    public class TextureInventoryEditor : UnityEditor.Editor
    {
        SerializedProperty _templateTexture;
        SerializedProperty _textureCount;
        SerializedProperty _textures;

        bool _foldoutTextures = false;

        void OnEnable()
        {
            if (!EditorApplication.isPlaying) _templateTexture = serializedObject.FindProperty("_templateTexture");
            _textureCount = serializedObject.FindProperty("_textureCount");
            _textures = serializedObject.FindProperty("_textures");
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.UpdateIfRequiredOrScript();
            if (!EditorApplication.isPlaying) EditorGUILayout.PropertyField(_templateTexture);
            EditorGUILayout.PropertyField(_textureCount);
            if (EditorApplication.isPlaying) EditorGUILayout.PropertyField(_textures, true);
            if (_textureCount.intValue < 1) _textureCount.intValue = 1;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
