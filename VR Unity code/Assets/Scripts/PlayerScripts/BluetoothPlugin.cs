using System.Collections;
using System;
using UnityEngine;

public class BluetoothPlugin
{
    private static AndroidJavaClass _pluginClass;
    private static AndroidJavaObject _pluginInstance;    

    public static bool isConnected
    {
        get 
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return PluginInstance.Get<bool>("connected");
            }
            else
            {
                return false;
            }
        }
    }

    public static AndroidJavaClass PluginClass
    {
        get
        {
            if (_pluginClass == null)
            {
                _pluginClass = new AndroidJavaClass("com.IronyCom.UnityBluetooth.UnityBluetooth");
            }
            return _pluginClass;
        }
    }

    public static AndroidJavaObject PluginInstance
    {
        get
        {
            if (_pluginInstance == null)
            {
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("GetInstance");
            }
            return _pluginInstance;
        }
    }



    public static string GetStatus()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return PluginInstance.Call<string>("ErrorFeedback");
        }
        return null;
    }

    public static string[] GetPairedDeviceNames()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return PluginInstance.Call<String[]>("GetPairedDeviceNames");
        }
        return null;
    }

    public static string ConnectToDevice(string name)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string returned = PluginInstance.Call<String>("ConnectToDevice", name);
            return returned;
        }
        return null;
    }

    public static byte[] ReadBytes()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return PluginInstance.Call<byte[]>("ReadBytes");
        }
        return null;
    }

    public static void Close()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            PluginInstance.Call("Close");
        }
    }
}