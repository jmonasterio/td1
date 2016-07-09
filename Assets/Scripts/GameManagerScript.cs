using UnityEngine;
using System.Collections;
using Algorithms;
using UnityEngine.Assertions;

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

        // Now lets draw a path.

        var byteGrid = Toolbox.Instance.GameManager.GameGrid.ToByteGrid();

        var pf = new PathFinderFast(byteGrid);
        pf.Diagonals = false;
        pf.Formula = HeuristicFormula.Manhattan;
        pf.HeuristicEstimate = 2;
        pf.HeavyDiagonals = false;
        pf.PunishChangeDirection = true;
        pf.SearchLimit = 5000;
        pf.TieBreaker = false;
        var start = new Point(1,1);
        var end = new Point(10,10);
        Debug.Assert(byteGrid[1, 1] == 0);
        Debug.Assert(byteGrid[10, 10] == 0);
        var nodeList = pf.FindPath(start, end);
        if (nodeList != null)
        {
            foreach (var node in nodeList)
            {
                // Draw lines between the nodes, just to see.
                //Debug.DrawLine( );
                var nodePoint = new Point( node.X, node.Y);
                Toolbox.Instance.GameManager.GameGrid.DrawTextAtH9
                int a = 0;
            }
        }
        else
        {
            Debug.Assert(false, "No path found");
        }



    }

    // Update is called once per frame
    void Update () {
	
	}

   
}
