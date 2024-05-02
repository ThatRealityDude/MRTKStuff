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

public class URLToArduino : MonoBehaviour
{
    [SerializeField] private string ip;
    private bool running = false;
    [SerializeField] HandManager hm;

    private void Awake()
    {
        StartCoroutine(sendMessage("/thumb=0?index=0?middle=0?ring=0?little=0?"));
    }
    private void FixedUpdate()
    {
        if (!running)
        {
            StartCoroutine(sendMessage("/thumb=" + hm.handValues[0] + "?index=" + hm.handValues[1] + "?middle=" + hm.handValues[2] + "?ring=" + hm.handValues[3] + "?little=" + hm.handValues[4] + "?"));
        }
    }
    public IEnumerator sendMessage(string message)
    {
        running = true;
        float startTime = Time.deltaTime;
        WWWForm form = new WWWForm();
        string website = "http://" + ip + message;
        using (UnityWebRequest www = UnityWebRequest.Get(website))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);

            }
            else
            {
                Debug.Log("Post Request Complete! \n Time Taken: " + (Time.deltaTime - startTime));
                running = false;
            }
        }
    }
}
