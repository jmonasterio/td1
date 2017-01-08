using UnityEngine;
using Assets.Scripts;

public class DragSpawner : MonoBehaviour
{

    public DragSource SpawnPF;
    public GameObject SpawnParent;

    private MouseInput _mouseInput;

    // Use this for initialization
    void Start()
    {
        _mouseInput = Toolbox.Instance.GameManager.GetSelector().GetComponent<MouseInput>();
        
    }

    public void OnMouseDown()
    {
        var newGameObject = Entity.InstantiateAt(SpawnPF, SpawnParent, this.transform.position, isSnap: false);
        newGameObject.DeleteOnCancel = true;
        var dragSource = newGameObject.GetComponent<DragSource>();
        Debug.Assert( dragSource != null);
        BuyFromPalette(this.SpawnPF.GetComponent<Entity>(), dragSource); 
        _mouseInput.StartDraggingSpawnedObject(dragSource );
    }

    public void BuyFromPalette(Entity entityToBuy, DragSource dragSource)
    {
        var cost = entityToBuy.BuildValue;
        Toolbox.Instance.GameManager.ScoreController.BuildScore -= cost;
        Debug.Assert(Toolbox.Instance.GameManager.ScoreController.BuildScore >= 0.0f);
        dragSource.CostPaidToBuild = cost; // Used later, if we need to refund money if drag canceled.
    }



    // Update is called once per frame
    private void OnGUI()
    {
        return; // ONGUITEST
        if (Event.current.type == EventType.Repaint)
        {
            var income = SpawnPF.GetComponent<Entity>().BuildValue;
            var vector = this.transform.position + new Vector3(-1, 1, 0);
            GuiExtension.GuiLabel(vector, "Cost: " + income, Color.red);
        }
    }
}
