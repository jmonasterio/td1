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
            pos.z = -5;

	        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
	        if (gameGrid != null)
	        {
	            var pos2 = gameGrid.MapScreenToMapPositionOrNull(pos);
	            if (pos2.HasValue)
	            {
	                var pos3 = pos2.Value;
	                pos3.z = 0;
	                //Debug.Log(pos3);
	                this.transform.position = pos3;
	            }
	            else
	            {
                    // We've gone off the grid! Yucky. Need better way to handle this.
                    var pos3 = new Vector3(-10,-10);
                    pos3.z = 0;
                    //Debug.Log(pos3);
                    this.transform.position = pos3;
                }
            }

	    }
	}
}
