using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    private GameObject optionObject;

    // Start is called before the first frame update
    void Start()
    {
        optionObject = GameObject.FindGameObjectWithTag("UIControls");
        optionObject.transform.localScale = new Vector3(0,0,0);
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
        optionObject.transform.localScale = new Vector3(1, 1, 1);
    }
    public void StopShowControls()
    {
        optionObject.transform.localScale = new Vector3(0, 0, 0);
    }

    public void ShowOptions()
    {
        //SceneManager.LoadScene("SampleScene");
    }
}
