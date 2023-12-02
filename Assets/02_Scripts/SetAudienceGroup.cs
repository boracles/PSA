using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SetAudienceGroup : MonoBehaviour
{
    public AudienceController[] audienceControllers;
    void Start()
    {
        // 씬에 있는 모든 AudienceController 컴포넌트를 가져옴
        audienceControllers = FindObjectsOfType<AudienceController>();
        
        // StageManager 인스턴스를 찾음
        StageManager stageManager = StageManager.Instance;

        if (stageManager != null)
        {
            // 현재 단계를 가져옴
            int currentStage = stageManager.GetCurrentStage();
            
            // 단계별 인원에 맞게 집중 그룹과 비집중 그룹을 설정
            AssignAudienceGroups(currentStage);
        }
        else
        {
            Debug.LogError("StageManager 인스턴스를 찾을 수 없습니다.");
        }
    }

    private void AssignAudienceGroups(int currentStage)
    {
        List<AudienceController> allAudience = new List<AudienceController>(audienceControllers);
        List<AudienceController> selectedFocusAudience = new List<AudienceController>();
        List<AudienceController> selectedNonFocusAudience = new List<AudienceController>();

        int focusCount, nonFocusCount;
        switch (currentStage)
        {
            case 1:
                focusCount = allAudience.Count;
                nonFocusCount = 0;
                break;
            case 2:
                focusCount = 18;
                nonFocusCount = 7;
                break;
            case 3:
                focusCount = 10;
                nonFocusCount = 15;
                break;
            case 4:
                focusCount = 5;
                nonFocusCount = 20;
                break;
            default:
                Debug.LogError("잘못된 단계입니다.");
                return;
        }

        for (int i = 0; i < focusCount; i++)
        {
            int randomIndex = Random.Range(0, allAudience.Count);
            selectedFocusAudience.Add(allAudience[randomIndex]);
            allAudience.RemoveAt(randomIndex);
        }

        selectedNonFocusAudience.AddRange(allAudience);

        SetAudienceTypeForList(selectedFocusAudience, AudienceType.FOCUS);
        SetAudienceTypeForList(selectedNonFocusAudience, AudienceType.NONFOCUS);

    }
    private void SetRandomAudienceType(List<AudienceController> audienceList, AudienceType type, int count)
    {
        if (audienceList.Count < count)
        {
            Debug.Log($"청중리스트 {type}의넘어오는 수 = {audienceList.Count}, 넘어와야 하는 수 = {count}");
            Debug.LogError("캐릭터 수가 부족합니다.");

            // 필요한 수만큼 캐릭터가 없을 경우, count 값을 적절히 조정합니다.
            count = Mathf.Min(count, audienceList.Count);
        }
        
        // 캐릭터를 중복되지 않게 랜덤으로 선택
        List<AudienceController> shuffledAudience = new List<AudienceController>(audienceList);
        List<AudienceController> selectedAudience = new List<AudienceController>();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, shuffledAudience.Count);
            AudienceController selectedCharacter = shuffledAudience[randomIndex];
            shuffledAudience.RemoveAt(randomIndex); // 이미 선택된 캐릭터를 제거
            selectedAudience.Add(selectedCharacter);
        }

        // 선택된 캐릭터에게 타입 설정
        foreach (var character in selectedAudience)
        {
            character.SetAudience(type);
        }
    }
    private void SetAudienceTypeForList(List<AudienceController> audienceList, AudienceType type)
    {
        foreach (AudienceController audienceController in audienceList)
        {
            audienceController.SetAudience(type);
        }
    }
}
