 using UnityEngine;
  using System.Collections;
  using System.IO.Ports;
  using System.IO;
  using System;


public class SerialGyroTest : MonoBehaviour
{
    
    public Char endingChar = '|';
    public Char splitChar = ',';

    public int amountReadingOfErrors;
    [Space]
    public float accRollAcceptedAngle;

    public float accRollMaxPercent = 0.95f;
    public float accRollMinPercent = 0.00f;


    public float accPitchAcceptedAngle = 60;

    public float accPitchMaxPercent = 0.95f;
    public float accPitchMinPercent = 0.00f;

    [Space]
    public float roll;
    public float pitch;
    public float yaw;

    private SerialPort gyroPort = new SerialPort("COM4", 9600);
    private Vector3 gyroOffset = new Vector3(0f, 0f, 0f);
    private float accRollOffset = 0;
    private float accPitchOffset = 0;

    private string currentRead = "";
    private int amountOfReadingsDone = 0;
    private bool gettingError = false;


    // Start is called before the first frame update
    private void Start()
    {
        gyroPort.Open();
        Debug.Log(gyroPort.IsOpen);
    }

    // Update is called once per frame
    private void Update()
    {
        if (gyroPort.IsOpen)
        {
            try
            {               
                string read = gyroPort.ReadExisting();
                for (int i = 0;i < read.Length;i++)
                {
                    if (read[i] == endingChar)
                    {
                        string[] values = (currentRead.Split(splitChar));
                        if (!gettingError)
                        {
                            CalculateValues(-float.Parse(values[0]) - gyroOffset.x, -float.Parse(values[1]) - gyroOffset.y, -float.Parse(values[2]) - gyroOffset.z,
                             -float.Parse(values[3]) - accPitchOffset, float.Parse(values[4]) - accRollOffset);
                        }
                        else 
                        {
                            SetErrorValues(-float.Parse(values[0]), -float.Parse(values[1]), -float.Parse(values[2]), -float.Parse(values[3]), float.Parse(values[4]));
                        }
                        currentRead = "";
                    }
                    else
                    {                        
                        currentRead += read[i];
                    }
                }
            }

            catch(Exception e)
            {
                Debug.Log(e.Message);
                currentRead = "";
            }

            gameObject.transform.rotation = Quaternion.Euler(pitch, 0f, roll);
        }
        else 
        {
        }
    }

    private void CalculateValues(float gyroX, float gyroY, float gyroZ, float accPitch, float accRoll)
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

        float accRollPercent;
        float accPitchPercent;
        if (Mathf.Abs(accRoll) >= accRollAcceptedAngle || Mathf.Abs(accPitch) >= accPitchAcceptedAngle)
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
        roll = (accRollPercent * accRoll) + ( (1-accRollPercent) * gyroRoll);
        pitch = (accPitchPercent * accPitch) + ( (1-accPitchPercent) * gyroPitch);
        yaw = gyroYaw;  
    }    

    public void SetErrorValues(float gyroX, float gyroY, float gyroZ, float accPitch, float accRoll)
    {
        if (amountOfReadingsDone < amountReadingOfErrors)
        {
            gyroOffset += new Vector3(gyroX, gyroY, gyroZ);
            accRollOffset += accRoll;
            accPitchOffset += accPitch;

            amountOfReadingsDone++;
            Debug.Log(amountOfReadingsDone);
        }
        else 
        {
            //get the average of the errors
            gyroOffset = new Vector3(gyroOffset.x / amountReadingOfErrors, gyroOffset.y / amountReadingOfErrors, gyroOffset.z / amountReadingOfErrors);            
            accPitchOffset = accPitchOffset / amountReadingOfErrors;
            accRollOffset = accRollOffset/ amountReadingOfErrors;
            gettingError = false;
            amountOfReadingsDone = 0;
        }
    }

    public void UpdateReadingError()
    {
        gettingError = true;
    }
}
