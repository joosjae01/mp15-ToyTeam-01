using System;
using UnityEngine;
public class MonsterController : MonoBehaviour, IDamageable, IPoolable, ISpatialable
{
    [Header("몬스터 데이터")]
    private MonsterData _monsterData;
    
    public GameObject GameObject { get; set; }
    public ObjectPool Source { get; set; }

    public Vector3 Position { get => transform.position; }

    public event Action<MonsterData> OnKilled;
    public event Action<MonsterData> OnNexusArrived;

    public bool IsReturned = true;

    private void Awake() => CacheComponents();
    private void OnEnable()
    {
        IsReturned = false;
        EnemyManager.Instance.Register(this);
    }
    private void OnDisable()
    {
        IsReturned = true;
        
        // +
        OnNexusArrived = null;
        OnKilled = null;
        EnemyManager.Instance.UnRegister(this);
    }

    // =============== 기본 이동 로직 ===============
    public void Move(float time)
    {
        transform.Translate(Vector3.forward * _monsterData.MoveSpeed * time);
    }

    // =============== Nexus 도착 시 수행할 로직 ===============
    public void ArriveNexus()
    {   
        OnNexusArrived?.Invoke(_monsterData);
        ReturnToPool();
    }

    // =============== IDamageable 구현 ===============
    public void TakeDamage(float damage)
    {
        if (damage >= _monsterData.Health)
        {
            if (IsReturned)
                return;
            
            _monsterData.Health = 0;

            OnKilled?.Invoke(_monsterData);
            OnNexusArrived = null;
            OnKilled = null;
            ReturnToPool();
        }
        else
        {
            _monsterData.Health -= damage;
        }
    }

    // =============== IPoolable 구현 ===============

    public void ReturnToPool()
    {
        // +++
        if (IsReturned) 
            return;

        Source.Push(this);
    }

    // =============== Awake() 초기화 메서드 ===============

    private void CacheComponents() => _monsterData = GetComponent<MonsterData>();
}
