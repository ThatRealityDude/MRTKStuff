using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.Subsystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomSpeechDetector : MonoBehaviour
{
    [SerializeField]
    private List<PhraseAction> phraseActions;

    private void Start()
    {
        MixedReality.Toolkit.Subsystems.IKeywordRecognitionSubsystem phraseRecognitionSubsystem = XRSubsystemHelpers.KeywordRecognitionSubsystem;
        foreach (var phraseAction in phraseActions)
        {
            if (!string.IsNullOrEmpty(phraseAction.Phrase) &&
              phraseAction.Action.GetPersistentEventCount() > 0)
            {
                phraseRecognitionSubsystem.CreateOrGetEventForKeyword(phraseAction.Phrase).AddListener(() => phraseAction.Action.Invoke());
            }
        }
    }
    private void Update()
    {
        //Debug.Log(XRSubsystemHelpers.KeywordRecognitionSubsystem.running);
    }

    public void debug(string gestureName)
    {
        Debug.Log($"{gestureName} DETECTED");
    }
}


[Serializable]
public struct PhraseAction
{
    [SerializeField]
    private string phrase;

    [SerializeField]
    private UnityEvent action;

    public string Phrase => phrase;

    public UnityEvent Action => action;
}
