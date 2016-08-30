using UnityEngine;
using System.Collections;
using UnityEditor;

// TBD: Combine with drag transform.
public class DragSpawner : MonoBehaviour
{

    public DragSource SpawnPF;
    public GameObject SpawnParent;
    private GameGrid _gameGrid;

    private MouseInput _mouseInput;

    // Use this for initialization
    void Start()
    {
        _mouseInput = Toolbox.Instance.GameManager.GameGrid.GetSelector().GetComponent<MouseInput>();
        
    }

    public void OnMouseDown()
    {
        var newGameObject = Instantiate(SpawnPF);
        newGameObject.transform.SetParent(SpawnParent.transform, worldPositionStays:false);
        newGameObject.transform.position = this.transform.position;
        //newGameObject.transform.localScale = this.transform.localScale;
        newGameObject.gameObject.layer = GameGrid.DRAG_LAYER;
        newGameObject.DeleteOnCancel = true;
        var snap = newGameObject.GetComponent<SnapToGrid>();
        snap.snapToGrid = false;
        var dragSource = newGameObject.GetComponent<DragSource>();
        Debug.Assert( dragSource != null);
        _mouseInput.StartDraggingSpawnedObject(dragSource );

        Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Income -= newGameObject.GetComponent<Entity>().IncomeCost;
    }

    // Update is called once per frame
    private void OnGUI()
    {
        var income = SpawnPF.GetComponent<Entity>().IncomeCost;
        var vector = this.transform.position + new Vector3(-1, 1, 0);
        var color = GUI.color;
        GUI.color = Color.black;
        Handles.Label(vector, "Cost: " + income);
        GUI.color = color;
    }
}
