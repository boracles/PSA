using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    // 현재 단계를 저장하는 변수 
    private int currentStage;

    public static StageManager Instance { get; private set; }

    void Awake()
    {
        // 싱글턴 패턴 적용
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 씬이 로드될 때 호출될 함수를 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    // 씬 로드 시 호출될 콜백 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetCurrentStage();
    }

    void SetCurrentStage()
    {
        string sceneName = SceneManager.GetActiveScene().name;
    
        if (int.TryParse(sceneName, out int stageNumber))
        {
            if (stageNumber >= 0 && stageNumber <= 3)
            {
                currentStage = stageNumber;
                Debug.Log($"현재 단계 = {stageNumber}");

                // 0번 씬(명상 단계)에 대한 특별한 처리
                if (stageNumber == 0)
                {
                    HandleMeditationStage();
                }
            }
            else
            {
                // 이 경우에는 씬 번호가 1, 2, 3이 아니므로 명상 상태로 처리
                Debug.Log("현재 씬은 명상 단계입니다");
                HandleMeditationStage();
            }
        }
        else
        {
            // 씬 이름이 숫자로 파싱되지 않는 경우에도 명상 상태로 처리
            Debug.Log("현재 씬은 명상 단계입니다");
            HandleMeditationStage();
        }
    }

    // 명상 단계에 대한 처리를 하는 함수
    void HandleMeditationStage()
    {
        // 명상 단계에 필요한 설정이나 동작을 여기에 구현
        Debug.Log("명상 단계 시작");
    }

    // 현재 단계를 반환하는 함수 
    public int GetCurrentStage()
    {
        return currentStage;
    }

    // 게임 오브젝트가 파괴될 때 이벤트 구독 해제
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    public void OnPresentationComplete()
    {
        AudienceGazeController[] audienceMembers = FindObjectsOfType<AudienceGazeController>();
        foreach (var member in audienceMembers)
        {
            member.StartClapping();
        }
    }
    
    public void SceneChange()
    {
        // 현재 씬 이름 확인
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 현재 씬이 "OnBoarding"이면 "Meditation" 씬으로 이동
        if (currentSceneName == "OnBoarding")
        {
            SceneManager.LoadScene("Meditation");
        }
        // 다른 씬 이름에 대한 처리도 여기에 추가 가능
    }
   
}
