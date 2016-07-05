using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {

    // Inspectors
    public GameObject Map;
    public string GameName;
    public GameGrid GameGrid;

    // State
  
    // Use this for initialization
    void Start () {
        Toolbox.Instance.GameManager = this;

        Toolbox.Instance.GameManager.GameGrid = new GameGrid();
        Toolbox.Instance.GameManager.GameGrid.FillFromHeirarchy(Map);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

   
}
