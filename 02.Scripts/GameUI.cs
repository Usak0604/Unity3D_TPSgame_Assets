using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    public Text txtScore;
    private int totScore = 0;

	// Use this for initialization
	void Start () {
        totScore = PlayerPrefs.GetInt("TOT_SCORE", 0); //저장되어 있는 값가져옴.
        DispScore(0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DispScore(int score)
    {
        int ZeroCount = 3;
        string zero = "";
        totScore += score;
        if (totScore >= 1000)
            ZeroCount = 0;
        else if (totScore >= 100)
            ZeroCount = 1;
        else if (totScore >= 10)
            ZeroCount = 2;
        for (int i = 0; i < ZeroCount; i++)
            zero += "0";
        txtScore.text = "SCORE <color=#ff0000>" + zero + totScore.ToString() + "</color>";

        PlayerPrefs.SetInt("TOT_SCORE", totScore); //score를 저장한다. 이 저장법은 보안성이 제로에 가깝다. 누구나 수정할 수 있는 파일의 형태로 저장되기 때문, 암호화
    }
}
