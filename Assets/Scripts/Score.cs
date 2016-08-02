using UnityEngine;
using System.Collections;
using UnityEditor;

public class Score : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    private void OnGUI()
    {
        var score = Toolbox.Instance.GameManager.GetComponent<ScoreController>().Score;
        var vector = this.transform.position;
        var color = GUI.color;
        GUI.color = Color.green;
        Handles.Label(vector, ""+score);
        GUI.color = color;
    }
}

