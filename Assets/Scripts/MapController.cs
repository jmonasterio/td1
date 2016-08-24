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
#if NOT_DRAGGING_FROM_SOURCE
        if (Input.GetMouseButtonDown(0))
        {
            DropAPrefabAtSelector(TowerPrefab.gameObject);
         
#if NEWISH
            var gameGrid = Toolbox.Instance.GameManager.GameGrid;
            var cell = gameGrid.MapScreenToMapPositionOrNull(Input.mousePosition);
            Instantiate<Tower>();
            Debug.Log("Mouse click.");
#endif
        }
        else if (Input.GetMouseButtonDown(1))
        {
            DropAPrefabAtSelector( SquarePrefab);
        }
#endif
    }

    private void DropAPrefabAtSelector(GameObject prefab)
    {
        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        var cell = gameGrid.GetSelectorCellOrNull();
        if (cell != null)
        {
            if (cell.BackgroundGameObject == null)
            {
                gameGrid.InstaniatePrefabAtGameCell(prefab, cell);
            }

        }
    }
}
