using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 Mesh가 없는 transform을 조작하기 위해 편리한 에디터에서만 보이는 구체를 생성한다.
public class MyGizmo : MonoBehaviour {
    public Color _color = Color.yellow;
    public float _radius = 0.1f;

    void OnDrawGizmos()
    {
        Gizmos.color = _color;

        //Gizmo의 위치와 반지름을 지정한다.
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
