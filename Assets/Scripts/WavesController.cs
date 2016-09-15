using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private IEnumerable<WavePoco> _wavesOnLevel;


    void Start()
    {
        _levelStartTime = 0.0f;
        _enemiesCollection = GameObject.Find("Enemies"); // TBD: Maybe do this in the in the Enemy object.
                                                         // StartCoroutine(SpawnWaves());
        _wavesOnLevel = Toolbox.Instance.GameManager.GetComponent<DataController>().WavesCsv;

        StartLevel( 0.0f );
    }

    public void StartLevel( float levelStartTime)
    {
        _levelStartTime = levelStartTime;
        var paths = GetAvailablePaths();
        foreach (var path in paths)
        {

            StartCoroutine(SpawnWaves(path));
        }

    }

    IEnumerator SingleWave(IEnumerable<WavePoco> waveCsv, Path path)
    {
        //var child = _wavesOnThisLevel.transform.GetChild(childIdx);

        // TBD-JM: Maybe use a custom yield instruction here to 
        // avoid waiting if user wants to skip to next wave really fast.
        // User is only allowed to do this... when they're waiting for a wave anyway?
        //if (!_skipWaveDelay)
        //{

        // TBD: There may need to be a waves classes CSV to do this.
        //yield return new WaitForSeconds(wave.startDelayTime);

        //}
        //_skipWaveDelay = false;


        foreach ( var poco in waveCsv)
        {
            //wave.CancelWaveCoroutine();
            if (poco.EntityType == "Enemy")
            {
                Vector3 spawnPosition = path.StartWaypoint.transform.position;
                //var newEnemy = enemy.gameObject; // Instantiate<Enemy>(EnemyPrefab);
                var newEnemy = Instantiate<Enemy>(EnemyPrefab); // Make a copy, so we don't remove from tree and then we can run wave again.

                newEnemy.gameObject.name = path.gameObject.name + DateTime.Now.Ticks;
                newEnemy.transform.SetParent( _enemiesCollection.transform);
                newEnemy.transform.position = spawnPosition;
                newEnemy.gameObject.SetActive(true);

                Debug.Assert(newEnemy.isActiveAndEnabled);

                var pathFollower = newEnemy.GetComponent<PathFollower>();
                pathFollower.SetPathWaypoints(path); // TBD-JM: Need to pick targets here if there is a choice.

                this.LiveEnemyCount++;

                yield return new WaitForSeconds(poco.Delay);

            }
            else
            {
                if (poco.EntityType == "WavePause")
                {
                    yield return new WaitForSeconds(poco.Delay);
                }
                else
                {
                    if( poco.EntityType == "WaveBreak")
                    {
                        //yield return new WaitForSeconds(1.0f); // TBD REPLACE SYNC POINT
                        //Destroy(waveBreak.gameObject);
                        yield return new WaitForSeconds(poco.Delay);
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

    private static List<Path> GetAvailablePaths()
    {
        var pathsNode = GameObject.Find("Paths"); // Only finds waves on active level
        var paths = new List<Path>();
        foreach (Transform trans in pathsNode.transform)
        {
            var path = trans.gameObject.GetComponent<Path>();
            paths.Add(path);
        }
        return paths;
    }


    IEnumerator SpawnWaves(Path path)
    {
        yield return new WaitForSeconds(StartWait);
        var waveCsv = _wavesOnLevel.Where(_ => _.WaveId == 0 && _.Path == path.name);

        //foreach (Wave wave in waves)
        //{
            var coroutine = StartCoroutine(SingleWave(waveCsv, path));
            _runningWaves.Add(coroutine); // TBD: When to remove?
            //wave.SetCoroutine(coroutine);
        //}

    }

    private List<Coroutine> _runningWaves = new List<Coroutine>();
    private float _levelStartTime;

    public void CancelActiveWaves()
    {
        foreach (var co in _runningWaves)
        {
            StopCoroutine(co);
        }
        _runningWaves.Clear();
    }

    void Update()
    {
        if (LiveEnemyCount <= 0 && Time.time > _levelStartTime + 2.0f)
        {
            // Temporary hack
            Debug.Log("TBD: Next level goes here.");
            StartLevel(Time.time);
        }
    }


}


