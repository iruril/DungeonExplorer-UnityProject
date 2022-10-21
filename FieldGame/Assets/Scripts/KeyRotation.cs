using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyRotation : MonoBehaviour
{
    float degreesPerSec = 90.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.up * Time.deltaTime * degreesPerSec);
    }
}
