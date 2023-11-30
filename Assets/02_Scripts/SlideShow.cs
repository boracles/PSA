using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideShow : MonoBehaviour
{
    public Material screenMaterial;
    private Texture2D[] slides;     //슬라이드 이미지들을 저장할 배열
    private int currentSlide = 0;   // 현재 슬라이드 인덱스
    
    void Start()
    {
        //Resources 폴더에서 모든 Texutre2D 타임의 이미지를 로드 
        slides = Resources.LoadAll<Texture2D>("");
        
        //로드된 이미지가 없는 경우 경고 출력
        if (slides.Length == 0)
        {
            Debug.LogWarning("슬라이드 이미지가 존재하지 않습니다.");
        }
    }

    void ChangeSlide(int slideIndex)
    {
        if (slideIndex >= 0 && slideIndex < slides.Length)
        {
            // 모니터의 Material에 새로운 텍스쳐 적용
            screenMaterial.mainTexture = slides[slideIndex];
        }
        else
        {
            Debug.LogError("슬라이드의 인덱스가 유효하지 않습니다.");
        }
            
    }
}
