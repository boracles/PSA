using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ScriptPrompter : MonoBehaviour
{
    public TextMeshProUGUI textDisplay; // TextMeshPro UI 객체
    public TextAsset textFile; // TextAsset 타입의 변수
    private List<string> textPages; // 텍스트 페이지를 저장할 리스트
    private int currentPage = 0; // 현재 페이지 인덱스

    public void StartTextDisplay()
    {
        if (textFile != null)
        {
            LoadTextFile(textFile);
        }
    }

    void LoadTextFile(TextAsset file)
    {
        string text = file.text; // TextAsset의 내용을 가져옵니다.
        textPages = new List<string>();

        for (int i = 0; i < text.Length; i += 96)
        {
            if (i + 96 < text.Length)
                textPages.Add(text.Substring(i, 96));
            else
                textPages.Add(text.Substring(i));
        }

        currentPage = 0;
        if (textPages.Count > 0)
            textDisplay.text = textPages[currentPage]; // 첫 페이지를 표시합니다.
    }

    public void ResetTextDisplay()
    {
        textDisplay.text = ""; // 텍스트를 지웁니다.
        currentPage = 0; // 페이지 인덱스를 초기화합니다.
        textPages.Clear(); // 텍스트 페이지 리스트를 초기화합니다.
    }
    
    public void NextPage()
    {
        if (currentPage < textPages.Count - 1)
        {
            currentPage++;
            textDisplay.text = textPages[currentPage];
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            textDisplay.text = textPages[currentPage];
        }
    }
}