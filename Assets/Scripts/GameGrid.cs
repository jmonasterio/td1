using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    public class GameGrid
    {

        public GameCell[,] Cells;
        private Bounds _mapRect;
        private Vector2 _mapDims;

        public Vector3? MapScreenToGridCellsOrNull(Vector2 screenPos)
        {
            var pos2 = Camera.main.ScreenToWorldPoint(screenPos);
            pos2.z = 0;
            if (_mapRect.Contains(pos2))
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

        public void FillFromHeirarchy(GameObject map)
        {
            _mapRect = GetMapRect(map);
            _mapDims = CalcMapDims(_mapRect);
            InitGameGrid(_mapDims);

            const int BACKGROUND_LAYER = 8;
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
                var cell = Cells[(int) dims.x, (int) dims.y];
                if (cell.BackgroundGameObject != null)
                {
                    //Debug.Assert(cell.BackgroundGameObject == null);
                }
                cell.BackgroundGameObject = obj;

            }


        }

        private Vector2 CalcMapCoords(GameObject map, Vector3 position)
        {
            
            return new Vector2( (int) position.x - (int) _mapRect.min.x, (int) position.y -(int) _mapRect.min.y);
        }


        private static Vector2 CalcMapDims(Bounds b)
        {
            Vector3 end = b.max;
            Vector3 start = b.min;

            int ww = (int) end.x - (int) start.x;
            int hh = (int) end.y - (int) start.y;
            return new Vector2(ww, hh);
        }

        private static Bounds GetMapRect(GameObject map)
        {


            var rect = map.GetComponent<SpriteRenderer>().bounds;
            return rect;

            /*
                        var mapTrans = map.transform;
            var rect = mapTrans.localScale;
            return rect;
            Vector3 start = rect.min;
            Vector3 end = rect.max;

            for (int xx = (int)start.x; xx <= (int)end.x; xx++)
            {
                Vector3 ss = new Vector3(xx - 0.5f, (int)start.y, 0);
                Vector3 ee = new Vector3(xx - 0.5f, (int)end.y, 0);

                Debug.DrawLine(ss, ee, Color.green, 0.1f);
            }
    */

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
    }

    public class GameCell
    {
        public enum GroundTypes
        {
            Dirt,
            Water,
            Rock,
            Tree,
            Path
        }

        public GroundTypes GroundType { get; set; }
        public GameObject BackgroundGameObject { get; set; }



    }
