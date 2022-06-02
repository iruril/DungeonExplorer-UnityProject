using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HPSlider : MonoBehaviour
{
    public Slider hpbar;
    private float maxHp;
    private float currentHp;
    void Start()
    {
        maxHp = this.GetComponent<HPController>().myStartingHealth;
        currentHp = this.GetComponent<HPController>().health;
    }

    void Update()
    {
        currentHp = this.GetComponent<HPController>().health;
        transform.position = this.transform.position + new Vector3(0, 0, 0);
        hpbar.value = currentHp / maxHp;
        if(currentHp < 0.0f)
        {
            this.transform.Find("Fill Area").gameObject.SetActive(false);
        }
    }
}
