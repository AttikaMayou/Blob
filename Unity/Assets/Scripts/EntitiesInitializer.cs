using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

//Author : Attika

public class EntitiesInitializer : MonoBehaviour
{
    private static List<GameObject> _gameObjectsToConvert;

    private static Entity _entityValue;
    private BlobAssetStore _blobAssetStore;
    private static GameObjectConversionSettings _settings;

    private void Awake()
    {
        _blobAssetStore = new BlobAssetStore();

        _settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
    }

    private void OnDestroy()
    {
        _blobAssetStore.Dispose();
    }
    
    
    #region static methods

    public static void ConvertGameObjects(bool ball = true)
    {
        foreach (var go in _gameObjectsToConvert)
        {
            _entityValue = GameObjectConversionUtility.ConvertGameObjectHierarchy(go, _settings);
            // if(ball)
            //     EntityDirectoryScript.
        }
    }

    public static void FillGameObjectsToConvert(List<GameObject> list)
    {
        ClearGameObjectsList();
        _gameObjectsToConvert = list;
    }

    public static void FillGameObjectsToConvert(GameObject[] tab)
    {
        ClearGameObjectsList();
        foreach (var go in tab)
        {
            _gameObjectsToConvert.Add(go);
        }
    }
    
    public static void ClearGameObjectsList()
    {
        _gameObjectsToConvert.Clear();
    }
    
    #endregion
}
