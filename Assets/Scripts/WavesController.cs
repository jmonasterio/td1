using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

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
    private Waves _wavesOnThisLevel;


    void Start()
    {
        _enemiesCollection = GameObject.Find("Enemies"); // TBD: Maybe do this in the in the Enemy object.
                                                         // StartCoroutine(SpawnWaves());
        _wavesOnThisLevel = GameObject.Find("Waves").GetComponent<Waves>(); // Only finds waves on active level

        StartCoroutine(SpawnWaves());
    }

    IEnumerator SingleWave(Wave wave)
    {
        //var child = _wavesOnThisLevel.transform.GetChild(childIdx);

        // TBD-JM: Maybe use a custom yield instruction here to 
        // avoid waiting if user wants to skip to next wave really fast.
        // User is only allowed to do this... when they're waiting for a wave anyway?
        //if (!_skipWaveDelay)
        //{
        yield return new WaitForSeconds(wave.startDelayTime);

        //}
        //_skipWaveDelay = false;
        var children = wave.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach ( Transform childTransform in children)
        {
            if (childTransform == wave.transform)
            {
                continue;
            }

            //wave.CancelWaveCoroutine();
            var childGameObject = childTransform.gameObject;;
            var enemy = childGameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                Vector3 spawnPosition = wave.Path.StartWaypoint.transform.position;
                //var newEnemy = enemy.gameObject; // Instantiate<Enemy>(EnemyPrefab);
                var newEnemy = Instantiate<Enemy>(enemy); // Make a copy, so we don't remove from tree and then we can run wave again.
                
                newEnemy.gameObject.name = enemy.gameObject.name;
                newEnemy.transform.SetParent( _enemiesCollection.transform);
                newEnemy.transform.position = spawnPosition;
                newEnemy.gameObject.SetActive(true);

                Debug.Assert(newEnemy.isActiveAndEnabled);

                var pathFollower = newEnemy.GetComponent<PathFollower>();
                pathFollower.SetPathWaypoints(wave.Path); // TBD-JM: Need to pick targets here if there is a choice.

                this.LiveEnemyCount++;

                yield return new WaitForSeconds(0.25f);

            }
            else
            {
                var pause = childGameObject.GetComponent<WavePause>();
                if (pause != null)
                {
                    //pause.transform.parent = _enemiesCollection.transform; // Make sure removed from tree, so that loop works, even if buffer.delay is 0.
                    //Destroy(pause.gameObject);
                    yield return new WaitForSeconds(pause.delay);
                }
                else
                {
                    var waveBreak = childGameObject.GetComponent<WaveBreak>();
                    if (waveBreak != null)
                    {
                        //yield return new WaitForSeconds(1.0f); // TBD REPLACE SYNC POINT
                        //Destroy(waveBreak.gameObject);
                        yield return new WaitForSeconds(wave.startDelayTime);
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


    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(StartWait);
        var waves = _wavesOnThisLevel.GetComponentsInChildren<Wave>();

        foreach (Wave wave in waves)
        {
            var coroutine = StartCoroutine(SingleWave(wave));
            wave.SetCoroutine(coroutine);
        }

    }



}


