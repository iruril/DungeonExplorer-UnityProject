using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    public float time = 1.0f;
    public Light pointLight;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pointLight.intensity -= 2.0f * Time.deltaTime;
        Destroy(this.gameObject, time);
    }
}
