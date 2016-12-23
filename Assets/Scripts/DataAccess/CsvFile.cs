using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public struct JsonFile
{

}

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

public struct HumanPoco : IPoco
{
    public string EntityId;
    public float Entity_IncomeCost;
    public float Entity_Health;
    public float Entity_HealthMax;
    public float ReloadTime;
    public float Speed;

    public void Init(CsvLine line)
    {

        foreach (var col in line.Columns)
        {
            var key = col.Header.ComponentName + "." + col.Header.ComponentField;

            // TBD: This switch statement could be generated with reflection, but can't see benefit yet.
            switch (key)
            {
                case ".EntityId":
                    EntityId = col.Value;
                    break;
                case "Entity.IncomeCost":
                    Entity_IncomeCost = float.Parse(col.Value);
                    break;
                case "Entity.Health":
                    Entity_Health = float.Parse(col.Value);
                    break;
                case "Entity.HealthMax":
                    Entity_HealthMax = float.Parse( col.Value);
                    break;
                case "Entity.Speed":
                    Speed = float.Parse(col.Value);
                    break;
                case "Entity.ReloadTime":
                    ReloadTime = float.Parse(col.Value);
                    break;

                default:                    Debug.LogError("Could not parse CSV at line: " + line.LineNumber + " for key: " + key);

                    break;

            }
        }
    }
}

public struct RobotPoco : IPoco
{
    public string EntityId;
    public float Entity_IncomeCost;
    public float Entity_Health;
    public float Entity_HealthMax;
    public float ReloadTime;
    public float Speed;

    public void Init(CsvLine line)
    {

        foreach (var col in line.Columns)
        {
            var key = col.Header.ComponentName + "." + col.Header.ComponentField;

            // TBD: This switch statement could be generated with reflection, but can't see benefit yet.
            switch (key)
            {
                case ".EntityId":
                    EntityId = col.Value;
                    break;
                case "Entity.IncomeCost":
                    Entity_IncomeCost = float.Parse(col.Value);
                    break;
                case "Entity.Health":
                    Entity_Health = float.Parse(col.Value);
                    break;
                case "Entity.HealthMax":
                    Entity_HealthMax = float.Parse(col.Value);
                    break;
                case "Entity.Speed":
                    Speed = float.Parse(col.Value);
                    break;
                case "Entity.ReloadTime":
                    ReloadTime = float.Parse(col.Value);
                    break;

                default:
                    Debug.LogError("Could not parse CSV at line: " + line.LineNumber + " for key: " + key);

                    break;

            }
        }
    }
}


public struct WavePoco : IPoco
{
    public int WaveId;
    public string Path;
    public float Delay;
    public string EntityType;
    public string Notes;
    public string Data;

    public void Init(CsvLine line)
    {
        foreach (var col in line.Columns)
        {
            var key = col.Header.ComponentName + "." + col.Header.ComponentField;

            // TBD: This switch statement could be generated with reflection, but can't see benefit yet.
            switch (key)
            {
                case ".WaveId":
                    WaveId = Int32.Parse(col.Value);
                    break;
                case ".Path":
                    Path = col.Value;
                    break;
                case ".Delay":
                    Delay = float.Parse(col.Value);
                    break;
                case ".EntityType":
                    EntityType = col.Value;
                    break;
                case ".Data":
                    Data = col.Value;
                    break;
                case ".Notes":
                    Notes = col.Value;
                    break;
                default:
                    Debug.LogError("Could not parse CSV at line: " + line.LineNumber + " for key: " + key);
                    break;

            }
        }
    }
}

public class TypeSafeWrapper<T>:List<T> where T : IPoco, new()
{
    private TypeSafeWrapper()
    {
    }

    public TypeSafeWrapper(CsvFile csv)
    {
        foreach (var line in csv.GetRestofLines())
        {
            var item = new T();
            item.Init(line);
            this.Add( item);
        }
    }

    public List<T> Make()
    {
        return new List<T>();
    }
}
