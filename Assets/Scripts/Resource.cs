using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DropHuman(Human human)
    {
        Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Income += human.IncomeValue;

        // TBD: If city, etc.

    }
}
