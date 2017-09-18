using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMgr : MonoBehaviour {

    public void OnClickStartBtn()
    {
        Debug.Log("Click Button");

        //신 매니저를 통해 신을 로드한다.
        SceneManager.LoadScene("scLevel01");
        SceneManager.LoadScene("scPlay", LoadSceneMode.Additive); // 이 옵션은 추가로드로 먼저 로드한 씬에 새로운 신을 병합해서 로드한다. LoadSceneMode.Single로 옵션을 주면 전까지 로드한 신을 모두 삭제한 후에 로드된다.
    }
}
