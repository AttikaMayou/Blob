using Unity.Entities;
using UnityEngine;

namespace Management
{
    public class GameObjectToEntity : MonoBehaviour
    {
        public GameObject go;

        private Entity entity;
        private BlobAssetStore blobAssetStore;

        private void Awake()
        {
            blobAssetStore = new BlobAssetStore();
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
            entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(go, settings);
            Destroy(go);
        }

        private void OnDestroy()
        {
            blobAssetStore.Dispose();
        }
    }
}
