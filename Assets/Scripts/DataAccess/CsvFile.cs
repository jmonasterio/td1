using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public struct PocoList<T> where T:struct
{
    public T[] List;
}


public struct JsonFile
{
    public static void Test()
    {
#if CONVERT_CSV
        var Towers = new PocoList<TowerPoco>()
        {
            List = new TypeSafeWrapper<TowerPoco>(DataController.ReloadCsv("tower-classes.csv")).Make().ToArray()
        };
        var j = JsonUtility.ToJson(Towers);
#endif

        /*
        var humanClasses = new PocoList<HumanPoco>()
        {
            List = new HumanPoco[] {
            new HumanPoco() { EntityId = "Pawn", Entity_Health=3, Entity_HealthMax = 5, Entity_IncomeCost = 0, ReloadTime = 0.75f, Speed=2.0f},
            new HumanPoco() { EntityId = "Bishop", Entity_Health=7, Entity_HealthMax = 7, Entity_IncomeCost = 0, ReloadTime = 0.75f, Speed=2.0f},
            new HumanPoco() { EntityId = "Queen", Entity_Health=11, Entity_HealthMax = 11, Entity_IncomeCost = 0, ReloadTime = 0.75f, Speed=2.0f},
            }
        };
        var j = JsonUtility.ToJson(humanClasses);
        var txt = JsonUtility.FromJson<PocoList<HumanPoco>>(j);
        JsonUtility.FromJsonOverwrite(j, humanClasses);
        */
    }
}

#if DEAD

public struct CsvFile
{
    private CsvHeader[] _headers;
    private List<CsvLine> _lines;
    public FileInfo FileInfo;

    private static CsvHeader[] ReadHeader(string line)
    {
        var ret = new List<CsvHeader>();

        var trimmed = line.Trim();

        int lastSemicolon = trimmed.LastIndexOf(';');
        if (lastSemicolon >= 0)
        {
            trimmed = trimmed.Substring(0, lastSemicolon);
        }

        var parts = trimmed.Split(',');
        foreach (var part in parts)
        {
            var newHeader = new CsvHeader();
            var subparts = part.Trim().Split('.');
            if (subparts.Length == 2)
            {
                newHeader.ComponentName = subparts[0].Trim();
                newHeader.ComponentField = subparts[1].Trim();
            }
            else
            {
                newHeader.ComponentField = subparts[0].Trim();
            }
            ret.Add(newHeader);
        }
        return ret.ToArray();
    }

    public List<CsvLine> GetRestofLines()
    {
        return _lines;
    }

    public static CsvFile Open( FileInfo fileName)
    {
        var file = new CsvFile();
        file.FileInfo = fileName;
        var lines = File.ReadAllLines(fileName.FullName);
        file._headers = ReadHeader(lines.FirstOrDefault());
        file._lines = ParseLines(lines.Skip(1).ToArray(), file._headers);
        return file;
    }

    private static List<CsvLine> ParseLines(string[] lines, CsvHeader[] headers)
    {
        var ret = new List<CsvLine>();
        int lineNumber = 0;
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            var comment = "";

            int lastSemicolon = trimmed.LastIndexOf(';');
            if (lastSemicolon >= 0)
            {
                comment = trimmed.Substring(lastSemicolon);
                trimmed = trimmed.Substring(0, lastSemicolon);
            }

            if (trimmed.Length > 0)
            {
                ret.Add(ParseLine(trimmed, headers, lineNumber, comment));
            }
            lineNumber++;
        }
        return ret;
    }

    private static CsvLine ParseLine(string line, CsvHeader[] headers, int lineNumber, string comment)
    {
        var csvLine = new CsvLine();
        csvLine.LineNumber = lineNumber;
        csvLine.Comment = comment;
        var parts = line.Split(',');
        csvLine.Columns = new List<CsvColumn>();
        int colIdx = 0;
        foreach (var part in parts)
        {
            var newCol = new CsvColumn();
            newCol.Header = headers[colIdx];
            newCol.Value = part.Trim();
            csvLine.Columns.Add(newCol);
            colIdx++;
        }
        return csvLine;
    }
}

public struct CsvLine
{
    public int LineNumber;
    public string Comment;
    public List<CsvColumn> Columns;
}

public struct CsvColumn
{
    public string Value;
    public CsvHeader Header;
}

public struct CsvHeader
{
    public string ComponentName;
    public string ComponentField;
}

public interface IPoco
{
    void Init(CsvLine line);
}
#endif

[Serializable]
public struct TowerPoco
{
    public string EntityId;
    public float Entity_IncomeCost;
    public float Entity_Health;
    public float Entity_HealthMax;
    public float ReloadTime;
    public float Speed;

}

[Serializable]
public struct HumanPoco 
{
    public string EntityId;
    public float Entity_IncomeCost;
    public float Entity_Health;
    public float Entity_HealthMax;
    public float ReloadTime;
    public float Speed;

}

[Serializable]
public struct EnemyPoco 
{
    public string EntityId;
    public float Entity_IncomeCost;
    public float Entity_Health;
    public float Entity_HealthMax;
    public float ReloadTime;
    public float Speed;
    public int Enemy_FlagCount;

}


[Serializable]
public struct BackgroundPoco 
{
    public string EntityId;

}

[Serializable]
public struct RobotPoco 
{
    public string EntityId;
    public float Entity_IncomeCost;
    public float Entity_Health;
    public float Entity_HealthMax;
    public float ReloadTime;
    public float Speed;
}


[Serializable]
public struct WaveLevelData 
{
    public int WaveId;
    public string Path;
    public float Delay;
    public string EntityType;
    public string Notes;
    public string Data;

    
}

