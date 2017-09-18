using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {
    //광선을 그리기 위한 스크립트
    private Transform tr;
    private LineRenderer line;

    //광선에 충돌한 게임오브젝트까지 광선을 그려야하므로 맞은 오브젝트를 받아올 변수
    private RaycastHit hit;

	// Use this for initialization
	void Start () {
        tr = GetComponent<Transform>();
        line = GetComponent<LineRenderer>();

        line.useWorldSpace = false; //이것을 true로 놓으면 world좌표를 기준으로 움직이기 때문에 플레이어의 pivot인 발쪽에 생성된다.
        line.enabled = false; // 초기에는 비활성화 시켜놓는다.

        line.startWidth = 0.3f;
        line.endWidth = 0.01f;
        //line.SetWidth(0.3f, 0.01f);// 시작폭과 종료폭을 설정한다. 이 함수는 디플리케이트 되었다.
	}
	
	// Update is called once per frame
	void Update () {
        //광선을 미리 생성해 놓는다.
        Ray ray = new Ray(tr.position + (Vector3.up * 0.02f), tr.forward); //동적 생성, 첫번째 인자는 시작점, 두번째 인자는 방향이듯 하다. 이전에 만든 빨간 레이와 겹치지 않게 y축을 살짝 올린다.

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow); //광선을 눈에 보이게.

        if (Input.GetMouseButtonDown(0))//클릭시
        {
            //아래에서 ray.origin, hit.point, ray.GetPoint(100)을 인자로 받는 InverseTransformPoint 함수의 경우 글로벌 좌표를 로컬 좌표로 바꿔준다. 그 로컬로 바꾸는 기준이 되는 것이 바로 (로컬의 기준).InverseTransformPoint 다.
            line.SetPosition(0, tr.InverseTransformPoint(ray.origin)); //광선의 첫번째 점을 설정한다.

            if(Physics.Raycast(ray, out hit, 100.0f))//어떤 물체에 맞았을 경우
            {//그곳을 끝점을 생성한다.
                line.SetPosition(1, tr.InverseTransformPoint(hit.point));
            }
            else//맞지 않았을 경우 그냥 길이가 100인 광선을 생성한다.
            {
                line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100.0f)));
            }

            StartCoroutine(this.ShowLaserBeam());
        }
	}

    IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;//잠깐 광선이 보이게 하고 다시 비활성화한다.
    }
}
