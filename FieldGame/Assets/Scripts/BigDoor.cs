using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigDoor : MonoBehaviour
{
    public float speed = 1.0f;
    private GameObject[] lever = null;
    public bool isOpened = false;

    public bool playerAproached = false;

    // Start is called before the first frame update
    void Start()
    {
        lever = GameObject.FindGameObjectsWithTag("Lever");
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" & isOpened)
        {
            playerAproached = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        int conditions = 0;
        for(int i = 0; i < lever.Length; i++)
        {
            if (lever[i].GetComponent<Lever>().isPulled)
            {
                conditions++;
            }
        }
        if(conditions == lever.Length)
        {
            isOpened = true;
        }
        if (isOpened & playerAproached)
        {
            this.transform.Translate(Vector3.up * -speed * Time.deltaTime);
            if (this.transform.localPosition.y <= -5.0f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
