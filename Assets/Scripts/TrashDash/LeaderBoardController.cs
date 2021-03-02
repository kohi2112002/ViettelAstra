using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LeaderBoardController : MonoBehaviour
{
    [SerializeField] private LeaderBoardElement[] leaderBoardElements;
    void Start()
    {
        var datas = DataManager.Instance.dataCollection.datas;
        for (int i = 0; i < datas.Length; i++)
            leaderBoardElements[i].Init(datas[i].name, datas[i].score.ToString());
    }
    public void OnClickBack()
    {
        MySceneManager.Instance.LoadScene("Menu");
    }
}
