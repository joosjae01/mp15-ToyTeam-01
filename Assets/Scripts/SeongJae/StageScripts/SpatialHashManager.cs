using UnityEngine;

public class SpatialHashManager : MonoBehaviour
{
    [SerializeField] private int _hashSize;
    SpatialHash<MonsterController> _spatialHash;

    private void Start() => Init();

    private void Init()
    {
        _spatialHash = new(_hashSize);
    }
}
