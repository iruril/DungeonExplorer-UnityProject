using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyControl : HPController
{
    // Start is called before the first frame update
    NavMeshAgent navAgent;

    public STEP step = STEP.NONE; // 현재 상태.
    public STEP next_step = STEP.NONE; // 다음 상태.
    public float step_timer = 0.0f; // 타이머
    public float enemySpeed = 5.0f;
    public float attackIndicateDelay = 2.0f;

    public GameObject bloodParticle = null;

    private Transform player = null;

    private float coolDown = 2.0f;
    private WaitForSeconds CoolDownWaitForSeconds;
    private bool isCoolDown = false;
    public Coroutine CoolCoroutine;

    public float blinkCoolDown = 5.0f; //회피 빈도 조정
    private WaitForSeconds blinkCoolDownWaitForSeconds;
    private bool isBlinkCoolDown = false;
    public Coroutine blinkCoolCoroutine;

    public float attackCoolDown = 3.0f; //attack 빈도 조정
    private WaitForSeconds attackCoolDownWaitForSeconds;
    private bool isAttackCoolDown = false;
    public Coroutine attackCoolCoroutine;

    public GameObject aiMap = null;
    private AIMapScript aiScript = null;
    public bool isChasing = false;

    private TrailRenderer blinkTrail = null;
    public GameObject blinkParticle = null;
    public GameObject attackParticle = null;
    public GameObject dropItem = null;

    public GameObject hudDamageText;
    public Transform hudPos;

    private Vector3 prev_playerPosition;
    public int segments = 12;
    public float radius = 1.5f;
    LineRenderer indicator;

    public enum STEP
    { // Enemy의 상태를 나타내는 열거체.
        NONE = -1, // 상태 정보 없음.
        MOVE = 0, // 이동 중.
        PATROL = 1,
        ATTACK = 2, // 공격 중.
        BLINK = 3,
        NUM = 4, // 상태가 몇 종류 있는지 나타낸다(=3).
    };

    public IEnumerator tickCalc()
    {
        isCoolDown = true;
        yield return CoolDownWaitForSeconds;
        isCoolDown = false;
    }

    public IEnumerator blinkCalc()
    {
        isBlinkCoolDown = true;
        yield return blinkCoolDownWaitForSeconds;
        isBlinkCoolDown = false;
    }

    public IEnumerator attackCalc()
    {
        this.prev_playerPosition = player.position;
        isAttackCoolDown = true;

        DecreatePoints();
        yield return attackCoolDownWaitForSeconds;
        isAttackCoolDown = false;
    }

    protected override void Start()
    {
        base.Start();

        indicator = gameObject.GetComponent<LineRenderer>();
        indicator.SetVertexCount(segments + 1);

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = enemySpeed;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        this.blinkTrail = this.gameObject.transform.GetChild(0).transform.
            GetChild(0).gameObject.GetComponent<TrailRenderer>();
        this.blinkTrail.enabled = false;
        CoolDownWaitForSeconds = new WaitForSeconds(coolDown);
        blinkCoolDownWaitForSeconds = new WaitForSeconds(blinkCoolDown);
        attackCoolDownWaitForSeconds = new WaitForSeconds(attackCoolDown);

        if (aiMap == null)
        {
            this.step = STEP.NONE; // 현 단계 상태를 초기화.
            this.next_step = STEP.MOVE;
            isChasing = true;
        }
        else
        {
            aiScript = aiMap.GetComponent<AIMapScript>();
            this.step = STEP.NONE; // 현 단계 상태를 초기화.
            this.next_step = STEP.PATROL; // 다음 단계 상태를 초기화.
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(this.transform.position, player.position);

        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.MOVE:
                    if (distance < 8.0f)
                    {
                        this.next_step = STEP.ATTACK;
                    }
                    if (this.health < (this.myStartingHealth * 0.5) && !isBlinkCoolDown && distance < 8.0f)
                    {
                        this.next_step = STEP.BLINK;
                        this.blinkTrail.enabled = true;
                    }
                    break;
                case STEP.PATROL:
                    if (aiScript.isInArea() || (this.health < this.myStartingHealth) || distance < 5.0f)
                    {
                        this.isChasing = true;
                        this.next_step = STEP.MOVE;
                    }
                    break;
                case STEP.ATTACK:
                    if (distance > 8.0f)
                    {
                        this.next_step = STEP.MOVE;
                    }
                    if (this.health < (this.myStartingHealth * 0.5) && !isBlinkCoolDown)
                    {
                        this.next_step = STEP.BLINK;
                        this.blinkTrail.enabled = true;
                    }
                    break;
                case STEP.BLINK:
                    if (this.step_timer > 1.0f)
                    {
                        this.next_step = STEP.MOVE;
                        this.blinkTrail.enabled = false;
                    }
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
                    this.navAgent.speed = enemySpeed;
                    break;
                case STEP.PATROL:
                    
                    break;
                case STEP.ATTACK:
                    
                    break;
                case STEP.BLINK:
                    this.navAgent.speed = 100.0f;
                    Blink();
                    break;
            }
            this.step_timer = 0.0f;
        }
        // 각 상황에서 반복할 것----------.
        switch (this.step)
        {
            case STEP.MOVE:
                Move();
                break;
            case STEP.PATROL:
                if (!isCoolDown)
                {
                    Patrol();
                }
                break;
            case STEP.ATTACK:
                if (!isAttackCoolDown)
                {
                    Attack(); //임시
                }
                break;
            case STEP.BLINK:
                this.step_timer += Time.deltaTime;
                break;
        }

        if (isHitted)
        {
            generateBlood();
        }
    }

    public override void TakeHit(float damage)
    {
        GameObject hudText = Instantiate(hudDamageText); // 생성할 텍스트 오브젝트
        hudText.transform.position = hudPos.position; // 표시될 위치
        hudText.GetComponent<DamageText>().damage = (int)damage; // 데미지 전달
        base.TakeHit(damage);
    }

    private void generateBlood()
    {
        Instantiate(bloodParticle, new Vector3(
                this.transform.position.x,
                this.transform.position.y + 1.0f,
                this.transform.position.z),
                this.transform.rotation);
        this.isHitted = false;
    }

    private void Move()
    {
        navAgent.SetDestination(new Vector3(
            player.transform.position.x,
            0.15f,
            player.transform.position.z));
    }
    private void Patrol()
    {
        CoolCoroutine = StartCoroutine(tickCalc());

        float random_number_x = Random.Range(-5.0f, 5.0f);
        float random_number_z = Random.Range(-5.0f, 5.0f);

        navAgent.SetDestination(new Vector3(
           this.transform.position.x + random_number_x,
           this.transform.position.y,
           this.transform.position.z + random_number_z));
    }

    private void Attack()
    {
        attackCoolCoroutine = StartCoroutine(attackCalc());

        navAgent.SetDestination(this.transform.position);

        AttackIndicator();
        Invoke("AttackImpact", attackIndicateDelay - 0.5f);
    }

    private void AttackIndicator()
    {
        float x;
        float y = 0.16f;
        float z;
        float angle = 20.0f;
        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            indicator.SetPosition(i, new Vector3(prev_playerPosition.x + x, y, prev_playerPosition.z + z));
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

    private void AttackImpact()
    {
        Instantiate(attackParticle, new Vector3(prev_playerPosition.x, 0.75f, prev_playerPosition.z), this.transform.rotation);
        DecreatePoints();
    }

    private void Blink()
    {
        blinkCoolCoroutine = StartCoroutine(blinkCalc());

        float random_angle = Random.Range(-90.0f, 90.0f);
        float random_range = Random.Range(3.0f, 4.5f);
        Vector3 backward = this.transform.position - player.position;
        backward.Normalize();
        backward = Quaternion.AngleAxis(random_angle, Vector3.up) * backward;
        navAgent.SetDestination(this.transform.position + backward * random_range);
        Instantiate(blinkParticle, this.transform.position, this.transform.rotation);
    }

    protected override void Die()
    {
        if(dropItem == null)
        {
            base.Die();
        }
        else
        {
            Instantiate(dropItem, new Vector3(this.transform.position.x, 0.0f, this.transform.position.z), this.transform.rotation);
            base.Die();
        }
    }
}
