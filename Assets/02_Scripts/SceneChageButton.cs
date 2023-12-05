using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class SceneChageButton : MonoBehaviour
{
    public Button sceneChangeButton;

    void Start()
    {
        if (sceneChangeButton != null)
        {
            sceneChangeButton.onClick.AddListener(()=> StageManager.Instance.SceneChange());
        }
    }
}
