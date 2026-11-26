using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private int _width = 0;
    private int _height = 0;
    private Node[,] _flowField;

    private Vector2Int _targetCell;
    private Vector3 _origin;

    private void SetGrid()
    {
        _flowField = new Node[_width, _height];

        for(int x = 0; x < _width; ++x)
        {
            for(int y = 0; y < _height; ++y)
            {
                _flowField[x, y] = new Node(int.MaxValue, Vector2Int.zero);
            }
        }
    }

    private void SetFlowField()
    {
        Queue<Vector2Int> queue = new();
        List<Vector2Int> passable = new();
        bool[] visited = new bool[_width * _height];

    }
}

public struct Node
{
    public int Cost;
    public Vector2Int Direction;
    public Node(int cost, Vector2Int direction)
    {
        Cost = cost;
        Direction = direction;
    }
}
