using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataController : MonoBehaviour {

    public IEnumerable<WavePoco> WavesCsv { get; private set; }

    public CsvFile EnemiesClassesCsv { get; private set; }
    public CsvFile TowerClassesCsv { get; private set; }
    public CsvFile HumanClassesCsv { get; private set; }
    public CsvFile RobotClassesCsv { get; private set; }

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
        WavesCsv = new TypeSafeWrapper<WavePoco>(ReloadCsv("waves.csv"));
        EnemiesClassesCsv = ReloadCsv("enemy-classes.csv");
        TowerClassesCsv = ReloadCsv("tower-classes.csv");
        HumanClassesCsv = ReloadCsv("human-classes.csv");
        RobotClassesCsv = ReloadCsv("robot-classes.csv");
    }

    public CsvFile ReloadCsv(string fileName)
    {
        var csvFile = CsvFile.Open(new FileInfo("Assets//Data//" + fileName));
        return csvFile;
    }

}