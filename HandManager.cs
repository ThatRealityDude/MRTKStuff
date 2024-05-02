using MixedReality.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class HandManager : MonoBehaviour
{
    private TrackedHandJoint[] fingers = { TrackedHandJoint.ThumbTip, TrackedHandJoint.IndexTip, TrackedHandJoint.MiddleTip, TrackedHandJoint.RingTip, TrackedHandJoint.LittleTip };
    private float[,] handMapping = { { 0.06f, 0.102f }, { 0.08f, 0.12f }, { 0.111f, 0.133f }, { 0.102f, 0.12f }, { 0.058f, 0.10f } };
    private float[] maxValues = { 100, 100, 130, 90, 70 }; //Values relating to the max motor value for each motor on the hand
    public int[] handValues = new int[5];

    //private float testMap = 80;
    private void Update()
    {
        int count = 0;
        XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, XRNode.RightHand, out HandJointPose palmPose);
        for (int i = 0; i < fingers.Length; i++)
        {
            XRSubsystemHelpers.HandsAggregator.TryGetJoint(fingers[i], XRNode.RightHand, out HandJointPose tipPose);
            float distance = Vector3.Distance(tipPose.Position, palmPose.Position);
            handValues[count] = (int)Extentions.Remap(distance, handMapping[count, 0], maxValues[i], handMapping[count, 1], 0);
            //handValues[count] = Vector3.Distance(tipPose.Position, palmPose.Position);
            //Debug.Log(count + "|" + fingers[i].ToString() + "| Value: " + Vector3.Distance(tipPose.Position, palmPose.Position) + " | Remapped Value: " + handValues[count]);
            count++;

        }
    }
}
