using MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatefulInteractable))]
public class GazeExample : MonoBehaviour //This is placed on the object you want to detect
{
    // Update is called once per frame
    void Update()
    {
        if(GetComponent<StatefulInteractable>().IsGazeHovered)
        {
            GetComponent<Renderer>().material.color = Color.green;

            //Can add some code that changes a static or referenced variable in another script to the object that is detected i
        } else {
            GetComponent<Renderer>().material.color = Color.red;
        }
        //GetComponent<Renderer>().material.color = GetComponent<StatefulInteractable>().IsGazeHovered ? Color.green : Color.red;
    }
}

