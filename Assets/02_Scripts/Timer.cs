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
    private bool timerExpired = false; // 타이머가 만료되었는지 여부를 나타내는 플래그

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
        timerExpired = false; // 타이머 만료 상태 초기화
    }

    public void ResetTimer()
    {
        isTimerRunning = false;
        timerExpired = false;
        isFlashing = false;
        timerText.enabled = true;
        timerText.color = Color.white;

        float totalElapsedTime = Time.time - startTime;
        int totalMinutes = Mathf.FloorToInt(totalElapsedTime / 60);
        int totalSeconds = Mathf.FloorToInt(totalElapsedTime % 60);

        ptTimeText.text = string.Format("발표 소요시간: {0:00}분 {1:00}초", totalMinutes, totalSeconds);

        timeRemaining = initialTime; // 시간을 초기 값으로 설정합니다.
        DisplayTime(timeRemaining);
    }
    
    void Update()
    {
        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0 && !timerExpired)
            {
                timerExpired = true;
                Debug.Log("시간이 다 됐습니다.");
                PlaySoundSequence();
            }

            if (timeRemaining <= 10.0f && !isFlashing)
            {
                // 10초 남았을 때 또는 타이머가 0에 도달했을 때 깜빡임 시작
                isFlashing = true;
                StartCoroutine(FlashTimerText());
            }

            DisplayTime(timeRemaining);
        }
    }
    IEnumerator FlashTimerText()
    {
        while (isFlashing)
        {
            timerText.enabled = !timerText.enabled; // 텍스트 활성화/비활성화 전환
            yield return new WaitForSeconds(0.5f); // 0.5초마다 전환
        }
    }
    
    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay *= -1; // 음수일 경우 양수로 변환
            timerText.color = Color.red; // 글자색을 빨간색으로 변경
        }
        else if (timeToDisplay <= 10.0f)
        {
            timerText.color = Color.red; // 10초 이하일 때 빨간색으로 변경
        }
        else
        {
            timerText.color = Color.white; // 그 외에는 흰색으로 유지
        }

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
