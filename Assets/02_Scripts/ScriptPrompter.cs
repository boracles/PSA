using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ScriptPrompter : MonoBehaviour
{

    public TextMeshProUGUI scriptText;
    public string scriptFileName;       // 대본 텍스트 파일 이름
    public float scrollSpeed = 2.0f;    // 텍스트 스크롤 속도 

    private bool isScrolling = false;
    private Coroutine scrollingCoroutine;
    private bool isScriptLoaded = false;    // 대본이 로드되었는지 여부

    public GameObject playButton;
    public GameObject pauseButton;


    public void ToggleScript()
    {
        if (!isScriptLoaded)
        {
            LoadScriptFromFile(scriptFileName);
            isScriptLoaded = true;
            Debug.Log("대본 로드됨");
        }
        else
        {
            ToggleScrolling();
        }
    }
    
    private void ToggleScrolling()
    {
        if (isScrolling)
        {
            // 스크롤 중지
            StopScrolling();
        }
        else
        {
            // 스크롤 시작
            StartScrolling();
        }
    }

    public void CompleteScript()
    {
        StopScrolling();
        isScriptLoaded = false;
        scriptText.text = ""; // 텍스트 내용 비우기
        Debug.Log("대본 완료 및 리셋");
    }

    private void OnDisable()
    {
        StopScrolling();
        isScriptLoaded = false;
        scriptText.text = ""; // 텍스트 내용 비우기
        Debug.Log("대본 완료 및 리셋");
    }

    private void StartScrolling()
    {
        if (!isScrolling)
        {
            if (scrollingCoroutine != null)
            {
                StopCoroutine(scrollingCoroutine);
                playButton.SetActive(true);
                pauseButton.SetActive(false);
                Debug.Log("스크롤 중지");
            }
            
            scrollingCoroutine = StartCoroutine(ScrollText());
            isScrolling = true;
            playButton.SetActive(false);
            pauseButton.SetActive(true);
            Debug.Log("스크롤 시작");
        }
    }

    private void StopScrolling()
    {
        if (scrollingCoroutine != null)
        {
            StopCoroutine(scrollingCoroutine);
            scrollingCoroutine = null;
            playButton.SetActive(true);
            pauseButton.SetActive(false);
            Debug.Log("스크롤 중지");
        }
        isScrolling = false;
    }

    void LoadScriptFromFile(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string scriptContent = File.ReadAllText(filePath);
            scriptText.text = scriptContent;
            StartScrolling();
        }
        else
        {
            Debug.Log("대본 파일을 찾을 수 없습니다.");
        }
    }

    IEnumerator ScrollText()
    {
        RectTransform textRectTransform = scriptText.GetComponent<RectTransform>();
        float scrollDistance = textRectTransform.rect.height*10.0f; // 스크롤할 거리 설정

        Vector2 startPosition = textRectTransform.anchoredPosition;
        Vector2 endPosition = new Vector2(startPosition.x, startPosition.y + scrollDistance);

        while (textRectTransform.anchoredPosition.y < endPosition.y)
        {
            textRectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
            yield return null;
        }
        
        // 스크롤이 끝나면 스크롤 상태 업데이트
        isScrolling = false;
    }
}