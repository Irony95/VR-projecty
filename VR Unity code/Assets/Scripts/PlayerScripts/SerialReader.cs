using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO.Ports;
using System.IO;
using System;
using System.Text;
using System.Threading;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class SerialReader : MonoBehaviour
{
    public Text onScreenText;
    public Button onScreenConnectButton;
    [Space]

    public PlayerController controller;
    public UpperArmRotation armRotate;
    public ForeArmRotation elbowRotate;



    [Space]
    public Char endingChar = '|';
    public Char splitChar = ',';

    [Space]
    public int serialBaudRate = 9600;

    private string[] availablePorts;
    private int selectedPort = 0;

    private volatile SerialPort serialPort;
    private string currentRead = "";


    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            availablePorts = BluetoothPlugin.GetPairedDeviceNames();
        }
        else
        {
            availablePorts = SerialPort.GetPortNames();
        }  
        onScreenConnectButton.GetComponentInChildren<Text>().text = availablePorts[selectedPort];
    }

    // Update is called once per frame
    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android && BluetoothPlugin.isConnected)
        {   
            UpdateData();         
        }
        else if (Application.platform == RuntimePlatform.Android && !BluetoothPlugin.isConnected)   
        {
            String[] btDevices = BluetoothPlugin.GetPairedDeviceNames();
            availablePorts = btDevices;

            String output = "";
            output += BluetoothPlugin.GetStatus() + "\n\n";
            if (btDevices != null)
            {
                foreach (String device in btDevices)
                {
                    output += device + "\n";
                }
                onScreenText.text = output;
            }
        }

        #if UNITY_EDITOR
        if (serialPort != null && serialPort.IsOpen)
        {
            UpdateData();
        }
        else
        {
            if(!onScreenText.enabled)
            {
                onScreenText.enabled = true;
            }
            string shownText = "";
            if (availablePorts != null)
            {
                for (int i = 0;i < availablePorts.Length;i++)
                {
                    shownText += availablePorts[i] + "\n";
                }
            }

            onScreenText.text = shownText;
        }
        #endif
    }
    public void UpdateData()
    {
        //wait for the connection to be established        
        if (Application.platform == RuntimePlatform.Android && BluetoothPlugin.isConnected)
        {
            try
            {            
                byte[] array = BluetoothPlugin.ReadBytes();
                string read = Encoding.ASCII.GetString(array, 0, array.Length);                
                for (int i = 0;i < read.Length;i++)
                {
                    if (read[i] == endingChar)
                    {                                
                        ParseData(currentRead);
                        onScreenText.text = currentRead;
                        currentRead = "";
                    }
                    else
                    {                        
                        currentRead += read[i];
                    }
                }
            }
            catch (Exception e)
            {
                currentRead = "";                
                return;
            }
        }        
        
        #if UNITY_EDITOR
        //wait for the connection to be established        
        if (serialPort != null && serialPort.IsOpen)
        {
           try
           {               
               string read = serialPort.ReadExisting();
               for (int i = 0;i < read.Length;i++)
                {
                    if (read[i] == endingChar)
                    {                                
                        ParseData(currentRead);
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
               Debug.Log("Opened port, but with error: " + e.Message);
               currentRead = "";               
               return;
           }
        }
        #endif
        
    }

    public void ParseData(String data)
    {
        //loop through the bytes stored, to look for the ending char and send the complete data once found                                 
        try
        {
            //get the different values and call functions of parts of the arm
            string[] values = (data.Split(splitChar));            
            armRotate.UpdateRotation(-float.Parse(values[0]), -float.Parse(values[1]), -float.Parse(values[2]), -float.Parse(values[3]),
             float.Parse(values[4]), float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]));
             
            elbowRotate.UpdateRotation(float.Parse(values[8]));
            if (int.Parse(values[9]) == 1)
            {
                controller.ButtonAPressed();
            }
            else if (int.Parse(values[9]) == 0)
            {
                controller.ButtonAReleased();
            }
            if (int.Parse(values[10]) == 1)
            {
                controller.ButtonBPressed();
            }
            else if (int.Parse(values[10]) == 0)
            {
                controller.ButtonBReleased();
            }
        }
        catch (Exception e)
        {
        }            
    }

    public void NextAvailablePort()
    {
        if (serialPort == null && !BluetoothPlugin.isConnected)
        {
            selectedPort++;
            if (selectedPort > availablePorts.Length - 1)
            {
                selectedPort = 0;
            }
            onScreenConnectButton.GetComponentInChildren<Text>().text = availablePorts[selectedPort];
        }
    }

    public void ConnectToPort()
    {
        #if UNITY_ANDROID
        if (!BluetoothPlugin.isConnected)
        {                                   
            onScreenConnectButton.GetComponentInChildren<Text>().text = BluetoothPlugin.ConnectToDevice(availablePorts[selectedPort]);                  
        }
        else 
        {
            BluetoothPlugin.Close();
            onScreenConnectButton.GetComponentInChildren<Text>().text = availablePorts[selectedPort];
        }
        #endif


        #if UNITY_EDITOR
        //If we are not connected
        if (serialPort == null || !serialPort.IsOpen)
        {
            try
            {
                serialPort = new SerialPort(availablePorts[selectedPort], serialBaudRate);
                serialPort.Open();
                onScreenConnectButton.GetComponentInChildren<Text>().text = "Connected/ trying to connect";
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        //If we are connected
        else
        {
            serialPort.Close();
            serialPort = null;
            onScreenConnectButton.GetComponentInChildren<Text>().text = availablePorts[selectedPort];
        }
        #endif
    }
}
