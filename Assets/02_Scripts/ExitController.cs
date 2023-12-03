using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
    public StageManager stageManager;
    public Transform exitPoint;
    public float speed = 1.0f;
    private float presentationStartTime;
    private bool presentationStarted = false;

    public void StartPresentation()
    {
        presentationStartTime = Time.time;
        presentationStarted = true;
    }

    void Start()
    {
        stageManager = StageManager.Instance;
    }

    void Update()
    {
        if (presentationStarted && stageManager.GetCurrentStage() == 3)
        {
            if (Time.time - presentationStartTime >= 60.0f) // 1분 후
            {
                StartExitingCharacters();
                presentationStarted = false; // 더 이상 이동하지 않도록 설정
            }
        }
    }

    private void StartExitingCharacters()
    {
        var charactersToExit = GameObject.FindGameObjectsWithTag("OUT");
        foreach (var character in charactersToExit)
        {
            var audienceGazeController = character.GetComponent<AudienceGazeController>();
            if (audienceGazeController)
            {
                audienceGazeController.enabled = false; // 비활성화
            }

            StartCoroutine(MoveCharacter(character.transform));
        }
    }

    private IEnumerator MoveCharacter(Transform character)
    {
        Animator animator = character.GetComponent<Animator>();
        if (animator)
        {
            // 먼저 'STANDING' 애니메이션 실행
            animator.Play("STANDING");
            yield return new WaitForSeconds(0.5f); // 'STANDING' 애니메이션이 재생되는 시간 동안 대기
        }

        float rotationSpeed = speed * 2; // 회전 속도를 높임

        Quaternion targetRotation = Quaternion.LookRotation(exitPoint.position - character.position);

        // 캐릭터가 목표 방향을 향해 완전히 회전할 때까지 기다림
        while (Quaternion.Angle(character.rotation, targetRotation) > 0.1f)
        {
            character.rotation = Quaternion.Slerp(character.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        if (animator)
        {
            // 'Walk' 애니메이션으로 전환
            animator.SetTrigger("Walk");
        }

        // 회전이 완료되면 exitPoint로 이동 시작
        while (Vector3.Distance(character.position, exitPoint.position) > 0.1f)
        {
            Vector3 exitDirection = (exitPoint.position - character.position).normalized;
            character.position += exitDirection * speed * Time.deltaTime;
            yield return null;
        }
    
        character.gameObject.SetActive(false); // exitPoint에 도달하면 오브젝트 비활성화
    }
}
