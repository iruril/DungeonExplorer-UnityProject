using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine;

public class PlayerControl : HPController
{
    NavMeshAgent navAgent;
    Transform movePoint;
    Transform aimPoint;
    public Image damagedImage;
    public Image coolDownImage_space;
    public Image coolDownImage_skill;
    public Light movePointLight;

    public bool isSuccess = false;
    public bool isOnExit = false;

    public float flashSpeed = 5.0f;
    public Color flashColor = new Color(0.35f, 0f, 0f, 0.1f);

    public GameObject bloodParticle = null;

    public STEP step = STEP.NONE; // 현재 상태.
    public STEP next_step = STEP.NONE; // 다음 상태.
    public float step_timer = 0.0f; // 타이머

    public float spaceCoolDown = 5.0f;
    public float skillCoolDown = 3.0f;

    private WaitForSeconds spaceCoolDownWaitForSeconds;
    private bool isSpaceCoolDown = false;
    public Coroutine spaceCoolCoroutine;

    private WaitForSeconds skillCoolDownWaitForSeconds;
    private bool isSkillCoolDown = false;
    public Coroutine skillCoolCoroutine;

    private float spaceTime = 0.0f;
    private float skillTime = 0.0f;

    public float speed;
    public float spaceSpeed;

    private GameObject closest_item = null; // 플레이어의 정면에 있는 GameObject.
    private GameObject carried_item = null; // 플레이어가 들어올린 GameObject.
    private GameObject frontDoor = null; // 플레이어 앞의 Door GameObject.
    private GameObject lever = null;
    private GameObject totem = null;

    private TrailRenderer spaceTrail = null;
    public GameObject spaceParticle = null;
    public GameObject skillParticle = null;


    private ItemRoot item_root = null; // ItemRoot 스크립트를 가짐.
    public GUIStyle guistyle;

    private bool isGrabPressed;
    private bool isSpaced;
    private Vector3 pos;
    private Ray ray;
    private RaycastHit hitInfo;

    public int segments = 12;
    public float radius = 8.0f;
    LineRenderer indicator;

    public enum STEP
    { // 플레이어의 상태를 나타내는 열거체.
        NONE = -1, // 상태 정보 없음.
        MOVE = 0, // 이동 중.
        AIMMING = 1,
        ATTACK = 2, // 공격 중.
        SPACE = 3,
        NUM = 4, // 상태가 몇 종류 있는지 나타낸다(=3).
    };

    protected override void Start()
    {
        base.Start();

        this.step = STEP.NONE; // 현 단계 상태를 초기화.
        this.next_step = STEP.MOVE; // 다음 단계 상태를 초기화.

        indicator = gameObject.GetComponent<LineRenderer>();
        indicator.SetVertexCount(segments + 1);

        navAgent = GetComponent<NavMeshAgent>();
        movePoint = GameObject.FindGameObjectWithTag("MovePoint").transform;
        aimPoint = GameObject.FindGameObjectWithTag("AimPoint").transform;
        this.navAgent.speed = this.speed;
        this.item_root = GameObject.Find("GameRoot").GetComponent<ItemRoot>();
        this.spaceTrail = this.gameObject.transform.GetChild(3).transform.
            GetChild(0).gameObject.GetComponent<TrailRenderer>();
        this.guistyle.fontSize = 16;
        this.isGrabPressed = false;
        this.spaceTrail.enabled = false;
        spaceCoolDownWaitForSeconds = new WaitForSeconds(spaceCoolDown);
        skillCoolDownWaitForSeconds = new WaitForSeconds(skillCoolDown);
    }

    public IEnumerator coolDownCalc()
    {
        isSpaceCoolDown = true;
        yield return spaceCoolDownWaitForSeconds;
        isSpaceCoolDown = false;
    }

    public IEnumerator skillCoolDownCalc()
    {
        isSkillCoolDown = true;
        yield return skillCoolDownWaitForSeconds;
        isSkillCoolDown = false;
    }

    private void cheatOn()
    {
        spaceCoolDown = 0.1f;
        skillCoolDown = 0.1f;
        spaceCoolDownWaitForSeconds = new WaitForSeconds(spaceCoolDown);
        skillCoolDownWaitForSeconds = new WaitForSeconds(skillCoolDown);
    }

    void Update()
    {
        pos = Input.mousePosition;
        ray = Camera.main.ScreenPointToRay(pos);

        if (Input.GetKeyDown(KeyCode.H) & Input.GetKeyDown(KeyCode.I))
        {
            cheatOn();
        }

        if (isSpaceCoolDown)
        {
            spaceTime += Time.deltaTime;
        }
        else
        {
            spaceTime = 0.0f;
        }

        coolDownImage_space.fillAmount = (float)(spaceTime / spaceCoolDown);

        if (isSkillCoolDown)
        {
            skillTime += Time.deltaTime;
        }
        else
        {
            skillTime = 0.0f;
        }

        coolDownImage_skill.fillAmount = (float)(skillTime / skillCoolDown);

        if (Input.GetKeyDown(KeyCode.G))
        {
            isGrabPressed = true;
        }
        else
        {
            isGrabPressed = false;
        }

        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.MOVE:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (!isSpaceCoolDown)
                        {
                            this.next_step = STEP.SPACE;
                            this.spaceTrail.enabled = true;
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        if (!isSkillCoolDown)
                        {
                            this.next_step = STEP.AIMMING;
                        }
                    }
                    break;
                case STEP.SPACE:
                    if (navAgent.velocity.sqrMagnitude <= 0.1f && navAgent.remainingDistance <= 0.0f)
                    {
                        this.next_step = STEP.MOVE;
                        this.spaceTrail.enabled = false;
                    }
                    break;
                case STEP.AIMMING:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (!isSpaceCoolDown)
                        {
                            this.next_step = STEP.SPACE;
                            this.spaceTrail.enabled = true;
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1))
                    {
                        this.next_step = STEP.MOVE;
                    }
                    if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Q))
                    {
                        this.next_step = STEP.ATTACK;
                    }
                    break;
                case STEP.ATTACK:
                    this.next_step = STEP.MOVE; //임시로 해둠.
                    break;
            }
        }

            // 상태가 변화했을 때------------.
        while (this.next_step != STEP.NONE)
        { // 상태가 NONE이외 = 상태가 변화했다.
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.MOVE:
                    this.navAgent.speed = this.speed;
                    break;
                case STEP.SPACE:
                    spaceCoolCoroutine = StartCoroutine(coolDownCalc());
                    this.navAgent.speed = this.spaceSpeed;
                    this.playerSpace();
                    break;
                case STEP.AIMMING:

                    break;
                case STEP.ATTACK:
                    skillCoolCoroutine = StartCoroutine(skillCoolDownCalc());
                    break;
            }
            this.step_timer = 0.0f;
        }
        // 각 상황에서 반복할 것----------.
        switch (this.step)
        {
            case STEP.MOVE:
                DecreatePoints();
                aimPoint.gameObject.SetActive(false);
                this.playerRPGMove();
                this.pick_or_drop_control();
                break;
            case STEP.SPACE:
                DecreatePoints();
                aimPoint.gameObject.SetActive(false);
                break;
            case STEP.AIMMING:
                this.aimState();
                break;
            case STEP.ATTACK:
                DecreatePoints();
                aimPoint.gameObject.SetActive(false);
                attackState();
                break;
        }

        if (isHitted)
        {
            generateBlood();
        }
        else
        {
            damagedImage.color = Color.Lerp(damagedImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
    }
    private void generateBlood()
    {
        damagedImage.color = flashColor;
        Instantiate(bloodParticle, new Vector3(
                this.transform.position.x,
                this.transform.position.y + 1.0f,
                this.transform.position.z),
                this.transform.rotation);
        this.isHitted = false;
    }

    void OnTriggerStay(Collider other)
    {
        GameObject other_go = other.gameObject;
        // 트리거의 GameObject 레이어 설정이 Item이라면.
        if (other_go.layer == LayerMask.NameToLayer("Item"))
        {
            // 아무 것도 주목하고 있지 않으면.
            if (this.closest_item == null)
            {
                if (this.is_other_in_view(other_go))
                { // 정면에 있으면.
                    this.closest_item = other_go; // 주목한다.
                }
                // 뭔가 주목하고 있으면.
            }
            else if (this.closest_item == other_go)
            {
                if (!this.is_other_in_view(other_go))
                { // 정면에 없으면.
                    this.closest_item = null; // 주목을 그만둔다.
                }
            }
        }

        GameObject other_door = other.gameObject;

        if (other_door.layer == LayerMask.NameToLayer("Door"))
        {
            if (this.frontDoor == null)
            {
                this.frontDoor = other_door;
            }
        }

        GameObject other_lever = other.gameObject;
        if (other_lever.layer == LayerMask.NameToLayer("Lever"))
        {
            // 아무 것도 주목하고 있지 않으면.
            if (this.lever == null)
            {
                this.lever = other_lever; // 주목한다.
            }
        }

        GameObject other_totem = other.gameObject;
        if (other_totem.layer == LayerMask.NameToLayer("Totem"))
        {
            // 아무 것도 주목하고 있지 않으면.
            if (this.totem == null)
            {
                this.totem = other_totem;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (this.closest_item == other.gameObject)
        {
            this.closest_item = null; // 주목을 그만둔다.
        }

        if (this.frontDoor == other.gameObject)
        {
            this.frontDoor = null; // 주목을 그만둔다.
        }

        if (this.lever == other.gameObject)
        {
            this.lever = null; // 주목을 그만둔다.
        }

        if (this.totem == other.gameObject)
        {
            this.totem = null; // 주목을 그만둔다.
        }
    }

    void OnGUI()
    {
        float x = Screen.width / 2.0f + 40.0f;
        float y = Screen.height / 2.0f - 40.0f;
        if (!isSuccess & !isOnExit)
        {
            if (isSpaceCoolDown)
            {
                GUI.Label(new Rect(x - 10.0f, y + 60.0f, 200.0f, 20.0f), "<color=#b8f8fb>Space Cool Down...</color>", guistyle);
            }

            if (isSkillCoolDown)
            {
                GUI.Label(new Rect(Screen.width / 2.0f - 60.0f, Screen.height - 220.0f, 200.0f, 20.0f),
                    "<color=#ffdfdc>Skill Cool Down...</color>", guistyle);
            }

            // 들고 있는 아이템이 있다면.
            if (this.carried_item != null)
            {
                if (this.frontDoor != null)
                {
                    GUI.Label(new Rect(x, y, 200.0f, 20.0f), "<color=#ffffff>G:Use key</color>", guistyle);
                }

                else
                {
                    if (this.totem == null & this.lever == null)
                    {
                        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "<color=#ffffff>G:Release</color>", guistyle);
                    }
                }

                if(this.totem != null)
                {
                    if (this.totem.GetComponent<TotemControl>().isCured == false)
                    {
                        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "<color=#ffffff>G:Cure the Totem</color>", guistyle);
                    }
                }

                if (this.lever != null)
                {
                    if (this.lever.GetComponent<Lever>().isPulled == false)
                    {
                        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "<color=#ffffff>G:Pull</color>", guistyle);
                    }
                }
            }
            else
            {
                // 주목하고 있는 아이템이 있다면.
                if (this.closest_item != null & this.totem == null)
                {
                    GUI.Label(new Rect(x, y, 200.0f, 20.0f), "<color=#ffffff>G:Grab</color>", guistyle);
                }

                if (this.lever != null)
                {
                    if (this.lever.GetComponent<Lever>().isPulled == false)
                    {
                        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "<color=#ffffff>G:Pull</color>", guistyle);
                    }
                }

                else if(this.totem != null)
                {
                    if (this.totem.GetComponent<TotemControl>().isCured == false)
                    {
                        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "<color=#ffffff>G:Cure the Totem</color>", guistyle);
                    }
                }
            }
        }
    }

    private void attackState()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        Vector3 direction;
        if (Physics.Raycast(ray, out hitInfo, 100f, layerMask))
        {
            direction = hitInfo.point;
            direction.y = 0.0f;
            Instantiate(skillParticle, new Vector3(hitInfo.point.x, hitInfo.point.y + 1.5f, hitInfo.point.z), this.transform.rotation);
            navAgent.enabled = false;
            this.transform.LookAt(direction);
            navAgent.enabled = true;
        }
    }

    private void aimState()
    {
        CreatePoints();
        movePointLight.intensity -= 10.0f * Time.deltaTime;
        int layerMask = 1 << LayerMask.NameToLayer("Ground");

        if (Physics.Raycast(ray, out hitInfo, 100f, layerMask))
        {
            aimPoint.position = hitInfo.point;
            if (Vector3.Distance(this.transform.position, hitInfo.point) <= radius)
            {
                aimPoint.gameObject.SetActive(true);
            }
            else
            {
                aimPoint.gameObject.SetActive(false);
            }
        }
    }

    private void CreatePoints()
    {
        float x;
        float y = 0.16f;
        float z;
        float angle = 20.0f;
        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            indicator.SetPosition(i, new Vector3(this.transform.position.x + x, y, this.transform.position.z + z));
            angle += (360f / segments);
        }
    }

    private void DecreatePoints()
    {
        for (int i = 0; i < (segments + 1); i++)
        {
            indicator.SetPosition(i, new Vector3(0, -5, 0));
        }
    }

    private void playerRPGMove()
    {
        movePointLight.intensity -= 10.0f * Time.deltaTime;
        if ((navAgent.remainingDistance == 0.0f))
        {
            movePoint.gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(1))
        {
            int layerMask = 1<<LayerMask.NameToLayer("Ground");
            movePointLight.intensity = 5.0f;

            if (Physics.Raycast(ray, out hitInfo, 100f ,layerMask))
            {
                movePoint.position = hitInfo.point;
                movePoint.gameObject.SetActive(true);

                navAgent.SetDestination(hitInfo.point);
            }
        }
    }

    private void playerSpace()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ground");

        Vector3 playerPos = this.transform.position;
        playerPos.y = 0.15f;

        if (Physics.Raycast(ray, out hitInfo, 100f, layerMask))
        {
            Vector3 direction = hitInfo.point - playerPos;
            direction.Normalize();
            navAgent.SetDestination(this.transform.position + direction * 4.0f);
            Instantiate(spaceParticle, this.transform.position, this.transform.rotation);
        }
    }

    private void pick_or_drop_control()
    {
        do
        {
            if (!isGrabPressed)
            { // '줍기/버리기'키가 눌리지 않았으면.
                break; // 아무것도 하지 않고 메소드 종료.
            }
            else
            {
                if (this.lever != null)
                {
                    this.lever.GetComponent<Lever>().isPulled = true;
                }

                if (this.totem != null)
                {
                    this.totem.GetComponent<TotemControl>().isCured = true;
                }

                if (this.carried_item == null)
                { // 들고 있는 아이템이 없고.
                    if (this.closest_item == null)
                    {// 주목 중인 아이템이 없으면.
                        break; // 아무것도 하지 않고 메소드 종료.
                    }
                    // 주목 중인 아이템을 들어올린다.
                    this.carried_item = this.closest_item;
                    // 들고 있는 아이템을 자신의 자식으로 설정.
                    this.carried_item.transform.parent = this.transform;
                    // 2.0f 위에 배치(머리 위로 이동).
                    this.carried_item.transform.localPosition = Vector3.forward * 1.0f;
                    // 주목 중 아이템을 없앤다.
                    this.closest_item = null;
                }
                else
                {
                    if (this.frontDoor != null)
                    {
                        this.frontDoor.GetComponent<Door>().isOpened = true;

                        this.carried_item.transform.localPosition = Vector3.forward * 1.0f + Vector3.up * (-1.0f);
                        this.carried_item.transform.parent = null;// 자식 설정을 해제.
                        Destroy(this.carried_item);
                    }
                    else
                    {
                        if (this.totem == null & this.lever == null)
                        {
                            this.carried_item.transform.localPosition = Vector3.forward * 1.0f + Vector3.up * (-1.0f);
                            this.carried_item.transform.parent = null;// 자식 설정을 해제.
                            this.carried_item = null; // 들고 있던 아이템을 없앤다.
                        }
                    }
                }
            }
        } while (false);
    }

    private bool is_other_in_view(GameObject other)
    {
        bool ret = false;
        do
        {
            Vector3 heading = // 자신이 현재 향하고 있는 방향을 보관.
            this.transform.TransformDirection(Vector3.forward);
            Vector3 to_other = // 자신 쪽에서 본 아이템의 방향을 보관.
            other.transform.position - this.transform.position;
            heading.y = 0.0f;
            to_other.y = 0.0f;
            heading.Normalize(); // 길이를 1로 하고 방향만 벡터로.
            to_other.Normalize(); // 길이를 1로 하고 방향만 벡터로.
            float dp = Vector3.Dot(heading, to_other); // 양쪽 벡터의 내적을 취득.
            if (dp < Mathf.Cos(45.0f))
            { // 내적이 45도인 코사인 값 미만이면.
                break; // 루프를 빠져나간다.
            }
            ret = true; // 내적이 45도인 코사인 값 이상이면 정면에 있다.
        } while (false);
        return (ret);
    }

    protected override void Die()
    {
        dead = true;
        this.gameObject.SetActive(false);
    }
}