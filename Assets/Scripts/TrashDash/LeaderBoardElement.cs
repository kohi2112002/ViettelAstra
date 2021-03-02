using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LeaderBoardElement : MonoBehaviour
{
    [SerializeField] private Text name, score;
    public void Init(string name, string score)
    {
        gameObject.SetActive(true);
        this.name.text = name;
        this.score.text = score;
    }
}
