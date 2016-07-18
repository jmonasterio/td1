using UnityEngine;
using System.Collections;

public class WavesController : MonoBehaviour
{

    public Enemy EnemyPrefab;
    public Waypoint SpawnWaypoint;
    public int HazardCount;
    public float SpawnWait;
    public float StartWait;
    public float WaveWait;
    private GameObject _enemiesCollection;

    void Start()
    {
        _enemiesCollection = GameObject.Find("Enemies"); // TBD: Maybe do this in the in the Enemy object.
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
                newEnemy.transform.SetParent(_enemiesCollection.transform);
                newEnemy.transform.position = spawnPosition;
                Toolbox.Instance.GameManager.Enemies().Add(newEnemy);
                yield return new WaitForSeconds(SpawnWait);
            }
            yield return new WaitForSeconds(WaveWait);
        }
    }


    IEnumerator SpawnWaves2()
    {
        yield return new WaitForSeconds(StartWait);
        while (true)
        {
            // Switch on next item in Waves heirarchy.


            // Until no more. Then, then level may be over.

            for (int i = 0; i < HazardCount; i++)
            {
                Vector3 spawnPosition = SpawnWaypoint.transform.position;
                var newEnemy = Instantiate<Enemy>(EnemyPrefab);
                newEnemy.transform.SetParent(_enemiesCollection.transform);
                newEnemy.transform.position = spawnPosition;
                Toolbox.Instance.GameManager.Enemies().Add(newEnemy);
                yield return new WaitForSeconds(SpawnWait);
            }
            yield return new WaitForSeconds(WaveWait);
        }
    }

}
