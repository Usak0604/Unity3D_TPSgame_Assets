using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCtrl : MonoBehaviour {
    public GameObject sparkEffect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.tag == "BULLET")
        {
            //벽과 총알이 부딪힌 곳에 스파크 파티클을 생성한다. 세번째 인자는 회전으로 아래와 같이 설정하면 회전 없이 설정된다.
            //spark변수를 만든 이유는 없이 생성만 한다면 사라지지를 않는다. 따라서 일정 시간후에 사라지게 만들기 위해.
            GameObject spark = (GameObject)Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);

            Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f); // 0.2초 후에 사라지도록 생성한다. 책에는 중간의 main이 없는데 없이 쓰는 방법은 디플리케이트 되었다.
            Destroy(coll.gameObject);
        }
    }
}
