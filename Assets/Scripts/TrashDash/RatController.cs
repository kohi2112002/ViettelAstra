using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatController : Obstacle
{
    protected const float k_LaneOffsetToFullWidth = 2f;
    private Animator anim;
    private bool isMoving = false;
    protected float maxSpeed, currentPos;
    protected Vector3 m_OriginalPosition = Vector3.zero;
    private void OnEnable()
    {
        isMoving = true;
        anim = GetComponent<Animator>();
        m_OriginalPosition = transform.localPosition - transform.forward * TrackSegment.LANE_OFFSET;
        transform.localPosition = m_OriginalPosition;
        float actualTime = Random.Range(2, 4);
        maxSpeed = (TrackSegment.LANE_OFFSET * k_LaneOffsetToFullWidth * 2) / actualTime;
        AnimationClip clip = anim.GetCurrentAnimatorClipInfo(0)[0].clip;
        anim.SetFloat("SpeedRatio", clip.length / actualTime);
    }
    public override GameObject[] Spawn(Vector3 position, Transform parent)
    {
        GameObject go = Instantiate(gameObject, position, Quaternion.Euler(0, 90, 0), parent);
        return new GameObject[] { go };
    }
    private void Update()
    {
        if (isMoving)
        {
            currentPos += Time.deltaTime * maxSpeed;
            transform.localPosition = m_OriginalPosition + transform.forward * Mathf.PingPong(currentPos, TrackSegment.LANE_OFFSET * k_LaneOffsetToFullWidth);
        }
    }
    public override void PlayCollisionAnim()
    {
        isMoving = false;
        anim.SetTrigger("Dead");
        Destroy(gameObject, 1f);
    }
}
