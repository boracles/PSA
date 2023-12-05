using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource bgmSource;
    public AudioClip[] bgmClips;

    public float crossfadeDuration = 1.0f; // 크로스페이드 지속 시간
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // "OnBoarding" 혹은 "Ending" 씬이 로드되면 0번 음악 재생
        if (scene.name == "OnBoarding" || scene.name == "Ending")
        {
            PlayBGM(0);
        }
        // "Meditation" 씬이 로드되면 1번 음악 재생
        else if (scene.name == "Meditation")
        {
            PlayBGM(1);
        }
    }

    public void PlayBGM(int bgmIndex)
    {
        if (bgmIndex >= 0 && bgmIndex < bgmClips.Length)
        {
            StartCoroutine(CrossfadeBGM(bgmClips[bgmIndex]));
        }
        else
        {
            Debug.LogError("BGM 인덱스가 유효하지 않습니다.");
        }
    }

    IEnumerator CrossfadeBGM(AudioClip newClip)
    {
        float timeElapsed = 0;

        // 현재 재생 중인 BGM 볼륨을 점차 줄임
        while (timeElapsed < crossfadeDuration)
        {
            bgmSource.volume = Mathf.Lerp(1.0f, 0.0f, timeElapsed / crossfadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        bgmSource.clip = newClip;
        bgmSource.Play();

        // 새로운 BGM 볼륨을 점차 증가
        timeElapsed = 0;
        while (timeElapsed < crossfadeDuration)
        {
            bgmSource.volume = Mathf.Lerp(0.0f, 1.0f, timeElapsed / crossfadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
