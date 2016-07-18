using UnityEngine;
using System.Collections;

public class WavesController : MonoBehaviour {

    public Enemy EnemyPrefab;
    public Waypoint SpawnWaypoint;
    public int HazardCount;
    public float SpawnWait;
    public float StartWait;
    public float WaveWait;

    void Start()
        {
        StartCoroutine(SpawnWaves());
        }

    IEnumerator SpawnWaves()
        {
        yield return new WaitForSeconds(StartWait);
        while (true)
            {
            for (int i = 0; i < HazardCount; i++)
                {
                Vector3 spawnPosition = SpawnWaypoint.transform.position;
                var newEnemy = Instantiate<Enemy>(EnemyPrefab);
                newEnemy.transform.position = spawnPosition;
                yield return new WaitForSeconds(SpawnWait);
                }
            yield return new WaitForSeconds(WaveWait);
            }
        }
    }
