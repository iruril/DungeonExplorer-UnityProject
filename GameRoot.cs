using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isFailed = false;
    public GameObject askExit;
    public GameObject askRetry;
    public GameObject successImg;
    public GameObject totemCurseImg;

    public AudioClip failMP;
    public AudioClip successMP;

    private AudioSource resultSound = null;
    private bool musicPlayed = false;

    public GameObject playerState = null;
    public bool isOnExit = false;

    [SerializeField] Text time;
    float leftTime;

    private GameObject[] lever = null;
    private bool activateTrap = false;

    private float coolDown = 0.5f;
    private WaitForSeconds CoolDownWaitForSeconds;
    private bool isCoolDown = false;
    public Coroutine CoolCoroutine;
    public IEnumerator tickCalc()
    {
        isCoolDown = true;
        yield return CoolDownWaitForSeconds;
        isCoolDown = false;
    }

    void Start()
    {
        lever = GameObject.FindGameObjectsWithTag("Lever");

        askExit.transform.localScale = new Vector3(0, 0, 0);
        askRetry.transform.localScale = new Vector3(0, 0, 0);
        successImg.transform.localScale = new Vector3(0, 0, 0);
        totemCurseImg.transform.localScale = new Vector3(0, 0, 0);
        leftTime = 5;
        //successSound.clip = successMP;

        resultSound = this.GetComponent<AudioSource>();
        resultSound.volume = 0.3f;
        //successSound.volume = 0.3f;

        CoolDownWaitForSeconds = new WaitForSeconds(coolDown);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < lever.Length; i++)
        {
            if (lever[i].GetComponent<Lever>().isTrapActivated)
            {
                activateTrap = true;
            }
        }

        if(activateTrap == true)
        {
            totemCurseImg.transform.localScale = new Vector3(1, 1, 1);
            if (!isCoolDown)
            {
                DealDamage();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            askExit.transform.localScale = new Vector3(1, 1, 1);
            isOnExit = true;
        }

        if (isOnExit)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                askExit.transform.localScale = new Vector3(0, 0, 0);
                isOnExit = false;
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                SceneManager.LoadScene("MainScene");
            }
        }

        if (playerState.GetComponent<PlayerControl>().dead == true)
        {
            isFailed = true;
            if (!musicPlayed)
            {
                resultSound.clip = failMP;
                resultSound.Play();
                musicPlayed = true;
            }
            askRetry.transform.localScale = new Vector3(1, 1, 1);
        }

        if(isFailed)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                SceneManager.LoadScene("MainScene");
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                SceneManager.LoadScene("StageScene1");
            }
        }

        if((playerState.GetComponent<PlayerControl>().isSuccess == true))
        {
            if (!musicPlayed)
            {
                resultSound.clip = successMP;
                resultSound.Play();
                musicPlayed = true;
            }
            successImg.transform.localScale = new Vector3(1, 1, 1);
            leftTime -= Time.deltaTime;
            time.text = string.Format("{0}", (int)leftTime + 1);
            Invoke("LoadNextScene", 5.0f);
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("StageScene2");
    }

    private void DealDamage()
    {
        CoolCoroutine = StartCoroutine(tickCalc());
        playerState.GetComponent<HPController>().TakeHit(10.0f);
    }
}
