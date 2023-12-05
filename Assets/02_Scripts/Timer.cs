using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timeRemaining;    // 남은 시간
    private bool isTimerRunning = false; // 타이머가 실행 중인지를 나타내는 플래그
    public float initialTime = 180; // 초기 시간 (3분)
    private bool isFlashing = false;

    void Start()
    {
        timeRemaining = initialTime; // 초기 시간으로 설정
        DisplayTime(timeRemaining);
    }
    
    public void StartTimer()
    {
        isTimerRunning = true;
        timeRemaining = initialTime;
        isFlashing = false; // 깜빡임 초기화
        timerText.color = Color.white; // 글자색 초기화
    }

    public void ResetTimer()
    {
        isTimerRunning = false; // 타이머를 정지합니다.
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
                Debug.Log("Time's Up!");
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
}
