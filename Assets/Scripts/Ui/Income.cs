using UnityEngine;
using System.Collections;
using UnityEditor;

public class Income : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    private void OnGUI()
    {

        var income = Toolbox.Instance.GameManager.GetComponent<ScoreController>().BuildScore;
        var vector = this.transform.position;
        var color = GUI.color;
        GUI.color = Color.black;
        Handles.Label(vector, "Income: " + income);
        GUI.color = color;

    }
}
