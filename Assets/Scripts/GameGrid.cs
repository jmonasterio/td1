using System.Collections.Generic;
using System.Linq;
using Algorithms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[ExecuteInEditMode]
    public class GameGrid : MonoBehaviour
    {
        public GameObject Map;

        private List<PathFinderNode> _pathNodeList;

        public const int BACKGROUND_LAYER = 8;

        public GameCell[,] Cells;
        private Waypoint StartWaypoint;
        private Waypoint EndWaypoint;
        private Rect _mapInternalGrid;
        private Vector2 _mapDims;

        public Vector3? MapScreenToGridCellsOrNull(Vector2 screenPos)
        {
            var pos2 = Camera.main.ScreenToWorldPoint(screenPos);
            pos2.z = 0;
            if (_mapInternalGrid.Contains(pos2))
            {
                return pos2;
            }
            return null;
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
                    Cells[ii, jj] = new GameCell();
                }
            }
        }

    public void FindPath()
    {
        var byteGrid = ToByteGrid();

        var pf = new PathFinderFast(byteGrid);
        pf.Diagonals = true;
        pf.Formula = HeuristicFormula.DiagonalShortCut;
        pf.HeuristicEstimate = 2;
        pf.HeavyDiagonals = true;
        pf.PunishChangeDirection = false;
        pf.SearchLimit = 5000;
        pf.TieBreaker = false;
        var start = MapVectorToPoint(this.StartWaypoint.transform.position);
        var end = MapVectorToPoint(this.EndWaypoint.transform.position);
        _pathNodeList = pf.FindPath(start, end);

    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        FillFromHeirarchy(Map);
#endif
        FindPath();
    }

    public void Start()
    {
        // Initialize connection to others.
        FillFromHeirarchy(Map);
    }

    void OnSceneGUI()
    {
        OnGUI();
    }

    void OnGUI()
    {
        if (_pathNodeList != null)
        {
            foreach (var node in _pathNodeList)
            {
                // Draw lines between the nodes, just to see.
                //Debug.DrawLine( );
                var nodePoint = new Point(node.X, node.Y);
                if (Toolbox.Instance.DebugSys.ShowXOnPath)
                {
                     DrawTextAtPoint(nodePoint, "X");
                }
            }
        }
        else
        {
            Debug.Assert(false, "No path found");
        }

    }


    public void FillFromHeirarchy(GameObject map)
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
                var dims = CalcMapCoords(map, obj.transform.position);
                if (dims.x < Cells.GetLength(0) && dims.y < Cells.GetLength(1) && dims.x >= 0 && dims.y >= 0)
                {
                    var cell = Cells[(int) dims.x, (int) dims.y];
                    if (cell.BackgroundGameObject != null)
                    {
                        // There may be more than one shape on a square? What to do? Pick the top one???
                        Debug.Assert(cell.BackgroundGameObject == null);
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
                                cell.GroundType = GameCell.GroundTypes.Start;
                            }
                            else if (wp.WaypointType == Waypoint.WaypointTypes.End)
                            {
                                EndWaypoint = wp;
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

    private Vector2 CalcMapCoords(GameObject map, Vector3 position)
        {
            
            return new Vector2( (int) position.x - (int) _mapInternalGrid.min.x, (int) position.y -(int) _mapInternalGrid.min.y);
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

        private static List<GameObject> GetObjectsInLayer(int layer)
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
            var grid = new byte[w,h];
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
                        grid[xx, yy] = CalcCellValue(xx,yy);
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

        public void DrawTextAtPoint(Point nodePoint, string s)
        {
            var vector = MapPointToVector( nodePoint)-new Vector3(0.25f,-0.25f, 0f);
            var color = GUI.color;
            GUI.color = Color.green;
            Handles.Label(vector, s);
            GUI.color = color;
        }

        public Point MapVectorToPoint(Vector2 vv)
        {
            var origin = _mapInternalGrid.min;
            var p = new Point(Mathf.RoundToInt(vv.x - origin.x), Mathf.RoundToInt(vv.y - origin.y));
            return p;
        }

        private Vector3 MapPointToVector(Point nodePoint)
        {
            var origin = _mapInternalGrid.min;
            origin.x += nodePoint.X;
            origin.y += nodePoint.Y;
            return origin;
        }
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

    public GroundTypes GroundType { get; set; }
    public GameObject BackgroundGameObject { get; set; }


    public bool IsStart
    {
        get { return GroundType == GroundTypes.Start; }
    }

    public bool IsEnd
    {
        get { return GroundType == GroundTypes.End; }
    }
}


