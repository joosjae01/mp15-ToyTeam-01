using System.Collections.Generic;
using UnityEngine;

public class SpatialHash<T> where T : ISpatialable
{
    private float _cellSize;
    private Dictionary<Vector2Int, List<T>> _buckets = new();

    public SpatialHash(float size)
    {
        _cellSize = size;
    }

    public void Clear()
    {
        foreach(List<T> list in _buckets.Values)
        {
            list.Clear();
        }
    }

    public void Insert(T agent)
    {
        int cx = Mathf.FloorToInt(agent.Position.x / _cellSize);
        int cy = Mathf.FloorToInt(agent.Position.z / _cellSize);

        Vector2Int key = new Vector2Int(cx, cy);

        if (!_buckets.TryGetValue(key, out List<T> list))
        {
            list = new();
            _buckets.Add(key, list);
        }

        list.Add(agent);
    }

    public void Query(Vector3 position, List<T> result)
    {
        result.Clear();

        int cx = Mathf.FloorToInt(position.x / _cellSize);
        int cy = Mathf.FloorToInt(position.z / _cellSize);
        
        for(int dx = -1; dx <= 1; ++dx)
        {
            for(int dy = -1; dy <= 1; ++dy)
            {
                if(_buckets.TryGetValue(GetKey(cx + dx, cy + dy), out List<T> list)){
                    result.AddRange(list);
                }
            }
        }
    }

    public Vector2Int GetKey(int x, int y)
    {
        return new Vector2Int(x, y);
    }
}
