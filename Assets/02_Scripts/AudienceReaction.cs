using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceReaction : MonoBehaviour
{
    private Animator audienceAnimator;
    void Start()
    {
        audienceAnimator = GetComponent<Animator>();
    }

    // 청중의 반응을 긍적적으로 변경
    public void SetPositiveReaction()
    {
        audienceAnimator.SetTrigger("PositiveReaction");
    }
    
    // 청중의 반응을 부정적으로 변경 
    public void SetNegativeReaction()
    {
        audienceAnimator.SetTrigger("NegativeReaction");
    }
    
    // 청중이 발표 종료 후 박수를 치게 하는 반응
    public void Applaud()
    {
        audienceAnimator.SetTrigger("Applaud");
    }
    
    // 청중이 자리를 이탈하는 반응
    public void LeaveSeat()
    {
        audienceAnimator.SetTrigger("LeaveSeat");
    }
    
    // 청중이 기침이나 말소리를 내는 반응
    public void CoughorTalk()
    {
        audienceAnimator.SetTrigger("CoughorTalk");
    }
    
    // 특정 상황에 따라 청중의 반응을 점진적으로 변경
    public void UpdateReaction(int presentationStage)
    {
        switch (presentationStage)
        {
            case 1:
                SetPositiveReaction();
                break;
            case 2:
                SetNegativeReaction();
                // 추가적으로 미소나 기타 긍정적인 표정 유지
                break;
            case 3:
                CoughorTalk();
                break;
            case 4:
                LeaveSeat();
                break;
            default:
                Debug.Log("발표 단계가 설정되지 않았습니다.");
                break;
        }
    }
}
