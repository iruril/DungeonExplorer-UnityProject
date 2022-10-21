using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMapScript : MonoBehaviour
{
    private bool isInHere = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            isInHere = true;
        }
    }

    public bool isInArea()
    {
        if (isInHere)
        {
            return true;
        }
        return false;
    }
}
