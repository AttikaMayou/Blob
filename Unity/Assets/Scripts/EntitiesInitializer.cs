using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

//Author : Attika

public class EntitiesInitializer : MonoBehaviour
{
    //Handle EntitiesInitializer unique instance
    private static EntitiesInitializer _instance;
    public EntitiesInitializer GetInstance()
    {
        return _instance;
    }
    
    private List<GameObject> _gameObjectsToConvert;

    private Entity _entityValue;
    private BlobAssetStore _blobAssetStore;
    private GameObjectConversionSettings _settings;

    private void Awake()
    {
        _instance = this;
        
        _blobAssetStore = new BlobAssetStore();

        _settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
    }

    private void OnDestroy()
    {
        _blobAssetStore.Dispose();
    }
    
    
    #region public methods

    public void ConvertGameObjects(bool ball = true)
    {
        foreach (var go in _gameObjectsToConvert)
        {
            _entityValue = GameObjectConversionUtility.ConvertGameObjectHierarchy(go, _settings);
            var _id = -1;
            EntityDirectoryScript.GetInstance().AddEntity(_entityValue, out _id, ball);
            //TODO : handle _id value => do something with it !
        }
    }

    public void FillGameObjectsToConvert(List<GameObject> list)
    {
        ClearGameObjectsList();
        _gameObjectsToConvert = list;
    }

    public void FillGameObjectsToConvert(GameObject[] tab)
    {
        ClearGameObjectsList();
        foreach (var go in tab)
        {
            _gameObjectsToConvert.Add(go);
        }
    }
    
    public void ClearGameObjectsList()
    {
        _gameObjectsToConvert.Clear();
    }
    
    #endregion
}
