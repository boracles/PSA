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
        //현재 씬의 이름을 얻음 
        string sceneName = SceneManager.GetActiveScene().name;
        
        //씬 이름을 정수로 변환하여 현재 단계를 설정
        if (int.TryParse(sceneName, out int stageNumber) && stageNumber >= 1 && stageNumber <= 4)
        {
            currentStage = stageNumber;
            Debug.Log($"현재 단계 = {stageNumber}");
        }
        else
        {
            Debug.LogError("씬 이름을 확인해 주세요");
            currentStage = 0; // 유효하지 않은 단계
        }
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
}
