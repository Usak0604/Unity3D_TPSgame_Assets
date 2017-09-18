using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour {
    public Transform[] points; //������ GameObject
    public GameObject monsterPrefab;
    public List<GameObject> monsterPool = new List<GameObject>(); //������Ʈ Ǯ�� Ȱ���� ���͸� �����ϵ��� �Ѵ�. -> �̸� �ν��Ͻ��� �������� Ȱ��ȭ, ��Ȱ��ȭ �ϸ鼭 �����ϴ� ��ó�� �����ش�. ������� ����ȴ�. (�Ѿ˿���?)

    public float createTime = 2.0f; //���� �ֱ�
    public int maxMonster = 10;//�ִ� ���� ��
    public bool isGameOver = false;

    public float sfxVolumn = 1.0f;
    public bool isSfxMute = false; // ���� �Ŵ����� ��ü ���� ����, ���ҰŸ� ����ϰ� �ȴ�. �̷� ������ ���� ������ �ΰ� ���尡 �߻��� ������ �� ���� ������ ó���ϸ� �ڵ�絵 �ð� ���� ���鿡�� ��ִ�.

    public static GameMgr instance = null; //�ٸ� ������Ʈ�� �Ŵ����� ������ ��. �Ź� ������ ������ �Ҵ��ϸ� ���ŷο�Ƿ� �̱��� �������� �����ϵ��� �Ѵ�.

    void Awake()
    {
        instance = this; //�̱��� ���� ����. �� �Լ��� ���ο��� �ڵ����� ����Ǵ� �Լ��μ� �̷��� �Ǹ� �ٸ� ��ũ��Ʈ���� ������ ��, GameMgr.instance�� �����ϸ� class ����, �Լ��鿡 ������ �� �ִ�.
    }

	// Use this for initialization
	void Start () {
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();//�� �۾��� ��ġ�� position 0,0,0�� �θ� SpawnPoint�� �ε��� 0�� �߰��ȴ�. �׷��Ƿ� ���� ���� �ε����� 1���� ������.

        for(int i = 0; i<maxMonster; i++)
        {
            //���� ������ ����
            GameObject monster = (GameObject)Instantiate(monsterPrefab);

            monster.name = "Monster_" + i.ToString();
            monster.SetActive(false);
            monsterPool.Add(monster); //�������� �������� �̸��� Monster_1�� ���� ��Ȱ��ȭ �ؼ� �����س��� pool�� ��� ���´�.
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
            ////������Ʈ Ǯ�� ������� �ʾ��� �� ���� ���� ��ƾ
            //int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length; //���� ������ ���� ��

            //if(monsterCount < maxMonster) // �̷��� ������ ���� �ջ��� ��� �̹� ���� ���͵� üũ�Ǿ� ���̻� �������� �ʴ´�. ���� ���Ͱ� ���� ��� ������ �±׸��� �ٲ۴�.
            //{
            //    yield return new WaitForSeconds(createTime);

            //    int idx = Random.Range(1, points.Length);
            //    Instantiate(monsterPrefab, points[idx].position, points[idx].rotation); //�������� ���� ���������� ��ġ�� ���� �������� �̿��� ����
            //}
            //else
            //{
            //    yield return null;
            //}
            yield return new WaitForSeconds(createTime); //�ð� ���
            if (isGameOver) yield break; // �ð� ��� �� ������ �����ٸ� �ڷ�ƾ�� �����Ѵ�.

            foreach(GameObject monster in monsterPool)
            {
                if (!monster.activeSelf) // Ȱ��ȭ �Ǿ� �ִ��� �ƴ����� ��밡���� �������� �Ǵ�.
                {
                    int idx = Random.Range(1, points.Length);
                    monster.transform.position = points[idx].position;
                    monster.SetActive(true);//��ġ�� �ű�� Ȱ��ȭ�ؼ� ��ȯ�� ���� ������ �ش�.
                    break; // �ϳ��� Ȱ��ȭ �ߴٸ� �ٽ� 2�ʸ� ��ٷ����ϹǷ� break�� ����������.
                }
            }
        }
    }

    //���� �����Լ�. �� �����Լ��� ���� ����, ���ҰŸ� �����ϰ� �ȴ�.
    public void PlaySfx(Vector3 pos, AudioClip sfx)
    {
        if (isSfxMute) return; //���Ұ��� ��� �ƹ� �۾��� ����.

        //���� ������Ʈ�� ���� "����"���� �����Ѵ�. ���带 ����� ���̴�.
        GameObject soundObj = new GameObject("Sfx");
        soundObj.transform.position = pos;

        AudioSource audioSource = soundObj.AddComponent<AudioSource>();//���� ������ ������Ʈ�� ����� �ҽ� ������Ʈ�� �߰��ϰ� �װ��� ����Ű�� ������ �ϳ� �д�.

        audioSource.clip = sfx; //Ŭ���� �Լ��� ���ڷ� ���� Ŭ���� �ִ´�. ��, �ٸ� ��ũ��Ʈ���� �� �Լ��� ȣ���ϸ鼭 �����ϰ����ϴ� Ŭ���� �ְ� �Ҹ��� �߿����� pos�� ������ �Ҹ��� ����ȴ�.
        audioSource.minDistance = 10.0f; // public ������ ������ ���� �����ٵ�
        audioSource.maxDistance = 30.0f;

        audioSource.volume = sfxVolumn;//���� ����. �� ������ ���������ϸ� �� ���� �ϳ������� ��� �Ҹ��� ũ�⸦ ������ �� �ִ�.

        audioSource.Play(); // �Ҹ� ���.

        Destroy(soundObj, sfx.length);//�Ҹ� ���̸�ŭ ����ǰ� ���� ������ ������Ʈ�� �����Ѵ�.
    }
}
