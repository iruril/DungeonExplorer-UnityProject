using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    private GameObject controlsObject;
    private GameObject optionsObject;

    // Start is called before the first frame update
    void Start()
    {
        controlsObject = GameObject.FindGameObjectWithTag("UIControls");
        controlsObject.transform.localScale = new Vector3(0,0,0);

        optionsObject = GameObject.FindGameObjectWithTag("UIOptions");
        optionsObject.transform.localScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoToPlay()
    {
        SceneManager.LoadScene("StageScene1");
    }

    public void gameExit()
    {
        Application.Quit();
    }

    public void ShowControls()
    {
        controlsObject.transform.localScale = new Vector3(1, 1, 1);
    }
    public void StopShowControls()
    {
        controlsObject.transform.localScale = new Vector3(0, 0, 0);
    }

    public void ShowOptions()
    {
        optionsObject.transform.localScale = new Vector3(1, 1, 1);
    }
    public void StopShowOptions()
    {
        optionsObject.transform.localScale = new Vector3(0, 0, 0);
    }
}
