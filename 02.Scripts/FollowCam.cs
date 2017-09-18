using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour { // 카메라가 캐릭터를 따라가기 위한 스크립트이다. 그냥 Plyaer의 차일드에 camera를 넣어놓으면 같이 움직이기는 하는데 부드러운 움직임과 여러 효과를 주는게 불가능하다.
    public Transform targetTr; //타겟
    public float dist = 10.0f; // 타겟과의 일정 거리
    public float height = 3.0f; // 카메라 높이 설정
    public float dampTrace = 20.0f; //부드러운 추적을 위한 변수
    private Transform tr; //본인(camera)

	// Use this for initialization
	void Start () {
        tr = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //추적 타겟의 이동이 끝난 후에 추적하기 위해 LateUpdate에 정의한다. Update에 구현할 경우 움직임이 완료되기도 전에 카메라가 이동해 떨림 현상이 생길 수 있다.
    void LateUpdate()
    {
        //Lerp 함수 : 첫번째와 두번째 벡터를 선형보간하는 함수
        tr.position = Vector3.Lerp(tr.position, //시작 위치 벡터
            targetTr.position - (targetTr.forward * dist) + (Vector3.up * height), //종료위치 : 타겟의 위치에서 dist만큼 물러난 후 height만큼 높이를 올려주고
            Time.deltaTime * dampTrace); //보간시간으로써, 이 함수 역시 시간을 사용해야 프레임에 따라 다른 결과를 보지 않을 수 있다.
        //시간은 0이면 시작 위치를, 1이면 종료위치를 반환한다. dampTrace를 조절할 경우 추적감도를 조정할 수 있다. 크게 변하지는 않는듯 싶다.
        tr.LookAt(targetTr.position + (targetTr.forward * 5)); // 타켓을 바라보게 한다. 뒤의 5를 곱한 벡터는 본인이 임의로 추가한 코드로써 이 코드로 시야가 좀더 넓어지게 된다.
    }
}
