using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] // 이 속성을 추가해줘야 클래스 타입의 변수가 Inspector에 노출이 된다.
public class Anim
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}
public class PlayerCtrl : MonoBehaviour {
    public int hp = 100; //Player 생명력
    private int initHp; // 초기 생명력 설정
    public Image imgHpBar; // 표시할 이미지

    private float h = 0.0f;
    private float w = 0.0f;

    //접근해야 하는 컴포넌트 (여기서는 Player인듯 하다.)
    private Transform tr;

    //public으로 선언하면 다른 스크립트에서 접근해 속도를 바꿀수 있는 것. 스크립트보다 Inspector에서 설정한 값이 더 우선순위가 높다.
    public float moveSpeed = 10.0f;

    //화면 회전 속도 변수(마우스 관련)
    public float rotSpeed = 100.0f;

    public Anim anim; // Inspector에 노출 시킬 애니메이션 클립의 클래스
    public Animation _animation; // Player 컴포넌트의 하위 컴포넌트인 Animation에 접근하기 위한 변수.

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie; // 이벤트 변수

    //private GameMgr gameMgr;//게임 매니저에 접근하기 위한 변수

	// Use this for initialization
	void Start () {
        initHp = hp; //초기 HP 초기화

        tr = GetComponent<Transform>();

        _animation = GetComponentInChildren<Animation>();//애니메이션이 여러개 존재할 때는 다른 방식으로 가져와야한다.

        _animation.clip = anim.idle; // 기본 클립을 idle로 지정 (idle이 초기화가 안됐다고 생각할 수 있는데, Inspector에서 드래그앤 드롭으로 초기화 되어있다.)
        _animation.Play();

        //gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>(); //스크립트의 클래스에 접근하게 된다.
	}
	
	// Update is called once per frame
	void Update () {
        h = Input.GetAxis("Horizontal");
        w = Input.GetAxis("Vertical");
        
        //Debug.Log("H= " + h.ToString());
        //Debug.Log("W= " + w.ToString());

        Vector3 moveDir = (Vector3.forward * w) + (Vector3.right * h); // 두개의 방향 벡터와 크기를 곱한 후 더해서 통합적인 이동을 관리할 수 있다.

        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);//moveDir을 그냥 사용하면 두개의 키가 동시에 눌리면 속도가 엄청 빨라지므로 normalize해서 사용.
        //반대로 크기를 알기위해서는 Vector3.Magnitude(Vector3.forward) 를 사용한다.

        #region 이동 제어 시작
        //이동방향 * 속도 * 변위값 * Time, 기준좌표
        //Vector3.forward 자체가 (0,0,1)을 뜻하는 것으로 속도와 변위를 곱하면 앞으로 이동하게 된다.
        //deltaTime을 곱하는건 이전 프레임과 현재 프레임의 차이만큼이다. 곱해줘야만 프레임에 상관없이 일정한 속도로 이동한다.
        //Space.Self를 지정하면 자신 컴포넌트의 Local 좌표를 기준으로 좌표를 이동한다. 즉 이 코드는 바라보는 방향으로 이동한다. Space.World를 지정하면 Grobal좌표를 기준으로 움직인다.

        //tr.Translate(Vector3.forward * moveSpeed * w * Time.deltaTime, Space.Self);
        #endregion

        //회전 기준좌표 * Time * 회전속도 * 변위 입력값(마우스의 X 축 이동)
        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        if(w >= 0.1f)
        {
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if (w <= -0.1f)
        {
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        {
            _animation.CrossFade(anim.idle.name, 0.3f);
        }//각 움직임을 일으키는 키입력에 따라 애니메이션을 변경한다.
        //CrossFade의 경우 첫째는 애니메이션 클립의 이름, 두번째는 페이드 아웃되는 시간이다(단위는 초).
    }

    void OnTriggerEnter(Collider coll) //충돌 Trigger가 발생했을 때, 수행되는 함수
    {
        if(coll.gameObject.tag == "PUNCH")
        {
            hp -= 10;

            imgHpBar.fillAmount = (float)hp / (float)initHp;

            Debug.Log("Player HP = " + hp.ToString());

            if(hp <= 0)
            {
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        Debug.Log("Player Die !!");
        /*
                //태그로 게임 내 존재하는 모든 오브젝트를 가져온다.
                GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

                //send message로 모든 몬스터 오브젝트의 OnPlayerDie라는 함수를 호출할 수 있게된다. 두번째 인자는 option으로 아래의 옵션은 리턴을 받지 않겠다는 뜻.
                //하지만 이방식으로 구현할 경우 몬스터가 매우 많다면 아주 비효율적인 호출이 된다. delegate를 이용해 이벤트를 처리하듯 사용한다.
                foreach (GameObject monster in monsters)
                {
                    monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
                }
        */
        OnPlayerDie(); // 이벤트를 발생시키기 위한 것.

        //싱글턴 패턴을 적용해. 이렇게 클래스에 바로 접근할 수 있다.
        GameMgr.instance.isGameOver = true; //게임 오버 되었음을 매니저에게 알려 더이상의 스폰을 막는다.
    }
}
