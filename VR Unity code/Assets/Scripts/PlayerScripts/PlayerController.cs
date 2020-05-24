using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    SerialReader connectionInput;

    public UpperArmRotation armRotate;
    public ForeArmRotation elbowRotate;
    public Transform hand;

    public GvrReticlePointer pointer;
    public Slider FOVslider;
    public GameObject drawnBoxPrefab;

    [Space]
    public int FOVupperLimit = 150;
    public int FOVlowerLimit = 30;
    [Space]
    public bool ifButtonADown = false;
    public bool ifButtonBDown = false;

    public bool isHandInBox = false;
    public bool isGrabbing = false;   
    public bool isDrawing = false; 

    private Camera playerCamera;
    private GameObject boxDrawn;
    private void Start() 
    {
        playerCamera = Camera.main;
        FOVslider.value = (playerCamera.fieldOfView - FOVlowerLimit)/ (FOVupperLimit - FOVlowerLimit);
    }

    private void Update() 
    {
        if (ifButtonADown && isDrawing && boxDrawn != null)
        {
            boxDrawn.transform.localScale = boxDrawn.transform.position - hand.position;
        }
    }

    public void CalculateErrors()
    {
        armRotate.UpdateReadingError();
        elbowRotate.UpdateReadingError();
    }

    public void AdjustFOV()
    {
        playerCamera.fieldOfView = (FOVupperLimit-FOVlowerLimit)*FOVslider.value + FOVlowerLimit;
    }

    //Button A is the button on the top
    public void ButtonAPressed()
    {
        ifButtonADown = true;

        if (!isGrabbing && !isDrawing && !isHandInBox)
        {
            Vector3 pos = hand.position;
            boxDrawn = Instantiate(drawnBoxPrefab, pos, Quaternion.identity); 
            isDrawing = true;           
        }        
    }
    //Button B is the button on the bottom
    public void ButtonBPressed()
    {
        ifButtonBDown = true;
    }

    public void ButtonAReleased()
    {
        ifButtonADown = false;   

        if (isDrawing && boxDrawn != null)
        {
            boxDrawn = null;
            isDrawing = false;
        }    
    }

    public void ButtonBReleased()
    {
        ifButtonBDown = false;
    }
}
