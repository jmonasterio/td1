using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Algorithms;
using Path = System.IO.Path;

public class DataController : MonoBehaviour {

    public IEnumerable<EnemyPoco> EnemiesClassesCsv { get; private set; }
    public IEnumerable<TowerPoco> TowerClasses { get; private set; }
    public IEnumerable<HumanPoco> HumanClasses { get; private set; }
    public IEnumerable<RobotPoco> RobotClasses { get; private set; }
    public IEnumerable<BackgroundPoco> BackgroundClasses { get; private set; }
    public JsonFile LevelsJson { get; private set; }

    void Awake()
    {
        // Read in the JSON files.
        ReloadJsonClasses();
    }

	
	// Update is called once per frame
	void Update () {
	
	}

    public void ReloadJsonClasses()
    {
        EnemiesClassesCsv = JsonUtility.FromJson<PocoList<EnemyPoco>>(LoadJson("enemy-classes.json")).List;
        TowerClasses = JsonUtility.FromJson<PocoList<TowerPoco>>(LoadJson("tower-classes.json")).List;
        HumanClasses = JsonUtility.FromJson<PocoList<HumanPoco>>(LoadJson("human-classes.json")).List;
        RobotClasses = JsonUtility.FromJson<PocoList<RobotPoco>>(LoadJson("robot-classes.json")).List;
        BackgroundClasses = JsonUtility.FromJson<PocoList<BackgroundPoco>>(LoadJson("background-classes.json")).List;
    }

    private string LoadJson(string humanClassesJson)
    {
        return File.ReadAllText( System.IO.Path.Combine("Assets//Data//", humanClassesJson));
    }

    public LevelData LoadLevelData(LevelController.Levels levelId)
    {
        return new LevelData()
        {

            Waves = JsonUtility.FromJson<PocoList<WaveLevelData>>(LoadJson("waves.json")).List.ToList(),

            // TBD: Items below should come from json for level, like "waves.json" above.
            RobotLevelData = new RobotLevelData
            {
                EntityId = "boss1",
                BaseLevelData = new BaseLevelData() { GridPoint = new GridPoint(0, 0) }
            },

            BackgroundLevelData = new List<BackgroundLevelData>()
            {
                new BackgroundLevelData() { EntityId = "block", BaseLevelData = new BaseLevelData() { GridPoint = new GridPoint(0,0) } },
            },

            HumanLevelData = new List<HumanLevelData>()
            {
                new HumanLevelData() { EntityId = "male", BaseLevelData = new BaseLevelData() { GridPoint = new GridPoint(5,5) } },
                new HumanLevelData() { EntityId = "male", BaseLevelData = new BaseLevelData() { GridPoint = new GridPoint(6,6) }  },
            },

            TowerLevelData = new List<TowerLevelData>()
            {
                new TowerLevelData() { EntityId = "male", BaseLevelData = new BaseLevelData() { GridPoint = new GridPoint(1,1) }  },
                new TowerLevelData() { EntityId = "male", BaseLevelData = new BaseLevelData() { GridPoint = new GridPoint(3,3) }  },
                new TowerLevelData() { EntityId = "male", BaseLevelData = new BaseLevelData() { GridPoint = new GridPoint(-1,-1) }  },
                new TowerLevelData() { EntityId = "male", BaseLevelData = new BaseLevelData() { GridPoint = new GridPoint(-3,-3) }  },
            },

            PathLevelData = new List<PathLevelData>()
            {
                new PathLevelData()
                {
                EntityId = "BasicPath", // AdvancedPath
                Start = new GridPoint(-8,-3),
                End = new GridPoint(4,4),
                Midpoints = new [] { new GridPoint(0,0) }
                },

                new PathLevelData()
                {
                EntityId = "AdvancedPath", // AdvancedPath
                Start = new GridPoint(10,0),
                End = new GridPoint(4,4),
                Midpoints = new [] { new GridPoint(0,0) }
                }

            }
        };
    }

    public struct LevelData
    {
        public List<WaveLevelData> Waves;
        public List<HumanLevelData> HumanLevelData;
        public List<TowerLevelData> TowerLevelData;
        public RobotLevelData RobotLevelData;
        public List<EnemyLevelData> EnemyLevelData;
        public List<BackgroundLevelData> BackgroundLevelData;
        public List<PathLevelData> PathLevelData;
    }

    public struct BaseLevelData
    {
        public GridPoint GridPoint;
    }

    public struct HumanLevelData
    {
        public BaseLevelData BaseLevelData;
        public string EntityId;
    }

    public struct PathLevelData
    {
        public string EntityId;
        public GridPoint Start;
        public GridPoint[] Midpoints;
        public GridPoint End;
    }

    public struct TowerLevelData
    {
        public BaseLevelData BaseLevelData;
        public string EntityId;
    }

    public struct EnemyLevelData
    {
        public BaseLevelData BaseLevelData;
        public string EntityId;
    }

    public struct RobotLevelData
    {
        public BaseLevelData BaseLevelData;
        public string EntityId;
    }

    public struct BackgroundLevelData
    {
        public BaseLevelData BaseLevelData;
        public string EntityId;
    }


}