using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
public class TutorialController : MonoBehaviour
{
    [SerializeField] private AudioSource countDown;
    [SerializeField] private PlayableDirector detectTimeline, finishTutorial;
    private AstraInputController astraInputController;
    private bool hasJump = false, hasDetectHumanShape = false;
    public bool hasCompleteTut = false;
    [SerializeField] private CenterAlignTutorial centerAlign;
    [SerializeField] private TextMeshProUGUI instruction;
    [SerializeField] private GameObject footPrint;
    private void Start()
    {
        astraInputController = FindObjectOfType<AstraInputController>();
        instruction.text = "đứng cách camera 2.5m để bắt đầu";
        astraInputController.onDetectBody += OnDetectBody;
    }
    private IEnumerator TutorialLoop()
    {
        instruction.text = "";
        detectTimeline.Play();
        var delay = new WaitForSeconds(1);
        yield return new WaitForSeconds(3);
        while (!centerAlign.IsPass())
            yield return delay;
        detectTimeline.Stop();
        Destroy(detectTimeline.gameObject);
        footPrint.SetActive(false);
        finishTutorial.Play();
        yield return new WaitForSeconds(2);
        finishTutorial.Stop();
        Destroy(finishTutorial.gameObject);
        yield return new WaitForSeconds(1);
        instruction.fontSize = 120;
        instruction.text = "4";
        int count = 4;
        countDown.Play();
        while (count > 0)
        {
            instruction.text = count.ToString();
            yield return new WaitForSeconds(0.6f);
            count--;
        }
        yield return new WaitForSeconds(0.3f);
        instruction.text = "Go";
        astraInputController.onDetectBody -= OnDetectBody;
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        hasCompleteTut = true;
        instruction.text = "";
    }
    private void OnDetectBody(bool status, Vector3 bodyPos)
    {
        if (status)
        {
            if (!hasDetectHumanShape)
            {
                StartCoroutine(TutorialLoop());
                hasDetectHumanShape = true;
            }
            if (!hasCompleteTut)
                transform.position = Vector3.Lerp(transform.position, new Vector3(bodyPos.x * 2, 0, -bodyPos.z), Time.deltaTime * 3);
        }
    }
}
