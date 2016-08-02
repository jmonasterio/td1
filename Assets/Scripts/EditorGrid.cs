using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public class EditorGrid : MonoBehaviour {

        // Use this for initialization
        void Start () {
	
        }

        // Update is called once per frame
        void Update()
        {
            if (!Application.isPlaying || Toolbox.Instance.DebugSys.ShowGridAtRuntime )
            {
                var rcInternalGrid = GridHelper.GetInternalGridRect(this.gameObject);

                DrawGridAroundIntegerPoints(rcInternalGrid);
            }
        }

        private void DrawGridAroundIntegerPoints(Rect rcInternalGrid)
        {
            for (int xx = (int)rcInternalGrid.xMin; xx <= rcInternalGrid.xMax; xx++)
            {
                Vector3 ss = new Vector3(xx - 0.5f, (rcInternalGrid.yMin - 0.5f), 0);
                Vector3 ee = new Vector3(xx - 0.5f, (rcInternalGrid.yMax - 0.5f), 0);

                Debug.DrawLine(ss, ee, Color.green, 0.1f);
            }

            for (int yy = (int)rcInternalGrid.yMin; yy <= rcInternalGrid.yMax; yy++)
            {
                Vector3 ss = new Vector3(rcInternalGrid.xMin - 0.5f, yy - 0.5f, 0);
                Vector3 ee = new Vector3(rcInternalGrid.xMax - 0.5f, yy - 0.5f, 0);

                Debug.DrawLine(ss, ee, Color.green, 0.1f);
            }
            //Debug.DrawLine( start, end, Color.magenta, 2f);
            //Debug.DrawLine( new Vector3(0,0,0), end, Color.red );

        }
    }
}