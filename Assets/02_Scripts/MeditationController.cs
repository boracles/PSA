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
    
    private float[] spectrum = new float[256];
    
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
        
        StartCoroutine(ThrottledRoutine());
    }
    
    IEnumerator PlayMeditationVoiceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        PlayMeditationVoice();
        meditationCircle.transform.parent.gameObject.SetActive(true);
    }
    
    public void PlayMeditationVoice()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        // 3분 명상 씬인 경우 Text_MinutesInfo를 먼저 비활성화
        if (currentSceneName == "Meditation")
        {
            Invoke(nameof(PlayMeditationVoiceinMeditation), 3.0f);
        }
        else
        {
            meditationVoice.Play();
            subtitlesScript.StartSubtitles();
            isRotating = true;
            StartCoroutine(UpdateFresnelPowerCoroutine(meditationVoice.clip.length)); // 코루틴 시작
        }
    }

    IEnumerator UpdateFresnelPowerCoroutine(float clipLength)
    {
        if (meditationVoice == null || meditationVoice.clip == null)
        {
            yield break; // 오디오 소스나 클립이 없으면 코루틴을 종료합니다.
        }
        
        float startTime = Time.time;
        while (Time.time - startTime <= clipLength)
        {
            float playbackProgress = (Time.time - startTime) / clipLength;
            float fresnelPower = 8 * (1 - playbackProgress);

            if (childMaterial != null)
            {
                childMaterial.SetFloat("_FresnelPower", fresnelPower);
            }

            yield return null; // 다음 프레임까지 대기
        }

        if (childMaterial != null)
        {
            childMaterial.SetFloat("_FresnelPower", 0); // 오디오 클립이 끝나면 fresnelPower를 0으로 설정
        }
    }
    
    void PlayMeditationVoiceinMeditation()
    {
        GameObject.Find("UI/Canvas/Text_MinutesInfo").SetActive(false);
        meditationVoice.Play();
        subtitlesScript.StartSubtitles();
        isRotating = true;
    }

    private void Update()
    {
        if (isRotating)
        {
            RotateMeditationCircle();
            ThrottledUpdate();
        }
    }
    IEnumerator ThrottledRoutine()
    {
        while (true)
        {
            UpdateParticleSystemRadius();
            yield return new WaitForSeconds(0.1f); // Update every 0.1 seconds instead of every frame
        }
    }
    
    void ThrottledUpdate()
    {
        meditationVoice.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
    }

    private void RotateMeditationCircle()
    {
        // 부모 객체의 X축을 중심으로 회전
        float rotationAmount = rotationSpeed * Time.deltaTime;
        meditationCircle.transform.parent.Rotate(Vector3.right, rotationAmount);
        currentRotation = (currentRotation + rotationAmount) % 360.0f;
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
        return Mathf.Clamp(volume * 50, 0, 6); // 오디오 음량에 따른 radius 값의 범위 조절
    }
    
    float MapVolumeToOffsetSize(float volume)
    {
        // 오디오 볼륨을 -0.05에서 0.05 사이의 값으로 변환
        return Mathf.Clamp(volume - 0.06f, -0.06f, 0.06f);
    }
}
