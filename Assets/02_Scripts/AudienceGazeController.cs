using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudienceGazeController : MonoBehaviour
{
    private Transform player;
    private Transform pptScreen;
    private Animator animator;

    public RuntimeAnimatorController sittingLCrossedController;
    public RuntimeAnimatorController sittingRCrossedController;
    
    public StageManager stageManager;

    public float minLookTime = 2.0f;
    public float maxLookTime = 5.0f;
    public float transitionSpeed = 1.0f;    // 시선 이동 속도
    private float lookTimer;
    public Transform currentTarget;
    private Vector3 lookAtPosition;
    private bool isPlayingAnimation = false;    // 애니메이션이 재생 중인지 나타내는 플래그
    public int randomChoice = 1;

    public enum AudienceType {FOCUS, NONFOCUS}
    public AudienceType audienceType;
    private static readonly int TiredStayAwake = Animator.StringToHash("TiredStayAwake");

    private Dictionary<AudienceController, float> audienceChangeTimers = new Dictionary<AudienceController, float>();

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        pptScreen = GameObject.Find("Level").transform.Find("PPTScreen");

        stageManager = StageManager.Instance;
        
        if (transform.Find("Laptop"))
        {
            gameObject.tag = "LaptopOwner";
        }

        audienceType = (AudienceType) GetComponent<AudienceController>().audience.audienceType;
        
        // 초기 시선 대상을 플레이어로 설정하고 타이머 재설정
        currentTarget = player;
        lookTimer = Random.Range(minLookTime, maxLookTime);
        
        InitializeAudienceChangeTimers();
    }

    void Update()
    {
        if (!isPlayingAnimation)
        {
            // 애니메이션이 재생되지 않는 동안에만 LookTimer 업데이트
            lookTimer -= Time.deltaTime;
            if (lookTimer <= 0)
            {
                SetRandomTarget();
            }
        }
        
        if (currentTarget != null)
        {
            lookAtPosition = Vector3.Lerp(lookAtPosition, currentTarget.position, Time.deltaTime * transitionSpeed);
        }
        
        // StageManager의 currentStage가 2단계 또는 3단계일 경우 AnimationController 교체
        if (stageManager.GetCurrentStage() == 2 || stageManager.GetCurrentStage() == 3)
        {
            ChangeAnimationControllerForNonFocusAudience();
            foreach (var audiencePair in audienceChangeTimers.ToList())
            {
                var audience = audiencePair.Key;
                var timer = audiencePair.Value;

                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    ChangeAudienceAnimationController(audience);
                    audienceChangeTimers[audience] = Random.Range(10f, 20f); // 다음 변경을 위한 새로운 랜덤 타이머 설정
                }
                else
                {
                    audienceChangeTimers[audience] = timer;
                }
            }
        }
    }
    private void InitializeAudienceChangeTimers()
    {
        var audiences = FindObjectsOfType<AudienceController>();

        foreach (var audience in audiences)
        {
            if (audienceType == AudienceType.NONFOCUS)
            {
                audienceChangeTimers[audience] = Random.Range(10f, 20f); // 각 청중에 대한 초기 랜덤 타이머 설정
            }
        }
    }
    private void ChangeAudienceAnimationController(AudienceController audience)
    {
        if (!audience.CompareTag("LaptopOwner"))
        {
            // 이름이 "L"이면 SittingLCrossed, "R"이면 SittingRCrossed로 교체
            if (audience.name == "L")
            {
                audience.GetComponent<Animator>().runtimeAnimatorController = sittingLCrossedController;
            }
            else if (audience.name == "R")
            {
                audience.GetComponent<Animator>().runtimeAnimatorController = sittingRCrossedController;
            }
        }
    }
    private void ChangeAnimationControllerForNonFocusAudience()
    {
        AudienceController[] nonFocusAudiences = FindObjectsOfType<AudienceController>()
            .Where(audience => audienceType == AudienceType.NONFOCUS)
            .ToArray();

        foreach (var audience in nonFocusAudiences)
        {
            if (Random.Range(0, 2) == 0) // 50% 확률로 선택
            {
                ChangeAudienceAnimationController(audience);
            }
        }
    }
    
    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // FOCUS 그룹이고, StageManager의 currentStage가 1 또는 2일 경우 상체를 발표자 쪽으로 기울임
            if (audienceType == AudienceType.FOCUS && (stageManager.GetCurrentStage() == 1 || stageManager.GetCurrentStage() == 2))
            {
                animator.SetLookAtWeight(1.0f, 0.5f); // 0.5는 상체 시선의 가중치 (조절 가능)
                animator.SetLookAtPosition(player.position);
            }
            else if (currentTarget)
            {
                // 기본 시선 처리
                animator.SetLookAtWeight(1.0f);
                animator.SetLookAtPosition(lookAtPosition);
            }
        }
    }

    void SetRandomTarget()
    {
        int maxRange;
        
        if (audienceType == AudienceType.FOCUS)
        {
            // FOCUS 그룹
            maxRange = gameObject.CompareTag("LaptopOwner") ? 3 : 2; // 노트북 소유 시 0, 1, 2 선택, 그렇지 않으면 0, 1 선택
        }
        else
        {
            // NONFOCUS 그룹
            maxRange = 4; // 일단 0, 1, 2, 3 중에서 선택
        }
        
        randomChoice = Random.Range(0, maxRange);
        
        if (audienceType == AudienceType.NONFOCUS && !gameObject.CompareTag("LaptopOwner"))
        {
            // NONFOCUS 그룹에서 노트북을 갖고 있지 않은 경우 case 2를 제외하고 0, 1, 3 중에서 선택
            if (randomChoice == 2)
            {
                randomChoice = 3;
            }
        }

        switch (randomChoice)
        {
            case 0:
                currentTarget = player;
                break;
            case 1:
                currentTarget = pptScreen;
                break;
            case 2:
                if (gameObject.CompareTag("LaptopOwner"))
                {
                    // 노트북을 갖고 있는 캐릭터인 경우 노트북을 바라보도록 설정
                    currentTarget = transform.Find("Laptop");
                }
                else if (audienceType == AudienceType.NONFOCUS)
                {
                    // NONFOCUS 그룹에서 노트북이 없는 경우에는 case 3 처리
                    goto case 3;
                }
                break;
            case 3:
                // NONFOCUS 타입일 경우, 시선 고정 해제 및 애니메이션 실행
                if (audienceType == AudienceType.NONFOCUS)
                {
                    TriggerRandomAnimation(stageManager.GetCurrentStage());
                    currentTarget = null;
                    isPlayingAnimation = true; // 애니메이션 재생 시작
                }
                break;
        }
        // 무작위로 대상과 시간을 선택 
        lookTimer = Random.Range(minLookTime, maxLookTime);
    }

    void TriggerRandomAnimation(int stage)
    {
        string[] triggers;
    
        if (stage == 2)
        {
            triggers = new string[] {"TiredStayAwake", "LookingAround", "Stretching"};
        }
        else if (stage == 3)
        {
            triggers = new string[] {"TiredStayAwake", "LookingAround", "Stretching", "Sneezing", "Chatting"};
        }
        else
        {
            return; // 다른 스테이지의 경우 아무것도 하지 않음
        }

        string selectedTrigger = triggers[Random.Range(0, triggers.Length)];
        animator.SetTrigger(selectedTrigger);
    }
    // 애니메이션이 끝나면 다시 상태 설정 모드로 진입
    public void ResetTiredState()
    {
        animator.CrossFade("Idle", 0.5f);
        isPlayingAnimation = false; // 애니메이션 재생 종료
        SetRandomTarget();
        Debug.Log("피곤상태초기화");

        currentTarget = player;
    }
}
