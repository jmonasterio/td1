using System.Collections;
using Algorithms;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class EditorGrid : MonoBehaviour {

        // Use this for initialization
        void Awake ()
        {
            //CameraHelper.main.orthographicSize = 11.0f/(Screen.height*Screen.width);
        }

        // Update is called once per frame
        void OnGUI()
        {
            return; // ONGUITEST

            if (Event.current.type == EventType.Repaint)
            {

                if (!Application.isPlaying || Toolbox.Instance.DebugSys.ShowGridAtRuntime)
                {
                    var rcInternalGrid = GridHelper.GetInternalGridRect(this.gameObject);

                    DrawGridAroundIntegerPoints(rcInternalGrid);
                    DrawGridCoordinateText(rcInternalGrid);
                }
            }
        }

        private void DrawGridCoordinateText(Rect rcInternalGrid)
        {
            DrawTextAtPoint( rcInternalGrid, new GridPoint(0,0), "0,0" );
            DrawTextAtPoint(rcInternalGrid, new GridPoint(1, 1), "1,1");
        }

        private void DrawTextAtPoint(Rect rcInternalGrid, GridPoint nodeGridPoint, string s)
        {
            var vector = MapGridPointToVector(rcInternalGrid, nodeGridPoint);
            GuiExtension.GuiLabel(vector, s, Color.green);
        }

        public Vector3 MapPositionToScreen(Vector3 pos)
        {
            var pos2 = Camera.main.WorldToScreenPoint(pos);
            pos2.z = 0;
            return pos2;
        }


        public GridPoint MapVectorToGridPoint(Rect rcInternalGrid, Vector2 vv)
        {
            var origin = rcInternalGrid.min;
            var p = new GridPoint(Mathf.RoundToInt(vv.x - origin.x), Mathf.RoundToInt(vv.y - origin.y));
            return p;
        }

        private Vector3 MapGridPointToVector(Rect rcInternalGrid, GridPoint nodeGridPoint)
        {
            var origin = rcInternalGrid.min;
            origin.x += nodeGridPoint.X;
            origin.y += nodeGridPoint.Y;
            return origin;
        }

        private void DrawGridAroundIntegerPoints(Rect rcInternalGrid)
        {
            for (int xx = (int)rcInternalGrid.xMin; xx <= (int) rcInternalGrid.xMax; xx++)
            {
                Vector3 ss = new Vector3(xx - 0.5f, (rcInternalGrid.yMin - 0.5f), 0);
                Vector3 ee = new Vector3(xx - 0.5f, (rcInternalGrid.yMax - 0.5f), 0);

                Debug.DrawLine(ss, ee, Color.green, 0.1f);
            }

            for (int yy = (int)rcInternalGrid.yMin; yy <= (int) rcInternalGrid.yMax; yy++)
            {
                Vector3 ss = new Vector3(rcInternalGrid.xMin -0.5f, yy - 0.5f, 0);
                Vector3 ee = new Vector3(rcInternalGrid.xMax -0.5f, yy - 0.5f, 0);

                Debug.DrawLine(ss, ee, Color.green, 0.1f);
            }
            //Debug.DrawLine( start, end, Color.magenta, 2f);
            //Debug.DrawLine( new Vector3(0,0,0), end, Color.red );

        }
    }
}
