using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GameGrid : MonoBehaviour
{
    public GameObject Map;

    public const int BACKGROUND_LAYER = 8;
    public const int TOWER_LAYER = 9;
    public const int ENEMY_LAYER = 10;
    public const int BULLET_LAYER = 11;

    public GameCell[,] Cells;
    private Waypoint StartWaypoint;
    private Waypoint EndWaypoint;
    private Rect _mapInternalGrid;
    private Vector2 _mapDims;
    public GameObject Selector;

    public Vector3? MapScreenToMapPositionOrNull(Vector2 screenPos)
    {
        var pos2 = Camera.main.ScreenToWorldPoint(screenPos);
        pos2.z = 0;
        if (_mapInternalGrid.Contains(pos2))
        {
            return pos2;
        }
        return null;
    }

    public List<GameCell> CurrentPath(PathFollower follwer)
    {
        var list = new List<GameCell>();

        if (follwer.Path != null)
        {
            foreach (var node in follwer.Path)
            {
                var gameCell = Cells[node.X, node.Y];
                list.Add(gameCell);

            }
        }
        return list;

    }




    public void InitGameGrid(Vector2 vec)
    {
        int GRID_ROWS = (int) vec.x;
        int GRID_COLS = (int) vec.y;

        Cells = new GameCell[GRID_ROWS, GRID_COLS];
        for (int ii = Cells.GetLength(0) - 1; ii >= 0; ii--)
        {
            for (int jj = Cells.GetLength(1) - 1; jj >= 0; jj--)
            {
                Cells[ii, jj] = new GameCell()
                {
                    GridPoint = new GridPoint(ii, jj)
                };
            }
        }
    }

    public List<GameCell> FindPath( GameCell start, GameCell end, PathFollower forFollower)
    {
        var byteGrid = ToByteGrid();

        var pf = new PathFinderFast(byteGrid);
        pf.Diagonals = true;
        pf.Formula = HeuristicFormula.DiagonalShortCut;
        pf.HeuristicEstimate = 2;
        pf.HeavyDiagonals = true;
        pf.PunishChangeDirection = false;
        pf.SearchLimit = 15000;
        pf.TieBreaker = false;
        forFollower.Path = pf.FindPath(start.GridPoint, end.GridPoint);
        return CurrentPath(forFollower);

    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        //InitCellMapFromLevelMap(Map);
#endif
        //var start = this.MapGridPointToCell( MapVectorToGridPoint(this.StartWaypoint.transform.position));
       // var end = this.MapGridPointToCell(MapVectorToGridPoint(this.EndWaypoint.transform.position));

        //FindPath(start, end);
    }

    public void Start()
    {
        // Initialize connection to others.
        InitCellMapFromLevelMap(Map);
    }

    public static GameGrid.GameCell FindPointOnPath(GridPoint nextGridPoint, List<GameGrid.GameCell> path)
    {
        foreach (var node in path)
        {
            if (node.GridPoint == nextGridPoint)
            {
                return node;
            }
        }
        return null;
    }

    public void InstaniatePrefabAtGameCell(GameObject prefab, GameCell cell)
    {
        cell.GroundType = GameGrid.GameCell.GroundTypes.Dirt;

        var newSquare = Instantiate(prefab);
        newSquare.transform.SetParent(this.Map.transform);
        newSquare.transform.position = MapGridPointToPosition(cell.GridPoint);
        cell.BackgroundGameObject = newSquare;

        var snap = newSquare.GetComponent<SnapToGrid>();
        snap.snapToGrid = false;
    }

    private Vector3 MapGridPointToPosition(GridPoint gridPoint)
    {
        return new Vector3( Mathf.RoundToInt((float) gridPoint.X + _mapInternalGrid.min.x),
            Mathf.RoundToInt((float) gridPoint.Y + _mapInternalGrid.min.y), 0);
    }

    public void InitCellMapFromLevelMap(GameObject map)
    {
        _mapInternalGrid = GridHelper.GetInternalGridRect(map);

        _mapDims = CalcMapDims(_mapInternalGrid);
        InitGameGrid(_mapDims);

        var oos = GetObjectsInLayer(BACKGROUND_LAYER);

        // Walk thru the grid and figure out terrain type for each
        //  block.
        // This is mostly, so I can:
        //  1) Do a path search to find out where units can go.
        //  2) Place targets and obstacles created by designer.
        //  3) Place any initial items on the map.
        foreach (var obj in oos)
        {
            var cell = MapPositionToGameCellOrNull(obj.transform.position);
            if( cell != null)
            {
                if (cell.BackgroundGameObject != null)
                {
                    // There may be more than one shape on a square? What to do? Pick the top one???
                    Debug.Assert(cell.BackgroundGameObject == null);
                    continue;
                }
                cell.BackgroundGameObject = obj;
                if (obj != null)
                {
                    var wp = obj.GetComponent<Waypoint>();
                    if (wp != null)
                    {
                        if (wp.WaypointType == Waypoint.WaypointTypes.Start)
                        {
                            StartWaypoint = wp;
                            wp.GridPoint = cell.GridPoint; // new GridPoint(cell.GridPoint.X, cell.GridPoint.Y); // Lame. Why doesn't the component know it's gridpoint?
                            cell.GroundType = GameCell.GroundTypes.Start;
                        }
                        else if (wp.WaypointType == Waypoint.WaypointTypes.End)
                        {
                            EndWaypoint = wp;
                            EndWaypoint.GridPoint = cell.GridPoint; // new GridPoint(cell.GridPoint.X, cell.GridPoint.Y); // Lame. Why doesn't the component know it's gridpoint?
                            cell.GroundType = GameCell.GroundTypes.End;
                        }
                    }
                    else
                    {
                        cell.GroundType = GameCell.GroundTypes.Dirt;
                    }
                }
                else
                {
                    cell.GroundType = GameCell.GroundTypes.Path;
                }
            }
            else
            {
                Debug.Assert(false, "Coords not on map.");
            }


        }
        Debug.Assert(StartWaypoint != null, "Did not find start.");
        Debug.Assert(EndWaypoint != null, "Did not find end.");


    }

    public GameCell MapPositionToGameCellOrNull(Vector3 position)
    {
        var dims = CalcMapCoords(position);
        if (dims.x < Cells.GetLength(0) && dims.y < Cells.GetLength(1) && dims.x >= 0 && dims.y >= 0)
        {
            var cell = Cells[(int) dims.x, (int) dims.y];
            return cell;
        }
        return null;
    }

    private Vector2 CalcMapCoords( Vector3 position)
    {

        return new Vector2(Mathf.RoundToInt( position.x) - Mathf.RoundToInt(_mapInternalGrid.min.x),
            Mathf.RoundToInt(position.y) - Mathf.RoundToInt(_mapInternalGrid.min.y));
    }


    private static Vector2 CalcMapDims(Rect b)
    {
        Vector3 end = b.max;
        Vector3 start = b.min;

        int ww = (int) end.x - (int) start.x;
        int hh = (int) end.y - (int) start.y;
        return new Vector2(ww, hh);
    }

    private static GameObject[] GetSceneObjects()
    {
        return Resources.FindObjectsOfTypeAll<GameObject>()
            .Where(go => go.transform.hideFlags == HideFlags.None).ToArray();
    }

    public static List<GameObject> GetObjectsInLayer(int layer)
    {
        var objects = GetSceneObjects();

        List<GameObject> Selected = new List<GameObject>();
        foreach (GameObject t in objects)
        {
            if (t.layer == layer)
            {
                Selected.Add(t);
            }
        }
        return Selected;

    }

    public byte[,] ToByteGrid()
    {
        var w = 32; // Must be power of 2. Cells.GetLength(0);
        var h = 32; // Must be power of 2. Cells.GetLength(1);
        var grid = new byte[w, h];
        for (int xx = 0; xx < w; xx++)
        {
            for (int yy = 0; yy < h; yy++)
            {
                if (yy >= Cells.GetLength(1) || xx >= Cells.GetLength(0))
                {
                    grid[xx, yy] = CELL_BLOCKED; // Assume outside area is blocked
                }
                else
                {
                    grid[xx, yy] = CalcCellValue(xx, yy);
                }
            }
        }
        return grid;
    }

    private const byte CELL_OPEN = 1;
    private const byte CELL_BLOCKED = 0;

    private byte CalcCellValue(int xx, int yy)
    {
        var cell = Cells[xx, yy];
        if (cell.IsStart || cell.IsEnd)
        {
            return CELL_OPEN;
        }
        return (cell.BackgroundGameObject != null) ? (byte) CELL_BLOCKED : (byte) CELL_OPEN;
    }

    readonly Vector3 _textOffset = new Vector3(0.25f, -0.25f, 0f);

    public void DrawTextAtPoint(GridPoint nodeGridPoint, string s)
    {
        var vector = MapGridPointToVector(nodeGridPoint) - _textOffset;
        var color = GUI.color;
        GUI.color = Color.green;
        Handles.Label(vector, s);
        GUI.color = color;
    }

    public GridPoint MapVectorToGridPoint(Vector2 vv)
    {
        var origin = _mapInternalGrid.min;
        var p = new GridPoint(Mathf.RoundToInt(vv.x - origin.x), Mathf.RoundToInt(vv.y - origin.y));
        return p;
    }

    private Vector3 MapGridPointToVector(GridPoint nodeGridPoint)
    {
        var origin = _mapInternalGrid.min;
        origin.x += nodeGridPoint.X;
        origin.y += nodeGridPoint.Y;
        return origin;
    }

    public GameCell GetStartGameCell()
    {
        return Cells[StartWaypoint.GridPoint.X, StartWaypoint.GridPoint.Y];
    }

    public GameCell GetEndGameCell()
        {
        return Cells[EndWaypoint.GridPoint.X, EndWaypoint.GridPoint.Y];
        }

    // Bad idea.
    public GameCell RandomGameCell(GameCell.GroundTypes OfGroundType)
    {
        var list = new List<GameCell>();
        for (int row = 0; row < Cells.GetLength(0); row++)
        {
            for (int col=0; col < Cells.GetLength(1); col++)
            {
                if (Cells[row, col].GroundType == OfGroundType)
                {
                    list.Add(Cells[row,col]);
                }
            }
        }
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public GameCell GetNextPathGameCell(GameCell curGameCell, PathFollower forFollower)
    {
        var path = forFollower.Path;
        if (path == null)
        {
            return null;
        }

        var curGridPoint = curGameCell.GridPoint;

        for (int ii = 0; ii < path.Count; ii++)
        {
            var cel = forFollower.Path[ii];
            if (cel.X == curGridPoint.X && cel.Y == curGridPoint.Y)
            {
                PathFinderNode node;
                if (ii < 1)
                {
                    return null;
                }
                node = path[ii - 1];
                return Cells[node.X, node.Y];
            }
        }
        return null;

    }

    public class GameCell
    {
        public enum GroundTypes
        {
            Dirt,
            Water,
            Rock,
            Tree,
            Path,
            Start,
            End
        }

        public GroundTypes GroundType;

        /// <summary>
        /// Towers or Squares that block enemies.
        /// </summary>
        public GameObject BackgroundGameObject;

        public List<Enemy> Enemies = new List<Enemy>();

        public bool IsStart
        {
            get { return GroundType == GroundTypes.Start; }
        }

        public bool IsEnd
        {
            get { return GroundType == GroundTypes.End; }
        }

        public GridPoint GridPoint;

        public bool IsBlocked()
        {
            bool open = this.GroundType == GroundTypes.Path || this.GroundType == GroundTypes.Start || this.GroundType == GroundTypes.End;
            return !open;
        }
    }

    public void MoveWayPointToGridPoint( Waypoint wp, GridPoint dst)
    {
        wp.GridPoint = dst;
        wp.GetComponent<Transform>().position = MapGridPointToVector(dst);
    }

    public void RandomizeEndCell()
    {
         MoveWayPointToGridPoint(EndWaypoint, RandomGameCell(GameCell.GroundTypes.Dirt).GridPoint);
    }

    public void MoveStartToEnd()
    {
        MoveWayPointToGridPoint( StartWaypoint, EndWaypoint.GridPoint);
    }
}


