using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour {
    public Transform[] points; //스폰될 GameObject
    public GameObject monsterPrefab;
    public List<GameObject> monsterPool = new List<GameObject>(); //오브젝트 풀을 활용해 몬스터를 생성하도록 한다. -> 미리 인스턴스를 만들어놓고 활성화, 비활성화 하면서 생성하는 것처럼 보여준다. 성능향상에 도움된다. (총알에도?)

    public float createTime = 2.0f; //스폰 주기
    public int maxMonster = 10;//최대 몬스터 수
    public bool isGameOver = false;

    public float sfxVolumn = 1.0f;
    public bool isSfxMute = false; // 게임 매니저로 전체 게임 음량, 음소거를 담당하게 된다. 이런 변수를 전역 변수로 두고 사운드가 발생할 때마다 그 값을 참조해 처리하면 코드양도 늘고 관리 측면에서 장애다.

    public static GameMgr instance = null; //다른 오브젝트가 매니저에 접근할 때. 매번 변수를 생성해 할당하면 번거로우므로 싱글턴 패턴으로 접근하도록 한다.

    void Awake()
    {
        instance = this; //싱글턴 패턴 적용. 이 함수는 내부에서 자동으로 실행되는 함수로서 이렇게 되면 다른 스크립트에서 접근할 때, GameMgr.instance에 접근하면 class 변수, 함수들에 접근할 수 있다.
    }

	// Use this for initialization
	void Start () {
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();//이 작업을 거치면 position 0,0,0인 부모 SpawnPoint도 인덱스 0에 추가된다. 그러므로 밑의 랜덤 인덱스를 1부터 돌린다.

        for(int i = 0; i<maxMonster; i++)
        {
            //몬스터 프리맵 생성
            GameObject monster = (GameObject)Instantiate(monsterPrefab);

            monster.name = "Monster_" + i.ToString();
            monster.SetActive(false);
            monsterPool.Add(monster); //프리맵을 바탕으로 이름을 Monster_1로 짓고 비활성화 해서 생성해놓고 pool에 담아 놓는다.
        }

        if(points.Length > 0)
        {
            StartCoroutine(this.CreateMonster());
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator CreateMonster()
    {
        while (!isGameOver)
        {
            ////오브젝트 풀을 사용하지 않았을 때 몬스터 생성 루틴
            //int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length; //현재 생성된 몬스터 수

            //if(monsterCount < maxMonster) // 이렇게 몬스터의 수를 합산할 경우 이미 죽은 몬스터도 체크되어 더이상 스폰되지 않는다. 따라서 몬스터가 죽을 경우 몬스터의 태그명을 바꾼다.
            //{
            //    yield return new WaitForSeconds(createTime);

            //    int idx = Random.Range(1, points.Length);
            //    Instantiate(monsterPrefab, points[idx].position, points[idx].rotation); //랜덤으로 뽑은 스폰지역의 위치에 몬스터 프리맵을 이용해 생성
            //}
            //else
            //{
            //    yield return null;
            //}
            yield return new WaitForSeconds(createTime); //시간 대기
            if (isGameOver) yield break; // 시간 대기 후 게임이 끝났다면 코루틴을 종료한다.

            foreach(GameObject monster in monsterPool)
            {
                if (!monster.activeSelf) // 활성화 되어 있는지 아닌지로 사용가능한 몬스터인지 판단.
                {
                    int idx = Random.Range(1, points.Length);
                    monster.transform.position = points[idx].position;
                    monster.SetActive(true);//위치를 옮기고 활성화해서 소환된 듯한 느낌을 준다.
                    break; // 하나를 활성화 했다면 다시 2초를 기다려야하므로 break로 빠져나간다.
                }
            }
        }
    }

    //사운드 공용함수. 이 공용함수를 통해 음량, 음소거를 관리하게 된다.
    public void PlaySfx(Vector3 pos, AudioClip sfx)
    {
        if (isSfxMute) return; //음소거일 경우 아무 작업도 안함.

        //게임 오브젝트를 새로 "동적"으로 생성한다. 사운드를 담당할 것이다.
        GameObject soundObj = new GameObject("Sfx");
        soundObj.transform.position = pos;

        AudioSource audioSource = soundObj.AddComponent<AudioSource>();//동적 생성한 오브젝트에 오디오 소스 컴포넌트를 추가하고 그것을 가리키는 변수도 하나 둔다.

        audioSource.clip = sfx; //클립은 함수의 인자로 들어온 클립은 넣는다. 즉, 다른 스크립트에서 이 함수를 호출하면서 수행하고자하는 클립을 넣고 소리의 발원지를 pos로 넣으면 소리가 재생된다.
        audioSource.minDistance = 10.0f; // public 변수로 조정할 수도 있을텐데
        audioSource.maxDistance = 30.0f;

        audioSource.volume = sfxVolumn;//볼륨 설정. 이 볼륨을 전역설정하면 이 변수 하나만으로 모든 소리의 크기를 조정할 수 있다.

        audioSource.Play(); // 소리 재생.

        Destroy(soundObj, sfx.length);//소리 길이만큼 재생되고 동적 생성한 오브젝트를 제거한다.
    }
}
