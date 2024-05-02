using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using MixedReality.Toolkit;
using UnityEngine.XR;
using Microsoft.MixedReality.OpenXR;
using UnityEditor;
using UnityEngine.AI;
using System.IO;
using static UnityEngine.XR.OpenXR.Features.Interactions.PalmPoseInteraction;
using UnityEngine.XR.Interaction.Toolkit.AR;
using Unity.VisualScripting;
using MixedReality.Toolkit.Input;



namespace CustomGestureDetector
{
    /// <summary>
    /// Component to allow custom gesture detection
    /// <remarks>
    /// Recommended to be attached to the PalmPrefab
    /// </remarks>
    /// </summary>
    public class GestureDetectorGlobal : MonoBehaviour
    {
        [SerializeField] private float gestureRecogThresh = 0.25f;              // Allowed margin of error of the cumulative distance between a gesture and current pose, value of 0.25 seems quite optimal

        //[SerializeField] private Handedness defaultHandedness = Handedness.Both; // The handedness of the palm joint this component is attached to
        [SerializeField] private XRNode hand = XRNode.RightHand;

        [SerializeField] private bool DEBUGMODE = false;                        // Whether currently in debug mode or not

        private GameObject handPrefab;
        // List of gestures that have been saved and can be recognised later on in script
        [SerializeField] private List<Gesture> gestures = new List<Gesture>();
        private Gesture prevGesture;                                             // the most recent gesture in the previous frame, kept track to trigger Change events



        private void Awake()
        {
            handPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Packages/org.mixedrealitytoolkit.input/Visualizers/RiggedHandVisualizer/openxr_right_hand.prefab", typeof(GameObject));
        }

        private void Update()
        {
            // Checking Key Presses
            if (DEBUGMODE)
                CheckKeyPresses();
        }

        private void FixedUpdate()
        {
            //-------------------------------------------------------------------
            //Handles the Detection of Gestures and their corresponding functions
            //-------------------------------------------------------------------
            Gesture currentDetectedGesture = TryRecogniseGesture();

            bool hasRecognisedGesture = !currentDetectedGesture.Equals(new Gesture());      // Set flag whether gesture has been detected

            if (hasRecognisedGesture && !currentDetectedGesture.Equals(prevGesture))        // if gesture is recognised and is different to before
            {
                if (!prevGesture.Equals(new Gesture()))                                     // if gesture transitioned from another saved gesture, invoke the onderecognised event
                    prevGesture.onDerecognise.Invoke();

                prevGesture = currentDetectedGesture;
                currentDetectedGesture.onRecognise.Invoke();
            }
            else if (!hasRecognisedGesture && !currentDetectedGesture.Equals(prevGesture))  // gesture is not recognised anymore and different to before
            {
                prevGesture.onDerecognise.Invoke();
                prevGesture = currentDetectedGesture;
            }
        }

        /// <summary>
        /// Checks the keypresses in debug mode
        /// </summary>
        void CheckKeyPresses()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SaveGestureDefault();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SaveGestureLeft();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SaveGestureRight();
            }
        }

        [ContextMenu("Save Left Gesture")]
        private void SaveGestureLeft()
        {
            SaveGesture(XRNode.LeftHand);
        }

        [ContextMenu("Save Right Gesture")]
        private void SaveGestureRight()
        {
            SaveGesture(XRNode.RightHand);
        }

        [ContextMenu("Save Default Gesture")]
        private void SaveGestureDefault()
        {
            SaveGesture(hand);
        }

        /// <summary>
        /// Saves the current gesture to a new Gesture Struct into the list of gestures
        /// </summary>
        /// <param name="handedness">Which hand to save gesture from</param>
        /// <remarks>
        /// We use save both lists and Dictionaries because only Lists can be serializable in the Unity Editor, while dictionaries will allow for more efficient lookups.
        /// However, Dictionaries are not serialisable by Unity
        /// </remarks>
        private void SaveGesture(UnityEngine.XR.XRNode handNode)
        {
            Debug.Log("Saving Gesture...");
            // Save joint data for this gesture to a newGesture Gesture struct
            Gesture newGesture = InitialiseNewGesture();

            Vector3 palmPos = Vector3.zero;
            Quaternion palmRot = Quaternion.identity;
            if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, handNode, out HandJointPose palmPose))
            {
                //Debug.Log("Got Palm Information");
                palmPos = palmPose.Position;
                palmRot = palmPose.Rotation;
            }


            // loop through all joints and save the data to newGesture
            foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
            {
                if (joint == TrackedHandJoint.TotalJoints)
                {
                    Debug.Log("TotalJoints Skipped");
                }
                else if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(joint, handNode, out HandJointPose pose))
                {
                    newGesture.handNode = handNode;
                    newGesture.joints.Add(joint);
                    newGesture.jointPoses.Add(pose);
                    newGesture.gestureJointData.Add(joint, pose);

                    Vector3 jointPosFromPalm = InverseTransformPoint(palmPos, palmRot, Vector3.one, pose.Position);

                    newGesture.positionsFromPalm.Add(jointPosFromPalm);  // Add to positions list the current joint's position relative to this gameObject (palm joint prefab)
                    newGesture.gestureJointPositionData.Add(joint, jointPosFromPalm);  // Add to positions list the current joint's position relative to this gameObject (palm joint prefab)
                }
            }
            gestures.Add(newGesture);
        }

        /// <summary>
        /// Converts a position in world space into local space relative to reference. InverseTransformPoint() function without having to include the transform. 
        /// </summary>
        /// <remarks>
        /// Obtained from PraetorBlue https://forum.unity.com/threads/transform-inversetransformpoint-without-transform.954939/
        /// </remarks>
        /// <param name="refPos">Position of the reference</param>
        /// <param name="refRot">Rotation of the reference</param>
        /// <param name="refScale">Scale of the reference</param>
        /// <param name="pos">Position of object to be converted to local space</param>
        /// <returns> Position of given object within local space </returns>
        Vector3 InverseTransformPoint(Vector3 refPos, Quaternion refRot, Vector3 refScale, Vector3 pos)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(refPos, refRot, refScale);
            Matrix4x4 inverse = matrix.inverse;
            return inverse.MultiplyPoint3x4(pos);
        }

        /// <summary>
        /// Function to initialise all the lists and dictionaries within our Gestures struct
        /// </summary>
        /// <returns> Gesture object with initialised attributes </returns>
        private Gesture InitialiseNewGesture()
        {
            Gesture newGesture = new Gesture
            {
                name = "New Gesture",
                gestureJointData = new Dictionary<TrackedHandJoint, HandJointPose>(),
                gestureJointPositionData = new Dictionary<TrackedHandJoint, Vector3>(),

                joints = new List<TrackedHandJoint>(),
                jointPoses = new List<HandJointPose>(),
                positionsFromPalm = new List<Vector3>(),
            };

            return newGesture;
        }


        /// <summary>
        /// Debug Function to print palm gestures
        /// </summary>
        [ContextMenu("Print Palm Gesture")]
        private void PrintPalmGestureData()
        {
            string toPrint = "";
            if (XRSubsystemHelpers.HandsAggregator.Equals(null))
                Debug.LogError("NO HANDS DETECTED WHILE ATTEMPTING TO PRINT PALM GESTURE");

            foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
            {
                if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(joint, XRNode.RightHand, out HandJointPose rightPose))
                    toPrint += (joint + " " + rightPose.Position + "\n");

                if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(joint, XRNode.LeftHand, out HandJointPose leftPose))
                    toPrint += (joint + " " + leftPose.Position + "\n");
            }
            Debug.Log(toPrint);
        }



        /// <summary>
        /// Polling function to find the gesture that matches current pose
        /// </summary>
        /// <returns> Returns the saved gesture that is most likely, or an empty Gesture object if there is none </returns>
        Gesture TryRecogniseGesture()
        {
            Gesture bestGesture = new Gesture();

            float bestDistance = Mathf.Infinity;
            foreach (Gesture gesture in gestures)
            {
                float thisDistance = CompareGestureDistancesToCurrent(gesture);

                if (thisDistance > gestureRecogThresh)
                    continue;

                if (thisDistance < bestDistance)
                {
                    bestDistance = thisDistance;
                    bestGesture = gesture;
                }
            }
            return bestGesture;
        }

        /// <summary>
        /// Helper function to compare the current palm pose and a given gesture pose
        /// </summary>
        /// <param name="gesture"> Gesture struct to match the current pose with </param>
        /// <returns> Cumulative distance of the current joints to the joints within the saved gesture </returns>
        float CompareGestureDistancesToCurrent(Gesture gesture)
        {

            XRNode thisHandNode = gesture.handNode;
            float distanceSum = 0;

            Vector3 palmPos = Vector3.zero;
            Quaternion palmRot = Quaternion.identity;

            if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, thisHandNode, out HandJointPose palmPose))
            {
                palmPos = palmPose.Position;
                palmRot = palmPose.Rotation;
            }
            else
            {
                return Mathf.Infinity;
            }

            foreach (TrackedHandJoint joint in Enum.GetValues(typeof(TrackedHandJoint)))
            {
                if (joint == TrackedHandJoint.TotalJoints)
                {
                    //Debug.Log("TotalJoints Skipped");
                }
                else if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(joint, thisHandNode, out HandJointPose currentPose))
                {
                    int thisGesturePoseIndex = gesture.joints.IndexOf(joint);
                    Vector3 thisSavedPosition = Vector3.zero;
                    try
                    {
                        thisSavedPosition = gesture.positionsFromPalm[thisGesturePoseIndex];
                    }
                    catch (Exception _)
                    {
                        continue;
                    }
                    //MixedRealityPose thisGesturePose = gesture.jointPoses[thisGesturePoseIndex];

                    Vector3 thisCurrentPosition = InverseTransformPoint(palmPos, palmRot, Vector3.one, currentPose.Position);
                    distanceSum += Vector3.Distance(thisSavedPosition, thisCurrentPosition);

                    if (distanceSum > gestureRecogThresh)
                        return Mathf.Infinity;
                }
            }

            if (distanceSum == 0)
                return Mathf.Infinity;
            return distanceSum;
        }

        [System.Serializable]
        public struct Gesture
        {
            public string name;

            // Gesture Data stored
            public Dictionary<TrackedHandJoint, HandJointPose> gestureJointData;
            public Dictionary<TrackedHandJoint, Vector3> gestureJointPositionData;

            public List<TrackedHandJoint> joints;
            public List<HandJointPose> jointPoses;
            public List<Vector3> positionsFromPalm;      // List of joint's positions from the palm

            public XRNode handNode;

            // custom events to trigger when recognising / losing gesture
            public UnityEvent onRecognise;
            public UnityEvent onDerecognise;
        }


    }
}
