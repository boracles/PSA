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
    

    public void StartingScript()
    {
        LoadScriptFromFile(scriptFileName);
    }

    void LoadScriptFromFile(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string scriptContent = File.ReadAllText(filePath);
            scriptText.text = scriptContent;
            StartCoroutine(ScrollText());
        }
        else
        {
            Debug.Log("대본 파일을 찾을 수 없습니다.");
        }
    }

    IEnumerator ScrollText()
    {
        RectTransform textRectTransform = scriptText.GetComponent<RectTransform>();
        float scrollDistance = textRectTransform.rect.height*2.0f; // 스크롤할 거리 설정

        Vector2 startPosition = textRectTransform.anchoredPosition;
        Vector2 endPosition = new Vector2(startPosition.x, startPosition.y + scrollDistance);

        while (textRectTransform.anchoredPosition.y < endPosition.y)
        {
            textRectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
            yield return null;
        }
    }
}