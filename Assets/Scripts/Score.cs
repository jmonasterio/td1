using UnityEngine;
using System.Collections;
using UnityEditor;

public class Score : MonoBehaviour {
    private SpriteRenderer _renderer;

    // Use this for initialization
	void Start ()
	{
	    _renderer = this.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
    private void OnGUI()
    {
        var vector = new Vector3(_renderer.bounds.min.x, _renderer.bounds.max.y, _renderer.bounds.min.z) +
                     new Vector3(0.5f, -0.5f, 0); 
        var color = GUI.color;
        GUI.color = Color.black;
        float lineSpacing = 0.45f;
        Handles.Label(vector, "Score: " + Toolbox.Instance.GameManager.ScoreController.Score);
        vector.Set(vector.x, vector.y- lineSpacing, vector.z);
        Handles.Label(vector, "Build Score: " + Toolbox.Instance.GameManager.ScoreController.BuildScore);
        vector.Set(vector.x, vector.y - lineSpacing, vector.z);
        Handles.Label(vector, "Grow Score: " + Toolbox.Instance.GameManager.ScoreController.GrowScore);
        vector.Set(vector.x, vector.y - lineSpacing, vector.z);
        Handles.Label(vector, "Grow rate: " + Toolbox.Instance.GameManager.ScoreController.GrowRate);
        GUI.color = color;
    }
}

