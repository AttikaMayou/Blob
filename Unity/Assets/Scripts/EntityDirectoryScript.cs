using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Unity.Entities;

//Author : Attika

public class EntityDirectoryScript : MonoBehaviour
{
    public List<Entity> allEntities;
    public int activeEntities = 0;
    private Dictionary<int, Entity> _ballsEntities = new Dictionary<int, Entity>();
    private Dictionary<int, Entity> _environmentEntities = new Dictionary<int, Entity>();

    public bool GetEntityByIndex(int index, out Entity entity, bool ball = true)
    {
        return ball ? _ballsEntities.TryGetValue(index, out entity) 
                    : _environmentEntities.TryGetValue(index, out entity);
    }

    public void GetEntityIndex(Entity entity)
    {
        //TODO : find a way to handle entity' index
    }

    public void AddEntity(Entity entity, out int id, bool ball = true)
    {
        if(allEntities == null)
            allEntities = new List<Entity>();
        
        if(!allEntities.Contains(entity))
            allEntities.Add(entity);

        ++activeEntities;
        
        id =  ball ? AddEntityToDirectory(entity)
                   : AddEntityToDirectory(entity, false);
    }

    private int AddEntityToDirectory(Entity entity, bool ball = true)
    {
        var id = -1;
        
        if (_ballsEntities.ContainsValue(entity)) return id;
        _ballsEntities.Add(entity.Index, entity);
        
        id = activeEntities - 1;
        
        return id;
    }

    public void RemoveEntity(Entity entity, bool ball = true)
    {
        activeEntities--;
        
        //TODO : set index to -1 on this entity (in the way you handle it)
        
        if (allEntities.Contains(entity))
            allEntities.Remove(entity);
    }
}
