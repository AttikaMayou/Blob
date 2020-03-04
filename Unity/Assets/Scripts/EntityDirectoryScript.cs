using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Unity.Entities;

//Author : Attika

public class EntityDirectoryScript : MonoBehaviour
{
    public List<Entity> allEntities;
    private Dictionary<int, Entity> _ballsEntities = new Dictionary<int, Entity>();
    private Dictionary<int, Entity> _environmentEntities = new Dictionary<int, Entity>();

    public bool GetBallEntity(int index, out Entity entity)
    {
        return _ballsEntities.TryGetValue(index, out entity);
    }

    public bool GetEnvironmentEntity(int index, out Entity entity)
    {
        return _environmentEntities.TryGetValue(index, out entity);
    }

    public void AddBallEntity(Entity entity, out int id)
    {
        id = -1;
    }

    private int AddBallToDirectory(Entity ball)
    {
        if (_ballsEntities.ContainsValue(ball)) return -1;
        _ballsEntities.Add(ball.Index, ball);
        return ball.Index;
    }
}
