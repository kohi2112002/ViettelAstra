using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class AstraInputController : MonoBehaviour
{
    public const int QUEUE_SIZE = 20;
    [SerializeField] private float detectThresholdDistance = 0.5f, distanceToPerfectSpot = 2.5f, bodyMoveThreshold = 0.25f;
    public System.Action<bool, Vector3> onDetectBody;
    private Vector3 averageChestPos = Vector3.zero, lastChestPos = Vector3.zero;
    private Vector3[] chestPosQueue = new Vector3[QUEUE_SIZE];
    private int currentQueueIndex = 0;
    private float vertical, horizontal;
    public float Vertical
    {
        get { return vertical; }
    }
    public float Horizontal
    {
        get { return horizontal; }
    }
    public float XPos
    {
        get { return lastChestPos.x; }
    }
    private void Start()
    {
        for (int i = 0; i < QUEUE_SIZE; i++)
            chestPosQueue[i] = Vector3.zero;
        averageChestPos = Vector3.zero;
        _bodies = new Astra.Body[Astra.BodyFrame.MaxBodies];
    }
    private Vector3 GetJointWorldPos(Astra.Joint joint)
    {
        return new Vector3(joint.WorldPosition.X / 1000f, joint.WorldPosition.Y / 1000f, joint.WorldPosition.Z / 1000);
    }
    private int firstBodyIndex = -1;
    private long _lastFrameIndex = -1;
    private Astra.Body[] _bodies = new Astra.Body[Astra.BodyFrame.MaxBodies];
    public void OnNewFrame(Astra.BodyStream bodyStream, Astra.BodyFrame frame)
    {
        if (frame.Width == 0 || frame.Height == 0)
            return;
        if (_lastFrameIndex == frame.FrameIndex)
            return;
        _lastFrameIndex = frame.FrameIndex;
        frame.CopyBodyData(ref _bodies);
        firstBodyIndex = GetCurrentBodyIndex();
        if (firstBodyIndex > -1)
        {
            Vector3 worldChestPos = GetJointWorldPos(_bodies[firstBodyIndex].Joints[(int)Astra.JointType.ShoulderSpine]);
            if (onDetectBody != null)
                onDetectBody(true, worldChestPos);
            EnQueue(worldChestPos);
            firstBodyIndex = -1;//reset this var
        }
        else
        {
            if (onDetectBody != null)
                onDetectBody(false, Vector3.zero);
        }
    }
    //filter body which is out of detect range and find the neareast one to the sweet spot.
    //may some people will stand in front of you and the cam so we need to filter this people base on the lastest distance from camera
    private int GetCurrentBodyIndex()
    {
        int index = -1, bodyIndex = -1;
        Vector3 bodyPos;
        float distanceToPerfectSpot = 10, currentDistanceToPerfectSpot, nearestBodyDistance = 10, currentBodyDistance = 0;
        for (int i = _bodies.Length - 1; i > -1; i--)
        {
            if (_bodies[i] == null || _bodies[i].Joints == null)
                continue;
            bodyPos = GetJointWorldPos(_bodies[i].Joints[(int)Astra.JointType.ShoulderSpine]);
            currentDistanceToPerfectSpot = Mathf.Abs(bodyPos.z - this.distanceToPerfectSpot);
            if (currentDistanceToPerfectSpot > detectThresholdDistance)
                continue;
            //detect the nearest body to the lastest tracked body
            currentBodyDistance = Vector3.Distance(bodyPos, lastChestPos);
            if (currentBodyDistance < bodyMoveThreshold && nearestBodyDistance > currentBodyDistance)
            {
                nearestBodyDistance = currentBodyDistance;
                bodyIndex = i;
            }
            if (distanceToPerfectSpot > currentDistanceToPerfectSpot)//find the body nearest to the perfect spot
            {
                distanceToPerfectSpot = currentDistanceToPerfectSpot;
                index = i;
            }
        }
        if (bodyIndex > -1) return bodyIndex;
        return index;
    }
    private void EnQueue(Vector3 pos)
    {
        chestPosQueue[currentQueueIndex] = pos;
        lastChestPos = pos;
        currentQueueIndex = (currentQueueIndex + 1) % QUEUE_SIZE;
        averageChestPos = GetAverageChestPos();
        vertical = lastChestPos.y - averageChestPos.y;
        horizontal = lastChestPos.x - averageChestPos.x;
    }
    private Vector3 GetAverageChestPos()
    {
        Vector3 average = Vector3.zero, sum = Vector3.zero;
        for (int i = chestPosQueue.Length - 1; i > -1; i--)
            sum += chestPosQueue[i];
        average = new Vector3(sum.x / chestPosQueue.Length, sum.y / chestPosQueue.Length, sum.z / chestPosQueue.Length);
        return average;
    }
}
