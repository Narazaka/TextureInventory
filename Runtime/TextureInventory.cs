using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.TextureInventory
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TextureInventory : UdonSharpBehaviour
    {
        [SerializeField] RenderTexture _templateTexture;
        [SerializeField] int _textureCount = 1;
        [SerializeField, HideInInspector] RenderTexture[] _textures = new RenderTexture[0];

        [PublicAPI]
        public int TextureCount => _textureCount;

        [UdonSynced] ushort _textureAge = 0;
        [UdonSynced] ushort[] _textureAges = new ushort[0];

        void OnEnable()
        {
            _textureAges = new ushort[_textureCount];
        }

        [PublicAPI]
        public void Apply(int index, RenderTexture texture)
        {
            VRCGraphics.Blit(_textures[index], texture);
        }

        [PublicAPI]
        public void Add(Texture texture)
        {
            var index = OldestTextureIndex();
            Put(index, texture);
        }

        [PublicAPI]
        public void Put(int index, Texture texture)
        {
            VRCGraphics.Blit(texture, _textures[index]);
            _textureAges[index] = ++_textureAge;
            QueueSerialization();
        }

        [PublicAPI]
        public void Invalidate(int index)
        {
            _textureAges[index] = 0;
            QueueSerialization();
        }

        [PublicAPI]
        public void InvalidateAll()
        {
            for (int i = 0; i < _textureCount; ++i)
            {
                _textureAges[i] = 0;
            }
            QueueSerialization();
        }

        [PublicAPI]
        public void Release(int index)
        {
            _textures[index].Release();
            Invalidate(index);
        }

        [PublicAPI]
        public void ReleaseAll()
        {
            for (int i = 0; i < _textureCount; ++i)
            {
                _textures[i].Release();
            }
            InvalidateAll();
        }

        int OldestTextureIndex()
        {
            var index = 0;
            ushort minAge = _textureAges[0];
            if (minAge == 0) return index;

            for (int i = 1; i < _textureCount; ++i)
            {
                var age = _textureAges[i];
                if (age < minAge)
                {
                    index = i;
                    minAge = age;
                    if (minAge == 0) break;
                }
            }
            return index;
        }

        void QueueSerialization()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
    }
}
