using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeditationController : MonoBehaviour
{
    public GameObject meditationCircle;
    public AudioSource meditationVoice;
    public float rotationSpeed = 10.0f;

    private bool isRotating = false;
    private float currentRotation = 0.0f;
    private Material childMaterial;
    private ParticleSystem particleSystem;
    private ParticleSystem.ShapeModule shapeModule;

    private void OnEnable()
    {
        meditationCircle.transform.parent.gameObject.SetActive(false);
    }

    void Start()
    {
        childMaterial = meditationCircle.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material;
        particleSystem = meditationCircle.transform.GetChild(1).GetComponent<ParticleSystem>();
        shapeModule = particleSystem.shape;
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
            RotateMeditationCircle();
            UpdateFresnelPower();
            UpdateParticleSystemRadius();
        }
    }

    private void RotateMeditationCircle()
    {
        // 부모 객체의 X축을 중심으로 회전
        float rotationAmount = rotationSpeed * Time.deltaTime;
        meditationCircle.transform.parent.Rotate(Vector3.right, rotationAmount);
        currentRotation = (currentRotation + rotationAmount) % 360.0f;
    }

    private void UpdateFresnelPower()
    {
        float fresnelPower;
        if (currentRotation <= 180)
        {
            fresnelPower = 8 * (1 - currentRotation / 180.0f);
        }
        else
        {
            fresnelPower = 8 * ((currentRotation - 180) / 180.0f);
        }

        if (childMaterial != null)
        {
            childMaterial.SetFloat("_FresnelPower", fresnelPower);
        }
    }

    private void UpdateParticleSystemRadius()
    {
        // 오디오 볼륨 레벨 계산
        float[] spectrum = new float[256];
        meditationVoice.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        float volumeLevel = GetRMS(spectrum);

        // 오디오 볼륨에 따라 ParticleSystem의 Radius 값 조절
        float radius = MapVolumeToRadius(volumeLevel);
        shapeModule.radius = radius;
    }

    float GetRMS(float[] spectrum)
    {
        float rms = 0f;
        for (int i = 0; i < spectrum.Length; i++)
        {
            rms += spectrum[i] * spectrum[i];
        }
        rms = Mathf.Sqrt(rms / spectrum.Length);
        return rms;
    }

    float MapVolumeToRadius(float volume)
    {
        return Mathf.Clamp(volume * 50, 0, 4); // 오디오 음량에 따른 radius 값의 범위 조절
    }
}
