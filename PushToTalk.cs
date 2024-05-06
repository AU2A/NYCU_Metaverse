using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;

/// <summary>
/// unused
/// </summary>
public class PushToTalk : MonoBehaviour
{
    private Recorder recorder;

    private void Awake()
    {
        if (recorder == null)
        {
            recorder = GetComponent<Recorder>();
            //Debug.Log("get recorder");
        }
    }

    private void Update()
    {
           if (recorder.VoiceDetector.Detected)
           {
               Debug.Log("Voice Detected");
               //PlayerController.instance.is_speaking = true;
           }
           else
           {
               //PlayerController.instance.is_speaking = false;
           }
    }
}
