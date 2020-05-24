using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraRotation : MonoBehaviour
{
    public Transform cameraToCopy;
    public bool follow = true;

    // Update is called once per frame
    void Update()
    {
        if (follow)
        {
            transform.rotation = Quaternion.AngleAxis(cameraToCopy.rotation.eulerAngles.y, Vector3.up);
        }
    }
}
