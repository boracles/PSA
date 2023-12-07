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
    
    public float minAnimationSpeed = 0.75f; // 최소 애니메이션 속도
    public float maxAnimationSpeed = 1.25f; // 최대 애니메이션 속도

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
        
        if (stageManager.GetCurrentStage() == 2 || stageManager.GetCurrentStage() == 3)
        {
            UpdateAnimationControllers();
        }

        // NONFOCUS 타입이고 randomChoice가 3이며, 현재 Idle 상태인 경우, 다시 SetRandomTarget() 호출
        if (audienceType == AudienceType.NONFOCUS && randomChoice == 3 && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            SetRandomTarget();
            isPlayingAnimation = false;
        }
    }

    void UpdateAnimationControllers()
    {
        foreach (var audiencePair in audienceChangeTimers.ToList())
        {
            var audience = audiencePair.Key;
            var timer = audiencePair.Value;

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ChangeAudienceAnimationController(audience);
                // 더 큰 범위의 랜덤 타이머 값으로 다시 설정
                audienceChangeTimers[audience] = Random.Range(15f, 30f);            }
            else
            {
                audienceChangeTimers[audience] = timer;
            }
        }
    }
    
    private void InitializeAudienceChangeTimers()
    {
        var audiences = FindObjectsOfType<AudienceController>();

        foreach (var audience in audiences)
        {
            if (audience.audience.audienceType == (global::AudienceType) AudienceType.NONFOCUS)
            {
                // 각 청중에 대한 더 큰 범위의 랜덤 타이머 설정
                audienceChangeTimers[audience] = Random.Range(5f, 25f);
                
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

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // FOCUS 그룹이고, StageManager의 currentStage가 1 또는 2일 경우 상체를 발표자 쪽으로 기울임
            if (audienceType == AudienceType.FOCUS && (stageManager.GetCurrentStage() == 1 || stageManager.GetCurrentStage() == 2))
            {
                animator.SetLookAtWeight(1.0f, 0.5f); // 상체 시선의 가중치 조절
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

    // 애니메이션 속도를 랜덤하게 조절하는 함수
    private void AdjustAnimationSpeed()
    {
        if (animator)
        {
            float randomSpeed = Random.Range(minAnimationSpeed, maxAnimationSpeed);
            animator.speed = randomSpeed;
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

        AdjustAnimationSpeed(); // 애니메이션 속도 조절

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

        AdjustAnimationSpeed(); // 애니메이션 속도 조절

        string selectedTrigger = triggers[Random.Range(0, triggers.Length)];
        animator.SetTrigger(selectedTrigger);
    }
    
    // 애니메이션이 끝나면 다시 상태 설정 모드로 진입
    public void ResetTiredState()
    {
        isPlayingAnimation = false; // 애니메이션 재생 종료
        SetRandomTarget();
        Debug.Log("피곤상태초기화");

        AdjustAnimationSpeed(); // 애니메이션 속도 조절

        currentTarget = player;
    }
    
    // 발표 완료 시 박수치기
    public void StartClapping()
    {
        if (animator)
        {
            animator.SetTrigger("Clapping");
            SoundManager.Instance.sfxSource = SoundManager.Instance.transform.GetChild(0).GetComponent<AudioSource>();
            SoundManager.Instance.sfxSource.clip = SoundManager.Instance.sfxClips[2];
            SoundManager.Instance.sfxSource.loop = true;
            SoundManager.Instance.sfxSource.volume = 0.2f;
            SoundManager.Instance.sfxSource.Play();
        }
    }

    public void PlayTypingSound()
    {
        SoundManager.Instance.sfxSource = gameObject.GetComponent<AudioSource>();
        SoundManager.Instance.sfxSource.clip = SoundManager.Instance.sfxClips[3];
        SoundManager.Instance.sfxSource.Play();
    }

    public void StopTypingSound()
    {
        SoundManager.Instance.sfxSource.Stop();
    }

    public void PlayIdleSound()
    {
        SoundManager.Instance.sfxSource = gameObject.GetComponent<AudioSource>();
        SoundManager.Instance.sfxSource.clip = SoundManager.Instance.sfxClips[4];
        SoundManager.Instance.sfxSource.Play();
    }

    public void StopIdleSound()
    {
        SoundManager.Instance.sfxSource.Stop();
    }
}
