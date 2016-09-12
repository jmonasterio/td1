using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.Assertions.Comparers;


public struct CsvFile
{
    private CsvHeader[] _headers;
    private List<CsvLine> _lines;
    public FileInfo FileInfo;

    private static CsvHeader[] ReadHeader(string line)
    {
        var ret = new List<CsvHeader>();

        var parts = line.Split(',');
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
            ret.Add( ParseLine(line, headers, lineNumber));
            lineNumber++;
        }
        return ret;
    }

    private static CsvLine ParseLine(string line, CsvHeader[] headers, int lineNumber)
    {
        var csvLine = new CsvLine();
        csvLine.LineNumber = lineNumber;
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

public interface Poco
{
    void Init(CsvLine line);
}

public struct WavePoco : Poco
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
            switch (col.Header.ComponentField)
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

            }
        }
    }
}

public class TypeSafeWrapper<T>:List<T> where T : Poco, new()
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
