using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timeRemaining;    // 남은 시간
    private bool isTimerRunning = false; // 타이머가 실행 중인지를 나타내는 플래그
    public float initialTime = 180.0f; // 초기 시간 (3분)
    private bool isFlashing = false;
    private float startTime; // 타이머 시작 시간
    private TextMeshProUGUI ptTimeText;
    
    void Start()
    {
        timeRemaining = initialTime; // 초기 시간으로 설정
        DisplayTime(timeRemaining);

        ptTimeText = GameObject.Find("UI/Canvas/Frame_Report/Text_PTTime").GetComponent<TextMeshProUGUI>();
    }
    
    public void StartTimer()
    {
        isTimerRunning = true;
        timeRemaining = initialTime;
        startTime = Time.time; // 타이머 시작 시간 기록
        isFlashing = false; // 깜빡임 초기화
        timerText.color = Color.white; // 글자색 초기화
    }

    public void ResetTimer()
    {
        isTimerRunning = false; // 타이머를 정지합니다.
        float elapsedTime = Time.time - startTime; // 총 소요시간 계산
        
        // 분과 초로 변환
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        ptTimeText.text = string.Format("발표 소요시간: {0:00}분 {1:00}초", minutes, seconds);
        
        timeRemaining = initialTime; // 시간을 초기 값으로 설정합니다.
        timerText.color = Color.white; // 글자색 초기화
        DisplayTime(timeRemaining); // 타이머 표시를 업데이트합니다.
    }
    
    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
                if (timeRemaining <= 10.0f && !isFlashing)
                {
                    // 10초 남았을 때 깜빡임 시작
                    isFlashing = true;
                    StartCoroutine(FlashTimerText());
                }
            }
            else
            {
                Debug.Log("시간이 다 됐습니다.");
                PlaySoundSequence();
                ResetTimer();
            }
        }
    }
    IEnumerator FlashTimerText()
    {
        timerText.color = Color.red; // 글자색을 빨간색으로 변경

        while (isFlashing)
        {
            timerText.enabled = !timerText.enabled; // 텍스트 활성화/비활성화 전환
            yield return new WaitForSeconds(0.5f); // 0.5초마다 전환
        }
    }
    
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    private void PlaySoundSequence()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.sfxClips.Length > 1)
        {
            SoundManager.Instance.sfxSource = gameObject.GetComponent<AudioSource>();
            SoundManager.Instance.sfxSource.clip = SoundManager.Instance.sfxClips[0];
            SoundManager.Instance.sfxSource.Play();

            // 임시 지연 시간. 실제 클립 길이에 따라 조정 필요
            float delayForSecondClip = 2.0f;
            Invoke("PlaySecondClip", delayForSecondClip);
        }
    }
    
    private void PlaySecondClip()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.sfxSource != null && SoundManager.Instance.sfxClips.Length > 1)
        {
            SoundManager.Instance.sfxSource.clip = SoundManager.Instance.sfxClips[1];
            SoundManager.Instance.sfxSource.Play();
        }
    }
}
