using UnityEngine;
using System.Collections;

class DragTransform : MonoBehaviour
{
    public bool Draggable = true;
    public Vector3 DragScale = new Vector3(1,1,1);

    private Color mouseOverColor = Color.blue;
    private Color originalColor = Color.yellow;
    public bool Dragging = false;
    private float _distance;
    private SpriteRenderer _renderer;
    private Vector3 _startPos;
    private Vector3 _originalScale;
    private PathFollower _pf;


    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _pf = GetComponent<PathFollower>();

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
            //transform.localScale = DragScale;
            _distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            Dragging = true; // Will stop path following. Lame. TBD

            var ds = GetComponent<DragSpawner>();
            if (ds != null)
            {
                ds.StartDrag();
            }
        }

    }

    void OnMouseUp()
    {
        if (Draggable)
        {
            Dragging = false;

            var gameGrid = Toolbox.Instance.GameManager.GameGrid;
            var worldPos = gameGrid.GetSelector().transform.position;
            worldPos.z = -10;
            GameGrid.GameCell dropCell = gameGrid.MapPositionToGameCellOrNull(worldPos);

            if (dropCell != null)
            {
                var human = GetComponent<Human>();
                if (human != null)
                {
                    // We're only allowed to drop on intersting cells.
                    if (dropCell.BackgroundGameObject != null)
                    {
                        var target = dropCell.BackgroundGameObject;
                        if (target.GetComponent<Resource>())
                        {
                            var resource = target.GetComponent<Resource>();
                            resource.DropHuman(human);
                            Destroy(human.gameObject);
                            return;
                        }
                        else if (target.GetComponent<Robot>())
                        {
                            var robot = target.GetComponent<Robot>();
                            robot.DropHuman(human);
                            Destroy(human.gameObject);
                            return;
                        }
                        else if (target.GetComponent<Tower>())
                        {
                            var tower = target.GetComponent<Tower>();
                            tower.DropHuman(human); // TBD: Should DropHuman be a component that everyone has?
                            Destroy(human.gameObject);
                            return;
                        }

                    }

                }
                else
                {
                    var ds = GetComponent<DragSpawner>();
                    if (ds != null)
                    {
                        // Only blank cells.
                        if (dropCell.BackgroundGameObject == null)
                        {
                            Toolbox.Instance.GameManager.GetComponent<ScoreController>().Income -=
                                ds.SpawnPF.GetComponent<Entity>().IncomeCost;
                            gameGrid.InstaniatePrefabAtGameCell(ds.SpawnPF, dropCell);
                        }

                        ds.StopDrag();

                        // FALL THRU TO CANCEL.
                    }
                }
            }
        }
        //Cancel
        transform.position = _startPos;
        transform.localScale = _originalScale;
    }



    void Update()
    {
        if (Draggable)
        {

            if (Dragging)
            {
                // TBD: Can do this faster in 2D
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayPoint = ray.GetPoint(_distance);
                transform.position = rayPoint;
            }
        }
    }
}
