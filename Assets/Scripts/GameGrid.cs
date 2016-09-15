using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class GameGrid : MonoBehaviour
    {
        private GameObject _map;

        public GameObject GetMap()
        {
            return _map;
        }

        public GameObject _selector;
        public GameObject _dragBox;
        public GameObject _disallowed;

        public GameObject GetSelector()
        {
            return _selector;
        }
        public GameObject GetDragBox()
        {
            return _dragBox;
        }
        public GameObject GetDisallowed()
        {
            return _disallowed;
        }
        public const int BACKGROUND_LAYER = 8;
        public const int TOWER_LAYER = 9;
        public const int ENEMY_LAYER = 10;
        public const int BULLET_LAYER = 11;
        public const int ROBOT_LAYER = 13;
        public const int DRAG_LAYER = 16;
        public const int HUMAN_LAYER = 17;

        public GameCell[,] Cells;
        private Waypoint StartWaypoint;
        private Waypoint EndWaypoint;
        private Rect _mapInternalGrid;
        private Vector2 _mapDims;

        public Vector3 MapScreenToMapPosition(Vector3 screenPos, out bool isOffScreen)
        {
            var pos2 = Camera.main.ScreenToWorldPoint(screenPos);
            pos2.z = 0;
            isOffScreen = !(_mapInternalGrid.Contains(pos2));
            return pos2;
        }

        public GameCell MapScreenToGameCellOrNull(Vector3 screenPos)
        {
            var pos2 = Camera.main.ScreenToWorldPoint(screenPos);
            pos2.z = 0;
            var cell = MapPositionToGameCellOrNull(pos2);
            return cell;

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

        /// <summary>
        /// From current cell, find a path that goes through the remaning waypoints to the end.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="remainingWaypoints">Last point is the end.</param>
        /// <returns></returns>
        public List<GameCell> FindPathWithWaypoints(GameCell current, List<GameCell> orderedWayPointsGameCells, GameCell targetGameCell)
        {
            var remainingWaypoints = new List<GameGrid.GameCell>();
            remainingWaypoints.AddRange(orderedWayPointsGameCells);
            remainingWaypoints.Add(targetGameCell); // TBD: Need more


            if (current == null || remainingWaypoints == null || remainingWaypoints.Count == 0)
            {
                return new List<GameCell>(); // TBD: Lame.
            }

            var fullPath = new List<GameCell>();
            var firstLeg = FindPath(current, remainingWaypoints[0]);
            if (firstLeg.Count == 0)
            {
                // No path
                return new List<GameCell>(); 
            }
            fullPath.AddRange(firstLeg);
            for (int ii = 0; ii < remainingWaypoints.Count - 1; ii++)
            {
                var nextLeg = FindPath(remainingWaypoints[ii], remainingWaypoints[ii + 1]);
                if (nextLeg != null)
                {
                    fullPath.AddRange(nextLeg);
                }
                else
                {
                    break;
                }
            }
            // TBD: There may be some overlap we need to fix
            return fullPath;
        }

        private List<GameCell> FindPath(GameCell start, GameCell end)
        {
            if (start == null || end == null)
            {
                return new List<GameCell>(); // TBD: Lame.
            }

            var byteGrid = ToByteGrid();

            var pf = new PathFinderFast(byteGrid)
            {
                Diagonals = true,
                Formula = HeuristicFormula.DiagonalShortCut,
                HeuristicEstimate = 2,
                HeavyDiagonals = true,
                PunishChangeDirection = false,
                SearchLimit = 15000,
                TieBreaker = false
            };
            var path = pf.FindPath(start.GridPoint, end.GridPoint);
            return MakeGameCellPath(path);
        }

        private List<GameCell> MakeGameCellPath(List<PathFinderNode> path)
        {
            var list = new List<GameCell>();

            if (path != null)
            {
                foreach (var node in path)
                {
                    var gameCell = Cells[node.X, node.Y];
                    list.Add(gameCell);

                }
            }
            return list;

        }



        // Update is called once per frame
        void Update()
        {

            if ( !Application.isPlaying)
            {
                InitCellMapFromLevelMap(_map);
            }
            //var start = this.MapGridPointToCell( MapVectorToGridPoint(this.StartWaypoint.transform.position));
            // var end = this.MapGridPointToCell(MapVectorToGridPoint(this.EndWaypoint.transform.position));

            //FindPath(start, end);
        }

        public void Start()
        {
            _map = FindActiveMap().gameObject;
            _selector = FindSelector().gameObject;

            // Initialize connection to others.
            InitCellMapFromLevelMap(_map);
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

        public static void SetCellEntity(GameCell cell, GameObject go, Entity.EntityClasses entityClass)
        {

            switch (entityClass)
            {
                case Entity.EntityClasses.Background:
                case Entity.EntityClasses.Waypoint:
                
                    cell.Background = go;
                    break;
                
                case Entity.EntityClasses.Enemy:
                    cell.Enemies.Merge(go.GetComponent<Enemy>());
                    break;
                case Entity.EntityClasses.Human:
                    cell.Humans.Merge(go.GetComponent<Human>());
                    break;
                case Entity.EntityClasses.Robot:

                    cell.Robot = go.GetComponent<Robot>();
                    break;
                case Entity.EntityClasses.Tower:
                    cell.Tower = go.GetComponent<Tower>();
                    break;
                default:
                    Debug.Assert(false, "Unsupported entity type.");
                    break;

            }
        }

        public static void RemoveEntity(GameCell cell, GameObject go, Entity.EntityClasses entityClass)
        {

            switch (entityClass)
            {
                case Entity.EntityClasses.Background:
                case Entity.EntityClasses.Waypoint:

                    cell.Background = null;
                    break;

                case Entity.EntityClasses.Enemy:
                    cell.Enemies.Remove(go.GetComponent<Enemy>());
                    break;
                case Entity.EntityClasses.Human:
                    cell.Humans.Remove(go.GetComponent<Human>());
                    break;
                case Entity.EntityClasses.Robot:

                    cell.Robot = null;
                    break;
                case Entity.EntityClasses.Tower:
                    cell.Tower = null;
                    break;
                default:
                    Debug.Assert(false, "Unsupported entity type.");
                    break;

            }
        }


        public void InstaniatePrefabAtGameCell(GameObject prefab, GameCell cell)
        {
            var entity = prefab.GetComponent<Entity>();

            var newGameObject = Instantiate(prefab);
            newGameObject.transform.SetParent(_map.transform);
            newGameObject.transform.position = MapGridPointToPosition(cell.GridPoint);

            SetCellEntity(cell, prefab, entity.EntityClass);

            var snap = newGameObject.GetComponent<SnapToGrid>();
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

            var blocks = GetActiveObjectsInLayer(BACKGROUND_LAYER);
            AddToMap(blocks);

            Debug.Assert(StartWaypoint != null, "Did not find start.");
            Debug.Assert(EndWaypoint != null, "Did not find end.");

            var towers = (GetActiveObjectsInLayer(TOWER_LAYER));
            AddToMap(towers);

            var robots = (GetActiveObjectsInLayer(ROBOT_LAYER));
            AddToMap(robots);

            var humans = (GetActiveObjectsInLayer(HUMAN_LAYER));
            AddToMap(humans);

        }

        public void AddToMap( IList<GameObject> entities)
        {

        // Walk thru the grid and figure out terrain type for each
        //  block.
        // This is mostly, so I can:
        //  1) Do a path search to find out where units can go.
        //  2) Place targets and obstacles created by designer.
        //  3) Place any initial items on the map.
        foreach (var obj in entities)
        {
            var entity = obj.GetComponent <Entity>();
                var cell = MapPositionToGameCellOrNull(obj.transform.position);
                if( cell != null)
                {
                    SetCellEntity(cell, obj, entity.EntityClass); // TBD: Could just pass entity for last two params.
                    if (entity.EntityClass == Entity.EntityClasses.Waypoint)
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
                            else
                            {
                                wp.GridPoint = cell.GridPoint;
                                cell.GroundType = GameCell.GroundTypes.Waypoint;
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
           

        }

        public GameCell MapGridPointToGameCellOrNull(GridPoint gp)
        {
            if (Cells == null)
            {
                return null;
            }
            if (gp.X < Cells.GetLength(0) && gp.Y < Cells.GetLength(1) && gp.X >= 0 && gp.Y >= 0)
            {
                var cell = Cells[gp.X, gp.Y];
                return cell;
            }
            return null;

        }

        public GameCell MapPositionToGameCellOrNull(Vector3 position)
        {
            var gp = MapPositionToGridPoint(position);
            return MapGridPointToGameCellOrNull(gp);
        }

        private GridPoint MapPositionToGridPoint( Vector3 position)
        {

            return new GridPoint(Mathf.RoundToInt( position.x) - Mathf.RoundToInt(_mapInternalGrid.min.x),
                Mathf.RoundToInt(position.y) - Mathf.RoundToInt(_mapInternalGrid.min.y));
        }

        public Vector3 CalcExactMapCoords(Vector3 position)
        {
            return new Vector2(position.x - Mathf.RoundToInt(_mapInternalGrid.min.x),
                position.y - Mathf.RoundToInt(_mapInternalGrid.min.y));
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

        public static EditorGrid FindActiveMap()
        {
            return
                Resources.FindObjectsOfTypeAll<EditorGrid>()
                    .Where(go => go.gameObject.activeInHierarchy && go.gameObject.name == "Map")
                    .ToList()
                    .FirstOrDefault();
        }

        private static MouseInput FindSelector()
        {
            return
                Resources.FindObjectsOfTypeAll<MouseInput>()
                    .Where(go => go.gameObject.activeInHierarchy && go.gameObject.name == "Selector")
                    .ToList()
                    .FirstOrDefault();
        }


        public static List<GameObject> GetActiveObjectsInLayer(int layer)
        {
            var objects = GetSceneObjects();

            List<GameObject> Selected = new List<GameObject>();
            foreach (GameObject t in objects)
            {
                if (t.activeInHierarchy && t.layer == layer)
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
            if (cell.IsStart || cell.IsEnd || cell.IsWaypoint) // TBD: Waypoint takes up whole cell. 
            {
                return CELL_OPEN;
            }
            return (cell.Background != null) ? (byte) CELL_BLOCKED : (byte) CELL_OPEN; // TBD: Other things should block
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

        public Vector3 MapGridPointToVector(GridPoint nodeGridPoint)
        {
            var origin = _mapInternalGrid.min;
            origin.x += nodeGridPoint.X;
            origin.y += nodeGridPoint.Y;
            return origin;
        }

        // Bad idea.
        public GameCell RandomGameCell(GameCell.GroundTypes OfGroundType)
        {
            if (Cells == null)
            {
                return null;
            }

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
            if (list.Count <= 0)
            {
                Debug.Assert(list.Count > 0);
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
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
                End, 
                Waypoint
            }

            public GroundTypes GroundType;

            /// <summary>
            /// Towers or Squares that block enemies.
            /// </summary>
            public GameObject Background; // TBD: Need a base type

            public List<Human> Humans = new List<Human>();
            public List<Enemy> Enemies = new List<Enemy>();
            public Tower Tower;
            public Robot Robot;

        
            public bool IsStart
            {
                get { return GroundType == GroundTypes.Start; }
            }

            public bool IsEnd
            {
                get { return GroundType == GroundTypes.End; }
            }

            public bool IsWaypoint
            {
                get { return GroundType == GroundTypes.Waypoint; }

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

        public GameCell GetSelectorCellOrNull()
        {
            var worldPos = this.GetSelector().transform.position;
            worldPos.z = -10;
            GameCell dropCell = this.MapPositionToGameCellOrNull(worldPos);
            return dropCell;
        }
    }
}