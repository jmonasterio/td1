using UnityEngine;
using System.Collections;

public class WavesController : MonoBehaviour
{
    public int LiveEnemyCount = 0;
    public Enemy EnemyPrefab;
    public int HazardCount;
    public float SpawnWait;
    public float StartWait;
    public float WaveWait;
    private GameObject _enemiesCollection;
    private bool _skipToNextWave;
    private bool _skipWaveDelay;


    void Start()
    {
        _enemiesCollection = GameObject.Find("Enemies"); // TBD: Maybe do this in the in the Enemy object.
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        var waves = GameObject.Find("Waves");
        yield return new WaitForSeconds(StartWait);
        for(int childIdx = 0; childIdx < waves.transform.childCount; childIdx++)
        {
            var child = waves.transform.GetChild(childIdx);
            var wave = child.gameObject.GetComponent<Wave>();
            if (wave == null)
            {
                Debug.LogError( "Only wave objects can be children of waves collection.");
            }

            // TBD-JM: Maybe use a custom yield instruction here to 
            // avoid waiting if user wants to skip to next wave really fast.
            // User is only allowed to do this... when they're waiting for a wave anyway?
            //if (!_skipWaveDelay)
            //{
                yield return new WaitForSeconds(wave.startDelayTime);

            //}
            //_skipWaveDelay = false;
            while( wave.transform.childCount > 0)
                {
                var subChild = wave.transform.GetChild(0);
                var enemy = subChild.GetComponent<Enemy>();
                if (enemy != null)
                {
                    

                    Vector3 spawnPosition = wave.StartWaypoint.transform.position;
                    //var newEnemy = enemy.gameObject; 
                    var newEnemy = enemy.gameObject; // Instantiate<Enemy>(EnemyPrefab);
                    newEnemy.transform.parent = _enemiesCollection.transform;
                    newEnemy.transform.position = spawnPosition;
                    newEnemy.gameObject.SetActive(true);

                    var pathFollower = newEnemy.GetComponent<PathFollowerStartToEnd>();
                    pathFollower.SetPathFromStartToEndWayPoints(wave.StartWaypoint, wave.EndWaypoint); // TBD-JM: Need to pick targets here if there is a choice.

                    this.LiveEnemyCount ++;

                    yield return new WaitForSeconds( 0.25f);

                }
                else
                {
                    var pause = subChild.GetComponent<WavePause>();
                    if (pause != null)
                    {
                        pause.transform.parent = _enemiesCollection.transform; // Make sure removed from tree, so that loop works, even if buffer.delay is 0.
                        Destroy(pause.gameObject);
                        yield return new WaitForSeconds(pause.delay);
                    }
                    else
                    {
                        var waveBreak = subChild.GetComponent<WaveBreak>();
                        if (waveBreak != null)
                        {
                            //yield return new WaitForSeconds(1.0f); // TBD REPLACE SYNC POINT
                            break;
                        }
                        else
                        {

                            Debug.Assert(false, "Unknown wave child type: Only enemy and waveBuffer allowed.");
                        }
                    }

                }
            }
            yield return new WaitForSeconds(WaveWait);

        }
    }

}


