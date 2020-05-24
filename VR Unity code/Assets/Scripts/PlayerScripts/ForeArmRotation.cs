using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeArmRotation : MonoBehaviour
{
    public int amountOfErrorReadings;
    private int amountOfReadingsDone = 0;
    public bool gettingError = false;
    [Space]
    public float angleOffset = 0f;

    public void UpdateRotation(float percentage)
    {
        float angle = percentage *360;
        if (!gettingError)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, angle - angleOffset);
        }
        else
        {
            setErrorValues(angle);
        }
    }

    private void setErrorValues(float angle)
    {
        if (amountOfReadingsDone < amountOfErrorReadings)
        {
            angleOffset += angle;
            amountOfReadingsDone++;
        }
        else
        {
            gettingError = false;
            angleOffset = angleOffset/amountOfErrorReadings;
            amountOfReadingsDone = 0;
        }
    }

    public void UpdateReadingError()
    {
        gettingError = true;
    }
}
