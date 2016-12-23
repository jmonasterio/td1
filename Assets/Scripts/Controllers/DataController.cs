﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Algorithms;

public class DataController : MonoBehaviour {

    public IEnumerable<WavePoco> Waves { get; private set; }

    public CsvFile EnemiesClassesCsv { get; private set; }
    public CsvFile TowerClassesCsv { get; private set; }
    public IEnumerable<HumanPoco> HumanClasses { get; private set; }
    public IEnumerable<RobotPoco> RobotClasses { get; private set; }
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
        RobotClasses = new TypeSafeWrapper<RobotPoco>(ReloadCsv("robot-classes.csv"));
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
                new HumanLevelData() { EntityId = "male", BaseLevelData = new BaseLevelData() { GridPoint = new GridPoint(5,5) } },
                new HumanLevelData() { EntityId = "male", BaseLevelData = new BaseLevelData() { GridPoint = new GridPoint(6,6) }  },
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
        public List<HumanLevelData> HumanLevelData;
        public List<TowerLevelData> TowerLevelData;
        public RobotLevelData RotbotLevelData;
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