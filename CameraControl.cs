using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Transform tracking;

    void Start()
    {
        //Player �ױ׸� ���� ���� ������Ʈ�� ��ǥ���� tr ������ �����Ѵ�.
        tracking = GameObject.FindGameObjectWithTag("Player").transform;
    }


    //Late �� �Է��ϸ� ��� ��ũ��Ʈ�� Update �� �Ϸ��� �ڿ� �ش� ������ ó���Ѵٴ� ���̴�. (Late�� �Է����� ������ Ư��PC ������ ����� ���� �߻�)
    void LateUpdate()
    {

        transform.position = tracking.position;
    }
}
