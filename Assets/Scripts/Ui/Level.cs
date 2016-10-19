using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
public class Level : MonoBehaviour
{

    public string LevelName;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // So we see in EDITOR, too.
    void OnSceneGUI()
        {
        OnGUI();
        }


    // Update is called once per frame
    void OnGUI()
        {
        var color = GUI.color;
        GUI.color = Color.green;

        GUIStyle myStyle = new GUIStyle(GUI.skin.GetStyle("label"));
        myStyle.fontSize = 32;

        Handles.Label(this.transform.position + new Vector3(-5.0f, +10.0f, 0f), "" + this.LevelName, myStyle);
        GUI.color = color;
        }
    }
