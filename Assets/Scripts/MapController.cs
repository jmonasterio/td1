using UnityEngine;
using System.Collections;

public class MapController : MonoBehaviour {

    public Tower TowerPrefab;
    public GameObject SquarePrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DropAPrefabAtSelector(TowerPrefab.gameObject);
         
#if NEWISH
            var gameGrid = Toolbox.Instance.GameManager.GameGrid;
            var cell = gameGrid.MapScreenToMapPositionOrNull(Input.mousePosition);
            Instantiate<Tower>();
            Debug.Log("Mouse click.");
#endif
        }
        else if (Input.GetMouseButton(1))
        {
            DropAPrefabAtSelector( SquarePrefab);
        }
    }

    private void DropAPrefabAtSelector(GameObject prefab)
    {
        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        var worldPos = gameGrid.Selector.transform.position;
        worldPos.z = -10;
        GameGrid.GameCell cell = gameGrid.MapPositionToGameCellOrNull(worldPos);
        if (cell != null)
        {
            if (cell.BackgroundGameObject == null)
            {
                gameGrid.InstaniatePrefabAtGameCell(prefab, cell);
            }

        }
    }
}
