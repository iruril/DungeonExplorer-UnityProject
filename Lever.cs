using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isPulled;
    public bool isDeongunCured = false;
    public bool isTrapActivated = false;

    private bool isRotated = false;
    private bool dropped = false;
    public GameObject dropItem = null;

    private GameObject[] totem = null;
    void Start()
    {
        this.isPulled = false;
        totem = GameObject.FindGameObjectsWithTag("Totem");
    }

    // Update is called once per frame
    void Update()
    {
        int conditions = 0;
        for (int i = 0; i < totem.Length; i++)
        {
            if (totem[i].GetComponent<TotemControl>().isCured)
            {
                conditions++;
            }
        }
        if (conditions == totem.Length)
        {
            isDeongunCured = true;
        }

        if (isPulled)
        {
            if (!isRotated)
            {
                this.transform.rotation = Quaternion.AngleAxis(180.0f, this.transform.up) * this.transform.rotation;
                isRotated = true;
            }
            if (isDeongunCured)
            {
                if (!dropped)
                {
                    if (dropItem != null)
                    {
                        dropItems();
                    }
                }
            }
            else
            {
                isTrapActivated = true;
            }
        }
    }

    private void dropItems()
    {
        for (int i = 0; i < 3; i++)
        {
            float random_number_x = Random.Range(-5.0f, 5.0f);
            float random_number_z = Random.Range(-5.0f, 5.0f);
            Instantiate(dropItem, new Vector3(
                this.transform.position.x + random_number_x,
                1.0f,
                this.transform.position.z + random_number_z),
                this.transform.rotation
                );
        }
        dropped = true;
    }
}
