using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EyeTracking : MonoBehaviour
{
    private Camera VRCamera;
    private Dictionary<string, float> gazeTime = new Dictionary<string, float>();
    private float presentationStartTime;
    private float presentationEndTime;

    
    void Start()
    {
        VRCamera = GetComponent<Camera>();
        InitializeGazeTimeDictionary();
    }

    void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 현재 씬 이름이 "1", "2", 또는 "3"인 경우에만 시선 추적 로직을 실행합니다.
        if (currentSceneName == "1" || currentSceneName == "2" || currentSceneName == "3")
        {
            RaycastHit hit;
            Ray ray = new Ray(VRCamera.transform.position, VRCamera.transform.forward);

            if (Physics.Raycast(ray, out hit))
            {
                string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);

                if (gazeTime.ContainsKey(layerName))
                {
                    gazeTime[layerName] += Time.deltaTime;
                }
            }
        }
    }
// 발표 시작 버튼을 눌렀을 때 호출되는 함수
    public void StartPresentation()
    {
        presentationStartTime = Time.time;
    }

    // 발표 종료 버튼을 눌렀을 때 호출되는 함수
    public void EndPresentation()
    {
        presentationEndTime = Time.time;
        CalculateGazePercentages();
        SaveGazeDataToFile();
    }

    // 각 레이어별 시선 체류 비율을 계산하는 함수
    private void CalculateGazePercentages()
    {
        float totalPresentationTime = presentationEndTime - presentationStartTime;
        foreach (var item in gazeTime)
        {
            float percentage = (item.Value / totalPresentationTime) * 100;
            Debug.Log(item.Key + ": " + percentage.ToString("F2") + "%");
        }
    }

    // 초기화 함수: 모든 레이어에 대한 시간 추적을 0으로 설정
    private void InitializeGazeTimeDictionary()
    {
        gazeTime["HEADRIGHT"] = 0;
        gazeTime["HEADMIDDLERIGHT"] = 0;
        gazeTime["HEADMIDDLE"] = 0;
        gazeTime["HEADMIDDLELEFT"] = 0;
        gazeTime["HEADLEFT"] = 0;
        gazeTime["BODYLEFT"] = 0;
        gazeTime["BODYMIDDLELEFT"] = 0;
        gazeTime["BODYMIDDLE"] = 0;
        gazeTime["BODYMIDDLERIGHT"] = 0;
        gazeTime["BODYRIGHT"] = 0;
        gazeTime["SCRIPT"] = 0;
        gazeTime["TIMER"] = 0;
        gazeTime["FLOOR"] = 0;
    }
    
    private void SaveGazeDataToFile()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string fileName = "GazeData_Scene" + currentSceneName + ".txt";

        StringBuilder sb = new StringBuilder();
        foreach (var item in gazeTime)
        {
            sb.AppendLine(item.Key + ": " + item.Value.ToString("F2"));
        }

        string filePath = Path.Combine(Application.dataPath, fileName);
        File.WriteAllText(filePath, sb.ToString());
        Debug.Log("시선 추적 데이터가 저장되었습니다: " + filePath);
    }
}
