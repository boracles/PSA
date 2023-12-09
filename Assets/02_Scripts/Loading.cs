using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke(nameof(OnPreparingPage), 20.0f);
    }

    private void OnPreparingPage()
    {
        GameObject.Find("UI/Canvas/Frame_PreparingReport").SetActive(true);
        gameObject.SetActive(false);
    }
}
