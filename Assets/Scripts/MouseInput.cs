using UnityEngine;
using System.Collections;
using System.Net;

public class MouseInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.mousePresent)
	    {
            var pos = Input.mousePosition;
            Debug.Log(pos);
            pos.z = -5;

	        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
	        if (gameGrid != null)
	        {
	            var pos2 = gameGrid.MapScreenToGridCellsOrNull(pos);
	            if (pos2.HasValue)
	            {
	                var pos3 = pos2.Value;
	                pos3.z = 0;
	                //Debug.Log(pos3);
	                this.transform.position = pos3;
	            }
	        }

	    }
	}
}
