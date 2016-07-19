using UnityEngine;
using System.Collections;
using UnityEditor;

public class Health : MonoBehaviour {

    public int Lives = 3;
    public Vector3 labelOffset = new Vector3(0, -1.5f, 0f);

    // Use this for initialization
    void Start () {
	
	}

    void OnSceneGUI()
    {
        OnGUI();
    }


    // Update is called once per frame
    void OnGUI () {
        var color = GUI.color;
        GUI.color = Color.green;
        Handles.Label(this.transform.position - labelOffset, "" + this.Lives);
        GUI.color = color;
    }


}
