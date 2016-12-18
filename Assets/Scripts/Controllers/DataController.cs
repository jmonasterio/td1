using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataController : MonoBehaviour {

    public IEnumerable<WavePoco> Waves { get; private set; }

    public CsvFile EnemiesClassesCsv { get; private set; }
    public CsvFile TowerClassesCsv { get; private set; }
    public IEnumerable<HumanPoco> HumanClasses { get; private set; }
    public CsvFile RobotClassesCsv { get; private set; }
    public JsonFile LevelsJson { get; private set; }

    void Awake()
    {
        // Read in the CSV files.
        ReloadCsvs();
    }

	
	// Update is called once per frame
	void Update () {
	
	}

    public void ReloadCsvs()
    {
        Waves = new TypeSafeWrapper<WavePoco>(ReloadCsv("waves.csv"));
        EnemiesClassesCsv = ReloadCsv("enemy-classes.csv");
        TowerClassesCsv = ReloadCsv("tower-classes.csv");
        HumanClasses = new TypeSafeWrapper<HumanPoco>(ReloadCsv("human-classes.csv"));
        RobotClassesCsv = ReloadCsv("robot-classes.csv");
    }

    public CsvFile ReloadCsv(string fileName)
    {
        var csvFile = CsvFile.Open(new FileInfo("Assets//Data//" + fileName));
        return csvFile;
    }

    public LevelData LoadLevelData(LevelController.Levels levelId)
    {
        return new LevelData()
        {
            HumanLevelData = new List<HumanLevelData>()
            {
                new HumanLevelData() { EntityId = "male", BaseLevelData = new BaseLevelData() { GridX = 5, GridY = 5 } },
                new HumanLevelData() { EntityId = "male", BaseLevelData = new BaseLevelData() { GridX = 6, GridY = 6 }  },
            }
        };
    }

    public struct LevelData
    {
        public List<HumanLevelData> HumanLevelData;
        public List<TowerLevelData> TowerLevelData;
        public RobotLevelData RotbotLevelData;
        public List<EnemyLevelData> EnemyLevelData;
        public List<BackgroundLevelData> BackgroundLevelData;
    }

    public struct BaseLevelData
    {
        public float GridX;
        public float GridY;
    }

    public struct HumanLevelData
    {
        public BaseLevelData BaseLevelData;
        public string EntityId;
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