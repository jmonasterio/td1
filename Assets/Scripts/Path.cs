using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class Path : Editor {


    void OnGUI()
    {
        if (this.GetInstanceID() == Selection.activeInstanceID)
        {
            Debug.LogWarning("in match.");
        }
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
