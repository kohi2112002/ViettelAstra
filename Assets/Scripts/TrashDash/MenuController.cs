using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MenuController : MonoBehaviour
{
    public void OnClickPlay()
    {
        MySceneManager.Instance.LoadScene("Main");
    }
    public void OnClickLeaderBoard()
    {
        MySceneManager.Instance.LoadScene("LeaderBoard");
    }
}
