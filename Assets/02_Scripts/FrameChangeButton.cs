using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameChangeButton : MonoBehaviour
{
    public Button frameChangeButton;
    void Start()
    {
        if (frameChangeButton != null)
        {
            frameChangeButton.onClick.AddListener(()=> SoundManager.Instance.PlayVoiceClipAndActivateButton());
        }
    }
}
