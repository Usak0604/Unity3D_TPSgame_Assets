using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour {
    //파괴력
    public int damage = 20;
    //발사 속도
    public float speed = 1000.0f;

	// Use this for initialization
	void Start () {
        //생성됨과 동시에 z축방향으로 날라감. Bullet 컴포넌트의 rigidbody를 찾아서 힘을 가함(AddForce)
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
