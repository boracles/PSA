using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MeditationController : MonoBehaviour
{
    public GameObject meditationCircle;
    public AudioSource meditationVoice;
    public MeditationSubtitles subtitlesScript;
    public float rotationSpeed = 10.0f;

    private bool isRotating = false;
    private float currentRotation = 0.0f;
    private Material childMaterial;
    private ParticleSystem particleSystem;
    private ParticleSystem.ShapeModule shapeModule;
    private Material particleMaterial;

    private void OnEnable()
    {
        meditationCircle.transform.parent.gameObject.SetActive(false);
    }

    void Start()
    {
        childMaterial = meditationCircle.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material;
        particleSystem = meditationCircle.transform.GetChild(1).GetComponent<ParticleSystem>();
        shapeModule = particleSystem.shape;
        particleMaterial = meditationCircle.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().material;
        
        // 현재 씬의 이름을 가져옴
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 씬 이름이 조건에 부합하면 PlayMeditationVoice() 실행
        if (currentSceneName == "1_Meditation30s" || currentSceneName == "2_Meditation30s" || currentSceneName == "3_Meditation30s")
        {
            StartCoroutine(PlayMeditationVoiceAfterDelay(3.0f)); // 3초 후에 실행;
        }
    }
    
    IEnumerator PlayMeditationVoiceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        PlayMeditationVoice();
        meditationCircle.transform.parent.gameObject.SetActive(true);
    }
    
    public void PlayMeditationVoice()
    {
        meditationVoice.Play();
        subtitlesScript.StartSubtitles();
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
        
        // 오디오 볼륨에 따라 Material의 _OffsetSize 값 조절
        float offsetSize = MapVolumeToOffsetSize(volumeLevel);
        particleMaterial.SetFloat("_OffsetSize", offsetSize);
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
    
    float MapVolumeToOffsetSize(float volume)
    {
        // 오디오 볼륨을 -0.05에서 0.05 사이의 값으로 변환
        return Mathf.Clamp(volume - 0.08f, -0.05f, 0.05f);
    }
}
