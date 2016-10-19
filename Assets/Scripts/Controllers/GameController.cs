using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GameOver()
    {
        //Debug.LogWarning("Should unload scene.");
        SceneManager.LoadScene("GameOver");
    }
}
