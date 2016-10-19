﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour {

#if UNITY_EDITOR
    public bool snapToGrid = true;
    public float snapValue = 1f;

    public bool sizeToGrid = false;
    public float sizeValue = 0.25f;

    // Adjust size and position
    void Update()
    {
        if (snapToGrid)
        {
            var newPos = RoundTransform(transform.position, snapValue);
            if (newPos != transform.position)
            {
                transform.position = newPos;
                //UnityEngine.Debug.Log("Snapped!");
            }
        }
        if (sizeToGrid)
        {
            transform.localScale = RoundTransform(transform.localScale, sizeValue);
        }
    }

    // The snapping code
    public static Vector3 RoundTransform(Vector3 v, float snapValue)
    {
        return new Vector3
        (
            snapValue * Mathf.Round(v.x / snapValue),
            snapValue * Mathf.Round(v.y / snapValue),
            v.z
        );
    }
#endif
}
