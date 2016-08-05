using UnityEngine;
using System.Collections;

public class PaletteController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
        // Enable all items whose cost is less than the income we have.
	    var scoreController = Toolbox.Instance.GameManager.GetComponent<ScoreController>();
	    foreach (var child in this.gameObject.GetComponentsInChildren<DragSpawner>(includeInactive:true))
	    {
	        int incomeCost = child.SpawnPF.GetComponent<Entity>().IncomeCost;
            bool enable = incomeCost < scoreController.Income;
            child.gameObject.SetActive(enable);
	    }
	}
}
