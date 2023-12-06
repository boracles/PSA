using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptPrompter : MonoBehaviour
{
    public TextMeshProUGUI textDisplay; // TextMeshPro UI 객체
    public TextAsset[] textFiles; // TextAsset 타입의 변수
    
    private int currentPage = 0; // 현재 페이지 인덱스
    private int linesPerPage = 6; // 페이지 당 표시할 줄 수
    private TMP_TextInfo textInfo;
    
    public void StartTextDisplay()
    {
        string sceneName = SceneManager.GetActiveScene().name; // 현재 씬의 이름을 가져옵니다.

        switch(sceneName)
        {
            case "1":
                LoadTextFile(textFiles[0]); // 씬 이름이 "1"일 경우 첫 번째 텍스트 파일을 로드합니다.
                break;
            case "2":
                LoadTextFile(textFiles[1]); // 씬 이름이 "2"일 경우 두 번째 텍스트 파일을 로드합니다.
                break;
            case "3":
                textDisplay.text = "대본이 제공되지 않습니다."; // 씬 이름이 "3"일 경우 아무것도 표시하지 않습니다.
                break;
        }
    }

    void LoadTextFile(TextAsset file)
    {
        string text = file.text; // TextAsset의 내용을 가져옵니다.
        textDisplay.text = text; // 전체 텍스트를 할당합니다.
        textInfo = textDisplay.textInfo;
        currentPage = 0;
        UpdateTextDisplay();
    }

    public void ResetTextDisplay()
    {
        textDisplay.text = ""; // 텍스트를 지웁니다.
        currentPage = 0; // 페이지 인덱스를 초기화합니다.
        // 추가적으로 화면 업데이트를 위한 메서드 호출이 필요할 수 있습니다.
        UpdateTextDisplay();
    }
    
    public void NextPage()
    {
        if (currentPage + linesPerPage < textInfo.lineCount)
        {
            currentPage += linesPerPage;
            UpdateTextDisplay();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage = Mathf.Max(0, currentPage - linesPerPage);
            UpdateTextDisplay();
        }
    }
    
    void UpdateTextDisplay()
    {
        textDisplay.pageToDisplay = currentPage / linesPerPage + 1;
    }
}