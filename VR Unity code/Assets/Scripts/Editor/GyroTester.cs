using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SerialGyroTest))]
public class GyroTester : Editor
{

    public override void OnInspectorGUI()
    {
        SerialGyroTest test = (SerialGyroTest)target;

        base.OnInspectorGUI();
        if(GUILayout.Button("Update Offset Angle"))
        {
            test.UpdateReadingError();
        }
    }
}
