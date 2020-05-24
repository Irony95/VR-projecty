using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperArmRotation : MonoBehaviour
{
    public int amountOfErrorReadings;
    [Space]
    public float accRollAcceptedAngle = 60;
    public float accPitchAcceptedAngle = 60;
    public float compassYawAcceptedAngle = 45;

    public float accRollMaxPercent = 0.95f;
    public float accRollMinPercent = 0.00f;

    public float accPitchMaxPercent = 0.95f;
    public float accPitchMinPercent = 0.00f;

    public float comYawPercent = 1f;

    [Space]
    public float roll;
    public float pitch;
    public float yaw;

    private Vector3 gyroOffset = new Vector3(0f, 0f, 0f);
    private float accRollOffset = 0;
    private float accPitchOffset = 0;
    private float comYawOffset = 0;
    private int amountOfReadingsDone = 0;
    private bool gettingError = false;
    private Transform camera;

    private void Start() 
    {
        camera = Camera.main.transform;    
    }

    //declinationAngle for singapore
    private float declinationAngle = (0.0f + (8.0f / 60.0f)) / (180.0f / Mathf.PI);

    public void UpdateRotation(float gyroX, float gyroY, float gyroZ, float accPitch, float accRoll, float headingX, float headingY, float headingZ)
    {

        if (!gettingError)
        {
            CalculateValues(gyroX - gyroOffset.x, gyroY - gyroOffset.y, gyroZ - gyroOffset.z,
             accPitch - accPitchOffset, accRoll - accRollOffset, headingX, headingY, headingZ);

            gameObject.transform.rotation = Quaternion.Euler(pitch, yaw, roll);
        }
        else
        {
            SetErrorValues(gyroX, gyroY, gyroZ, accPitch, accRoll, headingX, headingY, headingZ);
        }        
    }

    private void CalculateValues(float gyroX, float gyroY, float gyroZ, float accPitch, float accRoll, float headingX, float headingY, float headingZ)
    {
        //calculations of roll and pitch based on gyro values
        float gyroRoll = (roll+gyroY);
        float gyroPitch = (pitch+gyroX);
        float gyroYaw = (yaw+gyroZ);

        //convert values to +-180
        if (Mathf.Abs(gyroRoll) > 180)
        {
            gyroRoll = 180 - Mathf.Abs(gyroRoll);
        }
        if (Mathf.Abs(gyroPitch) > 180)
        {
            gyroPitch = 180 - Mathf.Abs(gyroPitch);
        }
        if (Mathf.Abs(gyroYaw) > 180)
        {
            gyroYaw = 180 - Mathf.Abs(gyroYaw);
        }

        float accRollPercent;
        float accPitchPercent;
        if (Mathf.Abs(accRoll*180/Mathf.PI) >= accRollAcceptedAngle || Mathf.Abs(accPitch*180/Mathf.PI) >= accPitchAcceptedAngle)
        {
            accRollPercent = accRollMinPercent;
            accPitchPercent = accPitchMinPercent;        
        }  
        else 
        {    
            accRollPercent = accRollMaxPercent;
            accPitchPercent = accPitchMaxPercent;
        }        

        //final calculation, taking percentage using from given calculations
        roll = (accRollPercent * (accRoll*180/Mathf.PI)) + ( (1-accRollPercent) * gyroRoll);
        pitch = (accPitchPercent * (accPitch*180/Mathf.PI)) + ( (1-accPitchPercent) * gyroPitch);

        //calculation of the yaw
        if (Mathf.Abs(roll) < compassYawAcceptedAngle && Mathf.Abs(pitch) < compassYawAcceptedAngle)
        {

            float compassHeading = CalculateYawWithCompass(headingX, headingY, headingZ) - comYawOffset;
            if (compassHeading > Mathf.PI)
            {
                compassHeading -= 2* Mathf.PI;
            }
            
            yaw = (comYawPercent * (compassHeading * 180/Mathf.PI)) + (1-comYawPercent) * gyroYaw;
        }
        else
        {
            Debug.Log("all dat gyro bro");
            yaw =  gyroYaw;  
        }
        yaw -= camera.rotation.y;
        Debug.Log(Mathf.Atan2(headingY, headingX)* 180/Mathf.PI);
    }

    private float CalculateYawWithCompass(float headingX, float headingY, float headingZ)
    {
        float cosRoll = Mathf.Cos(-roll * Mathf.PI/180);
        float sinRoll = Mathf.Sin(-roll * Mathf.PI/180);
        float cosPitch = Mathf.Cos(pitch * Mathf.PI/180);
        float sinPitch = Mathf.Sin(pitch * Mathf.PI/180);

        float Xh = headingX * cosPitch + headingZ * sinPitch;
        float Yh = headingX * sinRoll * sinPitch + headingY * cosRoll - headingZ * sinRoll * cosPitch;
        return Mathf.Atan2(Yh, Xh) + declinationAngle;             
    }

    private void SetErrorValues(float gyroX, float gyroY, float gyroZ, float accPitch, float accRoll, float headingX, float headingY, float headingZ)
    {
        if (amountOfReadingsDone == 0)
        {
            Debug.Log("resetting offset");
            gyroOffset = new Vector3(0, 0, 0);
            accRollOffset = 0;
            accPitchOffset = 0;
            comYawOffset = 0;
        
        }

        if (amountOfReadingsDone < amountOfErrorReadings)
        {
            Debug.Log("adding");
            gyroOffset += new Vector3(gyroX, gyroY, gyroZ);
            accRollOffset += accRoll;
            accPitchOffset += accPitch;
            amountOfReadingsDone++;
            Debug.Log(amountOfReadingsDone);
        }
        else 
        {
            //get the average of the errors
            gyroOffset = new Vector3(gyroOffset.x / amountOfErrorReadings, gyroOffset.y / amountOfErrorReadings, gyroOffset.z / amountOfErrorReadings);            
            accPitchOffset = accPitchOffset / amountOfErrorReadings;
            accRollOffset = accRollOffset/ amountOfErrorReadings;

            //In order to get an accurate offset for yaw, we need accurate data for pitch and roll, as it is used in the calculation of yaw.
            //Thereforem we calculate the values to update roll and pitch
            CalculateValues(gyroX - gyroOffset.x, gyroY - gyroOffset.y, gyroZ - gyroOffset.z,
            accPitch - accPitchOffset, accRoll - accRollOffset, headingX, headingY, headingZ);

            comYawOffset = CalculateYawWithCompass(headingX, headingY, headingZ);
            Debug.Log("offset is ");
            Debug.Log(comYawOffset);
            gettingError = false;
            amountOfReadingsDone = 0;
        }
    }

    public void UpdateReadingError()
    {
        gettingError = true;
    }
}
