using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private float _cellSize;

    private static EnemyManager _instance;
    public static EnemyManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<EnemyManager>();
            }

            return _instance;
        }
    }

    private List<MonsterController> _enemyList = new();
    private List<MonsterController> _pendingRemove = new();
    private SpatialHash<MonsterController> _enemyHash;

    private void Awake() => SetSingleton();
    private void Start() => Init();

    private void FixedUpdate()
    {
        _enemyHash.Clear();
        Flush();

        Debug.Log($"현재 몬스터 수 : {_enemyList.Count}");

        foreach(MonsterController enemy in _enemyList)
        {
            _enemyHash.Insert(enemy);
        }


        foreach(MonsterController enemy in _enemyList)
        {
            if (!enemy.IsReturned)
            {
                enemy.Move(Time.deltaTime);
            }
        }

        Flush();
    }

    public void Register(MonsterController enemy)
    {
        _enemyList.Add(enemy);
    }

    public void UnRegister(MonsterController enemy)
    {
        _pendingRemove.Add(enemy);
    }

    public void GetEnemies(Vector3 position, List<MonsterController> enemies)
    {
        _enemyHash.Query(position, enemies);
    }

    private void Flush()
    {
        foreach(MonsterController enemy in _pendingRemove)
        {
            _enemyList.Remove(enemy);
        }

        _pendingRemove.Clear();
    }

    private void Init()
    {
        _enemyHash = new(_cellSize);
    }

    private void SetSingleton()
    {
        if (_instance != null && _instance != this) return;

        _instance = this;
    }
}
