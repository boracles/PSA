using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudienceGazeController : MonoBehaviour
{
    private Transform player;
    private Transform pptScreen;
    private Animator animator;

    public float minLookTime = 2.0f;
    public float maxLookTime = 5.0f;
    public float transitionSpeed = 1.0f;    // 시선 이동 속도
    private float lookTimer;
    private Transform currentTarget;
    private Vector3 lookAtPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        pptScreen = GameObject.Find("Level").transform.Find("PPTScreen");

        if (transform.Find("Laptop"))
        {
            gameObject.tag = "LaptopOwner";
        }
        
        SetRandomTarget();
    }

    void Update()
    {
        lookTimer -= Time.deltaTime;
        if (lookTimer <= 0)
        {
            SetRandomTarget();
        }

        if (currentTarget)
        {
            lookAtPosition = Vector3.Lerp(lookAtPosition, currentTarget.position, Time.deltaTime * transitionSpeed);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator && currentTarget)
        {
            // IK의 가중치를 설정
            animator.SetLookAtWeight(1.0f);

            // 캐릭터가 플레이어의 카메라를 바라보게 설정
            animator.SetLookAtPosition(lookAtPosition); // 현재의 lookAtPosition을 사용하여 부드러운 움직임 구현

        }
    }

    void SetRandomTarget()
    {
        int randomChoice;

        if (gameObject.CompareTag("LaptopOwner"))
        {
            // 노트북을 갖고 있는 캐릭터라면 노트북을 바라보도록 설정
            randomChoice = Random.Range(0, 3);
        }
        else
        {
            randomChoice = Random.Range(0, 2);
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
                break;
        }
        // 무작위로 대상과 시간을 선택 
        lookTimer = Random.Range(minLookTime, maxLookTime);
    }
}
