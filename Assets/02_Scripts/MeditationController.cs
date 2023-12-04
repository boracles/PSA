using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeditationController : MonoBehaviour
{
    public GameObject meditationCircle;
    private Material circleMaterial;
    
    public AudioSource meditationVoice;
    public float rotationSpeed = 10.0f;

    private bool isRotating = false;


    private float currentRotation = 0.0f;
    private float targetOpacity = 0.7f;
    private bool increasingOpacity = true;
    private void OnEnable()
    {
        meditationCircle.transform.parent.gameObject.SetActive(false);
    }

    void Start()
    {
        circleMaterial = meditationCircle.GetComponent<MeshRenderer>().material;
    }

    public void PlayMeditationVoice()
    {
        meditationVoice.Play();

        isRotating = true;
    }

    private void Update()
    {
        if (isRotating)
        {
            // 부모 객체의 X축을 중심으로 회전
            float rotationAmount = rotationSpeed * Time.deltaTime;
            meditationCircle.transform.parent.Rotate(Vector3.right, rotationAmount);
            currentRotation += rotationAmount;

            // 한 바퀴 회전할 때마다 Opacity 변경
            if (currentRotation >= 360.0f)
            {
                currentRotation = 0.0f;
                increasingOpacity = !increasingOpacity; // Opacity 변경 방향 전환
            }

            // Material의 Opacity 조절
            float opacityChangeSpeed = 0.6f * Time.deltaTime; // Opacity 변경 속도
            float currentOpacity = circleMaterial.GetFloat("_Alpha");
            if (increasingOpacity)
            {
                currentOpacity = Mathf.Min(currentOpacity + opacityChangeSpeed, targetOpacity);
            }
            else
            {
                currentOpacity = Mathf.Max(currentOpacity - opacityChangeSpeed, 0.1f);
            }

            circleMaterial.SetFloat("_Alpha", currentOpacity);
        }
    }
}
