using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class PaletteController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
        // Enable all items whose cost is less than the income we have.
	    var scoreController = Toolbox.Instance.GameManager.ScoreController;
	    foreach (var child in this.gameObject.GetComponentsInChildren<DragSpawner>(includeInactive:true))
	    {
	        float incomeCost = child.SpawnPF.GetComponent<Entity>().BuildValue;
            bool enable = incomeCost <= scoreController.BuildScore;
            child.gameObject.SetActive(enable);
	    }
	}
}
