using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
#if UNITY_EDITOR
        Handles.Label(vector, "Income: " + income);
#endif
        GUI.color = color;

    }
}
