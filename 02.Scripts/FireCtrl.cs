using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//총알이 발사되는 작업을 하는 스크립트로써 PlayerCtrl스크립트에 추가해도 되지만 일단은 따로 생성한다.
[RequireComponent(typeof(AudioSource))]//반드시 필요한 컴포넌트가 삭제되는 것을 방지한다 : 플레이어가 벽에 붙어서 클릭할 경우 총알이 생성되면서 파괴되므로 소리가 재생되지 않는 현상을 바로잡기 위한 것.
public class FireCtrl : MonoBehaviour {
    //이 변수들의 접근지정자가 public으로 Inspector에서 지정해준 후에 사용한다.
    //연결할 때 주의점으로는 bullet에는 Project탭에 있는 프리맵을 연결해야 한다는 것이다. Hierarchy에 있는 Bullet과 연결할 경우 Null 에러가 난다.
    public GameObject bullet; //총알 프리맵
    public Transform firePos; //총알이 발사되는 시작 좌표

    public AudioClip fireSfx;
    //이 source라는 변수는 사운드 공용함수를 사용하게 되면서 필요없는 변수가 되었다.
    //private AudioSource source = null; // 맨위의 RequireComponent때문에 이 변수가 참조하는 컴포넌트는 삭제가 불가능하다.

    public MeshRenderer muzzleFlash; // 총구화염을 위한 컴포넌트이다. 컴포넌트를 짧은 시간동안 활성화/비활성화 하는 방식으로 표현

    void Start()
    {
        //source = GetComponent<AudioSource>();//오디오 소스 컴포넌트 추출
        muzzleFlash.enabled = false;
    }
    
	// Update is called once per frame
	void Update () {
        Debug.DrawRay(firePos.position, firePos.forward * 10.0f, Color.red);//레이캐스트를 이용하기에 앞서 개발 단계에서 Ray를 시각적으로 확인하게 해주는 것.

        if (Input.GetMouseButtonDown(0)) //마우스를 눌렀을 때 발생하는 이벤트 함수이며 인자의 경우 0: 왼쪽버튼, 1: 오른쪽버튼, 2: 가운데버튼
        {
            Fire();

            //여기서부터는 레이캐스트를 이용한 레이저총
            RaycastHit hit; //Ray에 맞은 오브젝트의 정보를 받아올 변수.

            if(Physics.Raycast(firePos.position, firePos.forward, out hit, 10.0f))//레이캐스트 함수, 총구위치에서 앞으로 10.0f 내에서 맞은 오브젝트가 있는지 true/false
            {
                if(hit.collider.tag == "MONSTER")//특이하게도 맞은 오브젝트의 태그를 확인하기 위해서는 collider로 접근해야한다.
                {
                    object[] _params = new object[2]; //전달할 인자를 담은 변수
                    _params[0] = hit.point; //레이에 맞은 정확한 위치값
                    _params[1] = 20; //레이의 데미지

                    //위의 인자를 넣어서 레이를 맞은 몬스터에게 메시지를 보낸다.
                    //레이를 이용한 것은 실제 충돌을 일으키지 않으므로 총맞았으니까 hp를 감소시켜라고 알려야만함
                    //첫번째 인자는 수행할 함수의 이름이고 두번째 인자가 그 함수에 전달할 인자다.
                    hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
                }

                if(hit.collider.tag == "BARREL")
                {
                    //드럼통에 맞은 Ray의 입사각 계산을 위해 원점과 맞은지점을 전달한다.
                    object[] _params = new object[2]; //전달할 인자를 담은 변수
                    _params[0] = firePos.position;//원점(총구)
                    _params[1] = hit.point;//맞은 지점
                    
                    hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
	}
    void Fire()
    {
        //레이캐스팅을 위해 총알을 만드는 함수는 주석처리한다.
        //CreateBullet();

        //오디오재생 (오디오 클립, 재생볼륨)
        //source.PlayOneShot(fireSfx, 0.9f);
        //오디오 재생을 사운드 공용함수를 이용해 수행한다.
        GameMgr.instance.PlaySfx(firePos.position, fireSfx); // 인자로 발사 위치와 소리 클립.

        //잠시 기다리는 루틴을 위해 코루틴 함수 호출 -> 코루틴 함수 공부필요
        StartCoroutine(this.ShowMuzzleFlash());
    }
    void CreateBullet()
    {
        //프리맵이나 게임오브젝트의 복사본을 만드는 함수 (프리맵, 생성위치, 각도)
        Instantiate(bullet, firePos.position, firePos.rotation);
    }
    IEnumerator ShowMuzzleFlash()
    {
        //좀더 실감나는 효과
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale; //크기를 불규칙하게 변경

        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360)); // 오일러 회전행렬과 정규화 회전행렬 -> 공부필요
        muzzleFlash.transform.localRotation = rot; // z축을 중심으로 불규칙적으로 회전시킴
        //위와같은 방법은 다른 게임오브젝트 아래에 차일드화 된 게임오브젝트의 Transform 컴포넌트 속성을 수정하려면 localScale, localRotation 등을 사용해야한다.

        muzzleFlash.enabled = true;

        //불규칙한 시간 동안 Delay
        //코루틴 함수에서 주어진 시간만큼 잠시 멈춤 기능을 제공하게 된다.
        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));

        muzzleFlash.enabled = false;
    }
}
