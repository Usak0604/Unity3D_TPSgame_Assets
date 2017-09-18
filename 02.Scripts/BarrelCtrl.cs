using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour {
    //폭발효과 변수
    public GameObject expEffect;
    private Transform tr;

    public Texture[] texture;

    private int hitCount = 0;

	// Use this for initialization
	void Start () {
        tr = GetComponent<Transform>();

        //드럼통의 텍스쳐를 랜덤으로 입히기 위하여
        int idx = Random.Range(0, texture.Length);
        //mesh의 경우에는 최상위 Parent인 Barrel의 자식에게 존재하는 component이므로 Children에 접근해야함.
        GetComponentInChildren<MeshRenderer>().material.mainTexture = texture[idx];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.tag == "BULLET")
        {
            Destroy(coll.gameObject);

            if (++hitCount >= 3) // 3방 이상 명중시 폭발함
                ExpBarrel();
        }
    }
    void ExpBarrel()
    {
        Instantiate(expEffect, tr.position, Quaternion.identity);

        //지정반경(10.0f)내의 모든 Collider 객체를 가져와서 폭발의 데미지를 입힌다.
        Collider[] colls = Physics.OverlapSphere(tr.position, 10.0f);

        foreach(Collider coll in colls)
        {
            Rigidbody rbody = coll.GetComponent<Rigidbody>();
            if(rbody != null)
            {
                rbody.mass = 1.0f;
                //(폭발력, 원점, 반경, 솟구치는힘)
                rbody.AddExplosionForce(1000.0f, tr.position, 10.0f, 300.0f);
            }
        }

        Destroy(gameObject, 5.0f);
    }

    void OnDamage(object[] _params)//Ray에 맞았을 때, 물리력 작용
    {
        Vector3 firePos = (Vector3)_params[0];
        Vector3 hitPos = (Vector3)_params[1]; // 발사 원점과 맞은 위치 뽑아냄

        Vector3 incomeVector = hitPos - firePos; // 두점을 잇는 벡터
        incomeVector = incomeVector.normalized;// 정규화

        GetComponent<Rigidbody>().AddForceAtPosition(incomeVector * 1000.0f, hitPos); // 맞은 자리에 입사벡터의 각으로 힘을 가한다.

        if(++hitCount >= 3)
        {
            ExpBarrel();
        }//맞은 횟수가 3회 이상이면 폭발
    }
}
