using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5, laneSpeed = 3;
    private int currentLane = 0;
    private float lastTimeSwitchLane, jumpStart, yPos, stunTime;
    private Vector3 laneTargetPos;
    [SerializeField] private Transform playerCollider;
    [SerializeField] private Animator animator;
    public bool jumping = false, isHitted = false, isPause = true;
    private AstraInputController astraInputController;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private AudioSource catJump, catInjured;
    const string hash_hori = "Horizontal", hash_jump = "Jump", hash_jumping = "Jumping";
    bool groundStick = true;


    private IEnumerator Start()
    {
        isPause = true;//stop player to wait for tutorial
        yield return null;
        astraInputController = FindObjectOfType<AstraInputController>();
        if (astraInputController) astraInputController.onDetectBody += OnDetectBody;
    }
    private void Destroy()
    {
        if (astraInputController) astraInputController.onDetectBody -= OnDetectBody;
    }
    void Update()
    {

        if (!isPause)
        {
            if (!isHitted)
            {
                if (Time.time - lastTimeSwitchLane > 0.5f)
                {
                    if (Input.GetAxis(hash_jump) > 0.1f || (astraInputController && astraInputController.Vertical > 0.15f))
                        Jump();
                    else if (Input.GetAxis(hash_hori) > 0.1f)
                        ChangeLane(1);
                    else if (Input.GetAxis(hash_hori) < -0.1f)
                        ChangeLane(-1);
                    groundStick = true;
                    Physics.IgnoreLayerCollision(9, 10, false);
                }
                if (jumping)
                {
                    if (transform.position.z - jumpStart > 3)
                    {
                        jumping = false;
                        animator.SetBool(hash_jumping, false);
                        Physics.IgnoreLayerCollision(9, 10, false);
                        groundStick = true;

                    }
                    else
                    {
                        yPos = Mathf.Sin(Mathf.PI * (transform.position.z - jumpStart) / 3f) * 1f; //*2f
                        Physics.IgnoreLayerCollision(9, 10, true);
                        groundStick = false;
                    }

                }
                transform.Translate(transform.forward * Time.deltaTime * speed);
                playerCollider.localPosition = Vector3.MoveTowards(playerCollider.localPosition, laneTargetPos + new Vector3(0, yPos, 0), Time.deltaTime * laneSpeed);

            }
            else
            {
                stunTime += Time.deltaTime;
                if (stunTime > 1f)
                {
                    isHitted = false;
                    stunTime = 0;
                }
            }
        }
        if (groundStick) playerCollider.transform.localPosition = new Vector3(playerCollider.transform.localPosition.x, 0, playerCollider.transform.localPosition.z); //trung
    }
    private void Jump()
    {
        if (!jumping)
        {
            jumping = true;
            animator.SetBool(hash_jumping, true);
            if (catJump) catJump.Play();
            jumpStart = transform.position.z;
        }
    }
    private void ChangeLane(int value)
    {
        if (currentLane + value > 1 || currentLane + value < -1) return;
        lastTimeSwitchLane = Time.time;
        currentLane += value;
        laneTargetPos = new Vector3(currentLane * 1.5f, 0, 0);
    }
    public void OnPlayerDie()
    {
        isHitted = true;
        jumping = false;
        animator.SetBool(hash_jumping, false);
        animator.SetTrigger("Hit");
        if (catInjured) catInjured.Play();
        FindObjectOfType<GameController>().Skull++;
    }
    public void StartRunning()
    {
        StartCoroutine(_StartRunning());
    }
    private IEnumerator _StartRunning()
    {
        cameraFollow.Follow = true;
        animator.Play("runStart");
        yield return new WaitForSeconds(0.1f);
        isPause = false;
    }
    public void StopRunning()
    {
        isPause = true;
        animator.SetInteger("RandomIdle", Random.Range(0, 5));
        animator.SetTrigger("Idle");
    }
    public void OnDetectBody(bool status, Vector3 bodyPos)
    {
        float xPos = Mathf.Clamp(bodyPos.x * 2.25f, -1.5f, 1.5f);
        if (status)
            laneTargetPos = new Vector3(xPos, 0, 0);
    }
}
