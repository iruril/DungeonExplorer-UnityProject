using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Transform tracking;

    void Start()
    {
        //Player 테그를 가진 게임 오브젝트의 좌표값을 tr 변수에 저장한다.
        tracking = GameObject.FindGameObjectWithTag("Player").transform;
    }


    //Late 를 입력하면 모든 스크립트의 Update 를 완료한 뒤에 해당 내용을 처리한다는 뜻이다. (Late를 입력하지 않으면 특정PC 에서는 끊기는 현상 발생)
    void LateUpdate()
    {

        transform.position = tracking.position;
    }
}
