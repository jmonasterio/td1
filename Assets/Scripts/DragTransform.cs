using UnityEngine;
using System.Collections;

class DragTransform : MonoBehaviour
{
    public bool Draggable = true;
    public Vector3 DragScale;
    private Color mouseOverColor = Color.blue;
    private Color originalColor = Color.yellow;
    private bool _dragging = false;
    private float _distance;
    private SpriteRenderer _renderer;
    private Vector3 _startPos;
    private Vector3 _originalScale;



    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();

    }

    void OnMouseEnter()
    {
        if (Draggable)
        {
           // _renderer.material.color = mouseOverColor;
        }
    }

    void OnMouseExit()
    {
        if (Draggable)
        {
           // _renderer.material.color = originalColor;
        }

    }

    void OnMouseDown()
    {
        if (Draggable)
        {
            _startPos = transform.position;
            _originalScale = transform.localScale;
            transform.localScale = DragScale;
            _distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            _dragging = true;
        }

    }

    void OnMouseUp()
    {
        if (Draggable)
        {

            _dragging = false;

            // Cancel
            var ds = GetComponent<DragSpawner>();
            if (ds != null)
            {
                DropAPrefabAtSelector(ds.SpawnPF);
            }
            //Cancel
            transform.position = _startPos;
            transform.localScale = _originalScale;
        }
    }

    private void DropAPrefabAtSelector(GameObject prefab)
        {
        var gameGrid = Toolbox.Instance.GameManager.GameGrid;
        var worldPos = gameGrid.GetSelector().transform.position;
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


    void Update()
    {
        if (Draggable)
        {

            if (_dragging)
            {
                // TBD: Can do this faster in 2D
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayPoint = ray.GetPoint(_distance);
                transform.position = rayPoint;
            }
        }
    }
}
