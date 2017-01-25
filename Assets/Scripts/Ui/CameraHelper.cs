using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class CameraHelper : MonoBehaviour
    {
        public const float DEFAULT_WIDTH = 1920f;
        public const float DEFAULT_HEIGHT = 1080;

        public const float DEFAULT_CELL_WIDTH = 24f; // Assume default of 80px per cell.
        public const float DEFAULT_CELL_HEIGHT = 13.5f;

        const float DEFAULT_ORTHOGRAPHIC_SIZE = (DEFAULT_CELL_HEIGHT / (2f) * DEFAULT_CELL_WIDTH);

        private const float DEFAULT_ASPECT = DEFAULT_WIDTH / DEFAULT_HEIGHT;


        private int _lastScreenWidth = 0;
        private int _lastScreenHeight = 0;

        public void Awake()
        {
        }

        /*
         * Camera Size = y / (2 * s)

Where:
y = Screen Height (px)
s = Desired Height of Photoshop Square (px)

    */
        public void Update()
        {
            // Note: Camera.main.aspect is one frame out of date.

            if ( Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
            {

                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;

                float screenAspectRatio = ((float)Screen.width / (float)Screen.height);
                if (screenAspectRatio > DEFAULT_ASPECT)
                {
                    // Handles bars on sides for 1800x900, for example.
                    screenAspectRatio = DEFAULT_ASPECT;
                }


                // Pixels per world-unit. 24x13.5 cells.
                Camera.main.orthographicSize =  DEFAULT_ORTHOGRAPHIC_SIZE * (DEFAULT_ASPECT/screenAspectRatio);

            }

        }

    }
}
