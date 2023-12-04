using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // 게임 종료 함수
    public void EndGame()
    {
        // 앱 종료 로그 출력
        Debug.Log("VR 앱 종료");

        // 애플리케이션 종료
        Application.Quit();

        // 에디터에서 실행 중이면 에디터 종료 (실제 빌드에서는 필요 없음)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
