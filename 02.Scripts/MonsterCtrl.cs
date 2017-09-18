using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent 형을 사용하기 위해서는 이것을 using하거나 형앞에 써줘야한다.

public class MonsterCtrl : MonoBehaviour {
    //이 방법으로 이동시 몬스터가 Nav Mesh 위에서 움직이기 때문에 살짝 바닥보다 떠서 움직이는 것처럼 보인다. 그러므로 monster Transform의 NavMeshAgent component의 Base offset을 조절한다.(-0.1)
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator; // 애니메이션 관리에 필요한 변수

    public enum MonsterState { idle, trace, attack, die };
    public MonsterState monsterState = MonsterState.idle;

    //추적사정거리
    public float traceDist = 10.0f;
    //공격 사정거리
    public float attackDist = 2.0f;
    //몬스터 사망 여부
    private bool isDie = false;

    //혈흔효과 관련 변수
    public GameObject bloodEffect;//혈흔 프리맵(particle 활용)
    public GameObject bloodDecal;//혈흔 데칼효과 (바닥에 스티커를 붙이는 듯한 효과, Plane mash)

    private int hp = 100;

    private GameUI gameUI; // 게임 UI에 접근하기 위한 변수. 


    void Start()
    {

    }
	// Use this for initialization

    //몬스터 생성을 오브젝트 풀로 하므로 start대신 awake에서 변수 할당을 수행한다. 왜냐하면 오브젝트 풀을 이용하면 OnEnable이 수행되는데 그건 Start전에 수행되므로 변수 할당은 OnEnable전에 수행되는 Awake에서 한다.
	void Awake () {
        monsterTr = this.gameObject.GetComponent<Transform>();
        //추적 대상을 직접 연결하는 속성이 없으므로 이렇게 대상과 추적속성(NavMeshAgent)를 스크립트로 연결해줘야한다.
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>(); // 추적대상, Plyaer 태그를 가진 Transform을 찾아낸다.
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();

        animator = this.gameObject.GetComponent<Animator>();

        //nvAgent.destination = playerTr.position; // 이 코드의 주석이 벗겨지면 시작과 동시에 플레이어를 추적하게 된다.


        #region 몬스터를 오브젝트 풀로 생성하지 않을때 코루틴함수
        //StartCoroutine(this.CheckMonsterState());//일정 간격으로 몬스터 행동을 체크하는 코루틴함수

        //StartCoroutine(this.MonsterAction());//몬스터 상태에 따라 동작을 기술한 코루틴함수
        //위의 코루틴 함수들은 Update함수 내에서 행동해도 문제될 것은 없다. 그러나 매 프레임마다 상태를 체크하기보다는 적정 주기로 체크하는 것이 효율적이고 깔끔하다.
        #endregion

        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();//스크립트를 할당함. 앞의 GameObject.Find함수는 GameUI라는 오브젝트를 찾은 것.
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            //일정시간 (0.2초)동안 Sleep하는 효과가 있다. 메인 루틴에게 양보한다.
            yield return new WaitForSeconds(0.2f); // 0.2초 기다렸다가 다음으로 넘어감. 이문장이 필요한 이유는 공부가 필요하겠지만 이것을 빼면 함수 자체에 에러가 잡힌다. 

            float dist = Vector3.Distance(playerTr.position, monsterTr.position); // 거리측정해서,

            if(dist <= attackDist)
            {
                monsterState = MonsterState.attack;
            }
            else if(dist <= traceDist)
            {
                monsterState = MonsterState.trace;
            }
            else
            {
                monsterState = MonsterState.idle;
            }//거리에 따른 몬스터의 상태 변환
        }
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (monsterState)
            {
                case MonsterState.idle:
                    nvAgent.Stop();//추적 
                    animator.SetBool("IsTrace", false);//Animator에서 설정한대로 IsTrace가 변경되면 자동으로 Animator가 애니메이션 분기를 변경시킨다.
                    break;
                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.Resume(); //목표의 위치를 받은 후 추적 재시작
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);
                    break;
                case MonsterState.attack:
                    nvAgent.Stop();
                    animator.SetBool("IsAttack", true);
                    break;
            }
            yield return null;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "BULLET")
        {
            CreateBloodEffect(coll.transform.position); //총알을 맞은 자리에 혈흔 효과 발생

            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage; //자신에게 맞은 총알의 컴포넌트 중, 총알의 정도가 담긴 스크립트 컴포넌트를 가져오고 데미지를 가져온다.(damage가 private면 접근 불가)
            if(hp <= 0)
            {
                MonsterDie();
            }

            Destroy(coll.gameObject);
            animator.SetTrigger("IsHit");
        }
    }

    IEnumerator PushObjectPool() //몬스터를 오브젝트 풀로 환원하는 코루틴함수이다.
    {
        yield return new WaitForSeconds(3.0f);

        //각종 변수를 초기화한다.
        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

        //몬스터의 Collider를 다시 활성화
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach(Collider coll in gameObject.GetComponentsInChildren<CapsuleCollider>())
        {
            coll.enabled = true;
        }

        //몬스터 비활성화.
        gameObject.SetActive(false);
    }

    void MonsterDie()
    {
        //모호하지만 자기자신의 태그에 접근하기 위해서는 gameObject.tag로 접근하는듯 싶다. 그냥 gameObject를 쓰면 자기 자신인듯.
        gameObject.tag = "Untagged"; //죽었을 경우 태그를 변경해서 새로운 몬스터 스폰에 영향을 주지 않도록 한다.

        StopAllCoroutines(); //모든 코루틴 정지

        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.Stop();//추적 중지
        animator.SetTrigger("IsDie");

        //아래의 코드는 죽은 몬스터 자신의 모든 Collider를 비활성화 한다.
        //GetComponentInChildren와 GetComponentsInChildren의 차이에 주목하자 Components는 배열을 리턴한다.
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;
        foreach(Collider coll in gameObject.GetComponentsInChildren<CapsuleCollider>())
        {
            coll.enabled = false;
        }

        //gameUI에 점수를 올려달라 요청
        gameUI.DispScore(50);

        StartCoroutine(this.PushObjectPool());//죽은 몬스터를 오브젝트 풀로 돌려주는 코루틴 함수 호출
    }

    void CreateBloodEffect(Vector3 pos)//인자로 총알이 맞은 위치를 넘겨받는다.
    {
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 2.0f); // 2초후에 혈흔 효과가 사라짐

        //데칼효과
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f); // 혈흔 데칼효과는 몬스터의 pivot위치에 생성하는데 floor랑 y위치가 같으면 안되므로 살짝 위로 올린다.
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));//혈흔 프리맵 자체가 x축을 기준으로 90도 회전해야 바닥에 평행하게 생성되므로 90도를 돌리고 z축 기준으로 랜덤한 각도로 회전시킨다.

        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        float scale = Random.Range(1.5f, 3.5f); // 데칼의 크기를 랜덤하게
        blood2.transform.localScale = Vector3.one * scale;

        Destroy(blood2, 5.0f); //5초후에 삭제
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();

        nvAgent.Stop();
        animator.SetTrigger("IsPlayerDie");
    }

    //PlayerCtrl class의 Delegate변수 OnPlayerDie에 접근해 자신의 함수를 더하고 빼줌으로써 이벤트를 연결하고 해제한다.
    //OnEnable과 OnDisable은 스크립트가 활성화되거나 비활성화될 때 자동으로 수행되는 함수이다.
    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;//이벤트 연결


        //오브젝트 풀로 생성할 경우 활성화 될 때 호출되는 OnEnable에서 코루틴 함수를 호출해야한다.
        StartCoroutine(this.CheckMonsterState());

        StartCoroutine(this.MonsterAction());
    }
    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;//이벤트 해제
    }

    void OnDamage(object[] _params)//send message로 수행할 함수
    {
        CreateBloodEffect((Vector3)_params[0]); //혈흔효과 함수 호출

        hp -= (int)_params[1]; //데미지 만큼 차감후 죽었는지 판단
        if(hp <= 0)
        {
            MonsterDie();
        }

        animator.SetTrigger("IsHit");// 피격효과를 보여주기 위해 애니메이터에 트리거 전달
    }
}
