using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDmg : MonoBehaviour
{
    // Start is called before the first frame update
    public float dealtDamage = 10.0f;

    private float coolDown = 0.2f; //데미지 틱 간격


    private WaitForSeconds CoolDownWaitForSeconds;
    private bool isCoolDown = false;
    public Coroutine CoolCoroutine;

    void Start()
    {
        CoolDownWaitForSeconds = new WaitForSeconds(coolDown);
    }

    public IEnumerator tickCalc()
    {
        isCoolDown = true;
        yield return CoolDownWaitForSeconds;
        isCoolDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCoolDown)
        {
            this.GetComponent<Collider>().enabled = false;
        }
        else
        {
            this.GetComponent<Collider>().enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            IDamgeable dealObject = other.gameObject.GetComponent<IDamgeable>();
            float rnd = Random.Range(-(dealtDamage / 10), (dealtDamage / 10));
            CoolCoroutine = StartCoroutine(tickCalc());
            dealObject.TakeHit(dealtDamage + rnd);
        }
    }
}
