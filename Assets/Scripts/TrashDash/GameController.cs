using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    private const int GAME_TIME = 60;
    private int skull;
    public int Skull
    {
        get { return skull; }
        set
        {
            skull = value;
            skullCounter.text = "x" + skull;
        }
    }
    private int coin;
    public int Coin
    {
        get { return coin; }
        set
        {
            coin = value;
            if(fishCollectAudio) fishCollectAudio.Play();
            coinCounter.text = System.String.Format("{0:D4}", coin);
        }
    }
    [SerializeField] private TextMeshPro coinCounter, skullCounter, timer;
    [SerializeField] private GameObject hud, startPanel, debugPanel;
    [SerializeField] private EndPanelController endPanel;
    public Transform fishboneTarget;
    private float startTime;
    [SerializeField] private TutorialController tutorialController;
    [SerializeField] private Image detectStatus;
    [SerializeField] private AudioSource fishCollectAudio;
    public bool isDebug;
    void Start()
    {
        StartCoroutine(GameLoop());
        AstraInputController astra = FindObjectOfType<AstraInputController>();
        if(astra) astra.onDetectBody += OnDetectBody;
    }
    void Destroy()
    {
        AstraInputController astra = FindObjectOfType<AstraInputController>();
        if (astra) astra.onDetectBody -= OnDetectBody;
    }
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(GameStart());
        yield return StartCoroutine(GamePlay());
        yield return StartCoroutine(GameEnd());
    }
    private IEnumerator GameStart()
    {
        if (isDebug) debugPanel.SetActive(true);
        endPanel.gameObject.SetActive(false);
        startPanel.SetActive(true);
        hud.SetActive(false);
        yield return null;
        while ((tutorialController != null && !tutorialController.hasCompleteTut))
           yield return null;
        FindObjectOfType<PlayerController>().StartRunning();
        startPanel.SetActive(false);
        startTime = Time.time;
        hud.SetActive(true);
    }
    private IEnumerator GamePlay()
    {
        //todo: slow the movement down when player is out of sensor range and display error border.
        while (Time.time - startTime < GAME_TIME)
        {
            yield return new WaitForSeconds(1);
            timer.text = (GAME_TIME + (int)(startTime - Time.time)).ToString();
        }
    }
    private IEnumerator GameEnd()
    {
        hud.SetActive(false);
        yield return null;
        FindObjectOfType<PlayerController>().StopRunning();
        yield return new WaitForSeconds(1.5f);
        int score = Mathf.Clamp(coin - 10 * skull, 0, 10000);
        endPanel.SetUpPanel(coin, skull, score, DataManager.Instance.IsHighScore(score));
    }
    bool currentDetectStatus = false;
    [SerializeField] private float warningThreshold;
    [SerializeField] private Animator warnAnim;
    private void OnDetectBody(bool status, Vector3 bodyPos)
    {
        if (status)
        {
            float xPos = bodyPos.x * 1.5f;
            if (xPos > warningThreshold) warnAnim.SetTrigger("Left");
            else if (xPos < -warningThreshold) warnAnim.SetTrigger("Right");
        }
        if (currentDetectStatus != status)
        {
            currentDetectStatus = status;
            if (currentDetectStatus)
                detectStatus.color = Color.green;
            else
                detectStatus.color = Color.red;
        }
    }
    public float ObstacleSpawnProbability()
    {
        return Random.Range(0.5f, 1.1f) - (Time.time - startTime) / GAME_TIME;
    }
}
