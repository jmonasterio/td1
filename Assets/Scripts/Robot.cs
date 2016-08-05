using UnityEngine;
using System.Collections;

public class Robot : MonoBehaviour
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
        Toolbox.Instance.GameManager.gameObject.GetComponent<ScoreController>().Income += human.IncomeValue*2;


    }
}
