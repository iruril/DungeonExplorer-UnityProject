using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemControl : MonoBehaviour
{
    public bool isCured;
    public float speed = 3.0f;
    private float size = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.isCured = false;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.up * 25.0f * Time.deltaTime);

        if (isCured)
        {
            size *= 0.998f;
            if (this.transform.localPosition.y <= 20.0f)
            {
                this.transform.Translate(Vector3.up * speed * Time.deltaTime);
                this.transform.localScale = new Vector3(size, size, size);
            }
        }
    }
}
