using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Income : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    private void OnGUI()
    {
        return; // ONGUITEST

        if (Event.current.type == EventType.Repaint)
        {
            var gameGrid = Toolbox.Instance.GameManager.LevelController.CurrentLevel.GameGrid;

            var income = Toolbox.Instance.GameManager.GetComponent<ScoreController>().BuildScore;
            var vector = this.transform.position;
            gameGrid.DrawTextAtVector(vector, "Income: " + income, Color.blue);
        }

    }
}
