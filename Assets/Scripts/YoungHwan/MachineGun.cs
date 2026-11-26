using UnityEngine;
using static UnityEngine.UI.Image;

public class MachineGun : TurretBase
{
    [Header("머신건 - 레이어")]
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private LayerMask _monsterLayer;
    [Header("머신건 - 이펙트")]
    [SerializeField] private GameObject _hitEffect;
    [Header("머신건 - 발사")]
    [SerializeField] private int _minBullets;
    [SerializeField] private int _maxBullets;
    [SerializeField] private float _spreadAngle = 1f;
    [Header("머신건 - 판정")]
    [SerializeField] private float _bulletRange;
    [SerializeField] private float _bulletRadius = 1f;
    [SerializeField] private float _explosionRadius = 0.3f;
    [SerializeField] private float _bulletDamage = 900f;
    public override void Attack()
    {
        int bullets = Random.Range(_minBullets, _maxBullets + 1);
        for (int i = 0; i < bullets; i++)
        {
            Quaternion spread = Quaternion.Euler(Random.Range(-_spreadAngle, _spreadAngle), Random.Range(-_spreadAngle, _spreadAngle), 0f);
            Vector3 direction = _firePoint.rotation * spread * Vector3.forward;
            Ray ray = new Ray(_firePoint.position, direction);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit,_bulletRange, _targetLayer))
            {
                Debug.DrawLine(_firePoint.position, hit.point, Color.red);
                Vector3 target = hit.point;
                EnemyManager.Instance.GetEnemies(target, _targets);
                Debug.Log($"검사한 몬스터 수 : {_targets.Count}");

                foreach (MonsterController enemy in _targets)
                {
                    if(Vector3.SqrMagnitude(target - enemy.Position) <= _bulletRadius * _bulletRadius)
                    {
                        enemy.TakeDamage(_bulletDamage);
                    }
                }
            }
        }
    }
}
