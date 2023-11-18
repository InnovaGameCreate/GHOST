using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _spawnPoint;

    public Vector3 GetSpawnPoint()
    {
        return _spawnPoint[Random.Range(0, _spawnPoint.Count)].transform.position;
    }
}
