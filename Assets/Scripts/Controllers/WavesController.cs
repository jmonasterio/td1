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


    void Start()
    {
        StartLevel(Time.time);
    }


    public void StartLevel(float levelStartTime)
    {
        Toolbox.Instance.GameManager.DataController.ReloadCsvs();
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


        var enemiesCollection = GetComponent<Level>().Nodes.Enemies;



        foreach (var poco in waveCsv)
        {
            //wave.CancelWaveCoroutine();
            if (poco.EntityType == "Enemy")
            {
                Vector3 spawnPosition = path.StartWaypoint.transform.position;
                //var newEnemy = enemy.gameObject; // Instantiate<Enemy>(EnemyPrefab);
                var newEnemy = Instantiate<Enemy>(EnemyPrefab);
                    // Make a copy, so we don't remove from tree and then we can run wave again.

                Debug.Assert(newEnemy.GetComponent<Enemy>() != null);

                newEnemy.gameObject.name = path.gameObject.name + DateTime.Now.Ticks;
                newEnemy.transform.SetParent(enemiesCollection);
                newEnemy.transform.position = spawnPosition;
                newEnemy.gameObject.SetActive(true);
                switch (poco.Data)
                {
                    case "StandardMale":
                        newEnemy.EnemyClass = Enemy.EnemyClasses.Standard;
                        break;
                    default:
                        newEnemy.EnemyClass = Enemy.EnemyClasses.Standard; // Need more types.
                        break;
                }

                Debug.Assert(newEnemy.isActiveAndEnabled);

                // Start() on enemy not called until just before first update,
                // so we can't use newEnemy.PathFollower;

                var pathFollower = newEnemy.GetComponent<PathFollower>();
                pathFollower.SetPathWaypoints(path);

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
                    if (poco.EntityType == "WaveBreak")
                    {
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
        var wavesOnLevel = Toolbox.Instance.GameManager.GetComponent<DataController>().Waves;
        var waveCsv = wavesOnLevel.Where(_ => _.WaveId == 0 && _.Path == path.name);

        var coroutine = StartCoroutine(SingleWave(waveCsv, path));
        _runningWaves.Add(coroutine);

    }

    private List<Coroutine> _runningWaves = new List<Coroutine>();
    private float _levelStartTime;

    public void CancelActiveWaves()
    {
        foreach (var co in _runningWaves)
        {
            if (co != null)
            {
                try
                {
                    StopCoroutine(co);
                }
                catch (Exception ex)
                {
                    Debug.Log( "Failed to stop coroutine: " + ex.Message);
                }
            }
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


