using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EndPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fishCounter, skullCounter, total;
    [SerializeField] private GameObject skull, deco, subtract, namePanel;
    private int score;
    private string playerName = "Tee";
    private void OnDisable()
    {
        subtract.SetActive(false);
        deco.SetActive(false);
        this.total.text = System.String.Format("{0:D4}", 0);
        this.total.gameObject.SetActive(false);
        skull.SetActive(false);
        skullCounter.text = "<size=26>100x</size> " + 0;
        skullCounter.gameObject.SetActive(false);
    }
    public void SetUpPanel(int fishBone, int death, int total, bool isHighScore)
    {
        gameObject.SetActive(true);
        score = total;
        StartCoroutine(DisplayScore(fishBone, death, total, isHighScore));
    }
    private IEnumerator DisplayScore(int fishBone, int death, int total, bool isHighScore)
    {
        int tmp = 0;
        for (int i = 0; i < 60; i++)
        {
            yield return null;
            tmp += fishBone / 60;
            fishCounter.text = System.String.Format("{0:D4}", tmp);
        }
        fishCounter.text = System.String.Format("{0:D4}", fishBone);
        yield return null;
        skullCounter.gameObject.SetActive(true);
        skullCounter.text = "<size=26>10x</size> " + death;
        skull.SetActive(true);
        yield return null;
        subtract.SetActive(true);
        deco.SetActive(true);
        yield return null;
        this.total.gameObject.SetActive(true);
        tmp = 0;
        for (int i = 0; i < 60; i++)
        {
            yield return null;
            tmp += total / 60;
            this.total.text = System.String.Format("{0:D4}", tmp);
        }
        this.total.text = System.String.Format("{0:D4}", total);
        if (isHighScore)
        {
            yield return new WaitForSeconds(2);
            namePanel.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(4);
            MySceneManager.Instance.LoadScene("Menu");
        }
    }
    public void OnFinishEditName(string name)
    {
        playerName = name;
    }
    public void OnClickSaveScore()
    {
        DataManager.Instance.SaveScore(playerName, score);
        MySceneManager.Instance.LoadScene("LeaderBoard");
    }
}
