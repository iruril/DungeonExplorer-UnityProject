using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float speed = 1.0f;
    public bool isOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        isOpened = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpened)
        {
            this.transform.Translate(Vector3.up * -speed * Time.deltaTime);
            if (this.transform.localPosition.y <= -5.0f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
