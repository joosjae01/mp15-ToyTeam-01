using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour, IPoolable
{
    [SerializeField] private LayerMask _monsterLayer;
    public GameObject GameObject { get; set; }
    public ObjectPool Source { get; set; }
    private BulletData _data;
    private Rigidbody _rb;
    private bool _isGrounded;
    private Vector3 _direction;
    private List<MonsterController> _enemies = new();

    private void Awake() => CacheComponents();

    private void FixedUpdate()
    {
        if (_data == null) return;
        if (_data.Type == BulletType.RollingBallBullet && _isGrounded)
        {
            EnemyManager.Instance.GetEnemies(transform.position, _enemies);

            foreach (MonsterController enemy in _enemies)
            {
                if(Vector3.SqrMagnitude(transform.position - enemy.transform.position) <= _data.HitRadius * _data.HitRadius){
                    enemy.TakeDamage(_data.Damage);
                }
            }
        }
    }

    public void SetData(BulletData data, Vector3 position, Quaternion rotation)
    {
        _data = data;
        transform.position = position;
        transform.rotation = rotation;
        _rb.position = position;
        _rb.rotation = rotation;

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.AddForce(transform.forward * _data.Speed, ForceMode.Impulse);

        _isGrounded = false;
        _direction = transform.forward;
        _direction.y = 0;
        _direction.Normalize();

        StartCoroutine(DisableRoutine());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_data.Type == BulletType.LauncherBullet)
        {
            Explode();
        }
        
        if (_data.Type == BulletType.RollingBallBullet && !_isGrounded)
        {
            _isGrounded = true;
            _rb.velocity = _direction * _data.RollSpeed;
        }
    }
    private void Explode()
    {
        ExplodeSoundOn();

        EnemyManager.Instance.GetEnemies(transform.position, _enemies);

        foreach(MonsterController enemy in _enemies)
        {
            if(Vector3.SqrMagnitude(transform.position - enemy.Position) <= _data.ExplosionRadius)
            {
                enemy.TakeDamage(_data.Damage);
            }
        }

        ReturnToPool();
        GameObject effect = Instantiate(_data.ExplodeEffect);
        effect.transform.position = transform.position;
        Destroy(effect, 2f);
    }

    private IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(_data.ReturnDelay);
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (!gameObject.activeSelf) return;
        
        Source.Push(this);
    }
    
    // +
    private SoundPlayer _explode;
    private void ExplodeSoundOn()
    {
        _explode = SoundManager.Instance.TakeSoundPlayer();

        if (_explode == null)
        {
            return;
        }
        
        _explode.SetSoundVolume(1f)
                .SetSoundLoop(false)
                .PlaySoundWhenStart(false)
                .ConvertSourceToClip(_data.ExplodeSound)
                .Play();
        
        SoundManager.Instance.StartCoroutine(SoundOffRoutine(_explode, _data.ExplodeSound.length));
    }

    private IEnumerator SoundOffRoutine(SoundPlayer player, float time)
    {
        yield return new WaitForSeconds(time);
        player.Stop();
    }

    private void CacheComponents()
    {
        _rb = GetComponent<Rigidbody>();
    }
}