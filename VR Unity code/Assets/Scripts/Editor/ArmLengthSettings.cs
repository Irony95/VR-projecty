using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ArmLengthSettings : EditorWindow 
{
    public float leftUpperArmLength = 0;
    public float leftForeArmLength = 0;
    public float leftHandSize = 0;

    public Vector3 leftShoulderLocation; 
    public GameObject shoulder;
    public GameObject leftUpperArm;
    public GameObject elbow;
    public GameObject leftForeArm;
    public GameObject leftHand;

    [MenuItem("Vr Things/ArmLengthSettings")]
    private static void ShowWindow() 
    {
        GetWindow<ArmLengthSettings>("Arm length settings");
    }

    private void OnGUI() 
    {
        leftUpperArmLength = EditorGUILayout.FloatField("left upper arm length", leftUpperArmLength);
        leftForeArmLength = EditorGUILayout.FloatField("left forearm length", leftForeArmLength);
        leftHandSize = EditorGUILayout.FloatField("left hand size", leftHandSize);
        leftShoulderLocation = EditorGUILayout.Vector3Field("shoulder local position", leftShoulderLocation);

        shoulder = (GameObject)EditorGUILayout.ObjectField("left shoulder", shoulder, typeof(GameObject), true);
        leftUpperArm = (GameObject)EditorGUILayout.ObjectField("left upper arm", leftUpperArm, typeof(GameObject), true);
        elbow = (GameObject)EditorGUILayout.ObjectField("left elbow", elbow, typeof(GameObject), true);
        leftForeArm = (GameObject)EditorGUILayout.ObjectField("left forearm", leftForeArm, typeof(GameObject), true);
        leftHand = (GameObject)EditorGUILayout.ObjectField("left hand", leftHand, typeof(GameObject), true);

        if (GUILayout.Button("Update Lengths"))
        {
            if (leftShoulderLocation != null)
            {
                shoulder.transform.localPosition = leftShoulderLocation;                
            }
            if (leftUpperArmLength > 0)
            {
                leftUpperArm.transform.localScale = new Vector3(leftUpperArm.transform.localScale.x, leftUpperArmLength, leftUpperArm.transform.localScale.z);
                leftUpperArm.transform.localPosition = new Vector3(leftUpperArm.transform.localPosition.x, -(leftUpperArmLength/2), leftUpperArm.transform.localPosition.z);
                elbow.transform.localPosition = new Vector3(elbow.transform.localPosition.x, -leftUpperArmLength, elbow.transform.localPosition.z);
            }
            if (leftForeArmLength > 0)
            {
                leftForeArm.transform.localScale = new Vector3(leftForeArm.transform.localScale.x, leftForeArmLength, leftForeArm.transform.localScale.z);
                leftForeArm.transform.localPosition = new Vector3(leftForeArm.transform.localPosition.x, -(leftForeArmLength/2), leftForeArm.transform.localPosition.z);
                leftHand.transform.localPosition = new Vector3(leftHand.transform.localPosition.x , -(leftForeArmLength + leftHandSize/2), leftHand.transform.localPosition.z);
            }
            if (leftHandSize > 0)
            {
                leftHand.transform.localScale = new Vector3(leftHandSize, leftHandSize, leftHandSize);
                leftHand.transform.localPosition = new Vector3(leftHand.transform.localPosition.x , -(leftForeArmLength + leftHandSize/2), leftHand.transform.localPosition.z);
            }
        }
    }
}

