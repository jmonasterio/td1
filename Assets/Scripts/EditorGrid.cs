﻿using System.Collections;
using Algorithms;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class EditorGrid : MonoBehaviour {

        // Use this for initialization
        void Start () {
	
        }

        // Update is called once per frame
        void OnGUI()
        {
            if (!Application.isPlaying || Toolbox.Instance.DebugSys.ShowGridAtRuntime )
            {
                var rcInternalGrid = GridHelper.GetInternalGridRect(this.gameObject);

                DrawGridAroundIntegerPoints(rcInternalGrid);
                DrawGridCoordinateText(rcInternalGrid);
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
            var color = GUI.color;
            GUI.color = Color.green;
            Handles.Label(vector, s);
            GUI.color = color;
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