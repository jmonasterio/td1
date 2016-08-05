using UnityEngine;
using System.Collections;
using UnityEditor;

// TBD: Combine with drag transform.
public class DragSpawner : MonoBehaviour
{

    public GameObject SpawnPF;
    private bool _dragging;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartDrag()
    {
        _dragging = true;
    }

    public void StopDrag()
    {
        _dragging = false;
    }

    // Update is called once per frame
    private void OnGUI()
    {
        if (!_dragging)
        {
            var income = SpawnPF.GetComponent<Entity>().IncomeCost;
            var vector = this.transform.position + new Vector3(-1, 1, 0);
            var color = GUI.color;
            GUI.color = Color.black;
            Handles.Label(vector, "Cost: " + income);
            GUI.color = color;
        }

    }
}
