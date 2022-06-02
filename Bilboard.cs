using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour
{
    private GameObject camera;
    private Transform cameraPos;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraPos = camera.transform;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(transform.position + cameraPos.rotation * Vector3.forward
                            , cameraPos.rotation * Vector3.up);
    }
}
