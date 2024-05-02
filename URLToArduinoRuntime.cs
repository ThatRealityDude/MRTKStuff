using MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.XR;
using Microsoft.MixedReality.OpenXR;
using Unity.VisualScripting;

using UnityEditor;
using TMPro;
using UnityEngine.Experimental.XR.Interaction;
using static UnityEngine.XR.OpenXR.Features.Interactions.PalmPoseInteraction;
using UnityEngine.UI;

public class URLToArduinoRuntime : MonoBehaviour
{
    private string ip;
    private bool running = false;
    [SerializeField] HandManager hm;
    [SerializeField] private TMP_Text ipText;

    private bool stopped = true;

    private bool firstRun = true;

    //private bool validIP = false;

    [SerializeField] private Image stoppedImage;

    [SerializeField] private Image ErrorBool;
    [SerializeField] private TMP_Text IPDebug;
    [SerializeField] private TMP_Text ErrorText;
    private void Awake()
    {
        stopped = true;
        stoppedImage.color = Color.red;
        //StartCoroutine(sendMessage("/thumb=0?index=0?middle=0?ring=0?little=0?"));
    }
    private void FixedUpdate()
    {
        if (!running && !stopped)
        {
            Debug.Log("First");
            StartCoroutine(sendMessage("/thumb=" + hm.handValues[0] + "?index=" + hm.handValues[1] + "?middle=" + hm.handValues[2] + "?ring=" + hm.handValues[3] + "?little=" + hm.handValues[4] + "?"));
        }
    }
    public IEnumerator sendMessage(string message)
    {
        running = true;
        float startTime = Time.deltaTime;
        WWWForm form = new WWWForm();
        string website = "http://" + ip + message + "/";
        IPDebug.text = "WEBSITE: " + website;
        using (UnityWebRequest www = UnityWebRequest.Get(website))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                ErrorBool.color = Color.green;
                ErrorText.text = www.error.ToString();
            }
            else
            {
                Debug.Log("Post Request Complete! \n Time Taken: " + (Time.deltaTime - startTime));
                //running = false;
                if(firstRun) {
                    Debug.Log("Valid IP!");
                }
                ErrorBool.color = Color.red;
            }
        }
        running = false;
    }

    public void addText(string text)
    {
        ip += text;
        ipText.text = ip;
    }

    public void submitText()
    {
        /*
        firstRun = true;
        StartCoroutine(sendMessage("/thumb=0?index=0?middle=0?ring=0?little=0?"));
        if (validIP) {
            ipText.text = "IP WORKED :)";
            stopped = false;
        } else {
            ipText.text = "INVALID IP ENTERED!";
        }
        firstRun = false;
        */
        stopped = false;
        stoppedImage.color = Color.green;
    }

    public void deleteText()
    {
        if(ip.Length < 0) {
            ipText.text = "Enter Arduino IP";
        } else {
            ip = ip.Remove(ip.Length - 1);
            ipText.text = ip;
            Debug.Log(ip);
        }
    }

    public void toggleConnection()
    {
        stopped = !stopped;
        stoppedImage.color = stopped ? Color.red : Color.green;
    }
}
