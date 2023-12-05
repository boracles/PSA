using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource bgmSource;
    public AudioClip[] bgmClips;

    public AudioSource voiceSource;
    public AudioClip[] voiceClips;
    
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
        if (scene.name == "OnBoarding")
        {
            PlayBGM(0);
            StartCoroutine(ActivateObjectAfterVoiceClip(0, 17f));      
        }
        else if (scene.name == "Ending")
        {
            PlayBGM(0);
        }
        else if (scene.name == "Meditation")
        {
            PlayBGM(1);
            StartCoroutine(PlayVoiceClipAndActivateObject(1, 3f, 4f)); // Meditation 씬 로드 후 처리
        }
        else if (scene.name == "1") // 1번 씬에 대한 처리
        {
            StartCoroutine(PlayVoiceClipAndActivateObject(2, 0f, 3f)); // 2번 VoiceClip 재생 후 처리
            PlayBGM(3);
        }
        else if (scene.name == "2") // 2번 씬에 대한 처리
        {
            StartCoroutine(PlayVoiceClipAndActivateButtonInScene2(4, 9f)); // 4번 VoiceClip 재생 후 처리
            PlayBGM(3);
        }
        else if (scene.name == "3") // 3번 씬에 대한 처리
        {
            StartCoroutine(PlayVoiceClipAndActivateObjectInScene3(5, 9f)); // 5번 VoiceClip 재생 후 처리
            PlayBGM(3);
        }
        else if(scene.name == "1_Meditation30s"||scene.name == "2_Meditation30s"||scene.name == "3_Meditation30s")
        {
            PlayBGM(2);
        }

    }
        IEnumerator PlayVoiceClipAndActivateObjectInScene3(int clipIndex, float delayAfterClipStart)
        {
            PlayVoiceClip(clipIndex); // VoiceClip 재생
            yield return new WaitForSeconds(delayAfterClipStart); // 9초 대기

            // "UI/Canvas/Frame/Button" 오브젝트 찾기 및 활성화
            GameObject buttonObject = GameObject.Find("UI/Canvas/Frame/Button");
            if (buttonObject != null)
            {
                buttonObject.SetActive(true);

                // 오브젝트에 할당된 AudioSource 찾아 재생
                AudioSource audio = buttonObject.GetComponent<AudioSource>();
                if (audio != null)
                {
                    audio.Play();
                }
                else
                {
                    Debug.LogError("오브젝트에 AudioSource 컴포넌트가 없습니다.");
                }
            }
            else
            {
                Debug.LogError("지정된 경로에 오브젝트를 찾을 수 없습니다.");
            }
        }
    
    IEnumerator PlayVoiceClipAndActivateButtonInScene2(int clipIndex, float delayAfterClipStart)
    {
        PlayVoiceClip(clipIndex); // VoiceClip 재생
        yield return new WaitForSeconds(delayAfterClipStart); // 9초 대기

        // "UI/Canvas/Frame/Button" 오브젝트 찾기 및 활성화
        GameObject buttonObject = GameObject.Find("UI/Canvas/Frame/Button");
        if (buttonObject != null)
        {
            buttonObject.SetActive(true);

            // 오브젝트에 할당된 AudioSource 찾아 재생
            AudioSource audio = buttonObject.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play();
            }
            else
            {
                Debug.LogError("오브젝트에 AudioSource 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("지정된 경로에 오브젝트를 찾을 수 없습니다.");
        }
    }
    
    IEnumerator PlayVoiceClipAndActivateObject(int clipIndex, float delayBeforeClip, float delayAfterClipStart)
    {
        yield return new WaitForSeconds(delayBeforeClip);
        PlayVoiceClip(clipIndex);

        yield return new WaitForSeconds(delayAfterClipStart);

        GameObject buttonObject = GameObject.Find("UI/Canvas/Frame/Button");
        if (buttonObject != null)
        {
            buttonObject.SetActive(true);

            AudioSource audio = buttonObject.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play();
            }
            else
            {
                Debug.LogError("오브젝트에 AudioSource 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("지정된 경로에 오브젝트를 찾을 수 없습니다.");
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
    
    IEnumerator ActivateObjectAfterVoiceClip(int clipIndex, float delayAfterClip)
    {
        yield return new WaitForSeconds(3f); // 씬 로드 후 3초 대기
        PlayVoiceClip(clipIndex); // 음성 클립 재생
        
        yield return new WaitForSeconds(delayAfterClip);

        // "BUTTON" 태그를 가진 오브젝트 찾기 및 활성화
        GameObject buttonObject = GameObject.Find("UI/Canvas/Frame/Button");

        if (buttonObject != null)
        {
            buttonObject.SetActive(true);
            // 오브젝트에 할당된 AudioSource를 찾아 재생
            AudioSource audio = buttonObject.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play();
            }
            else
            {
                Debug.LogError("오브젝트에 AudioSource 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("BUTTON 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    // VoiceClip 재생 메서드
    public void PlayVoiceClip(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < voiceClips.Length)
        {
            voiceSource.clip = voiceClips[clipIndex];
            voiceSource.Play();
        }
        else
        {
            Debug.LogError("VoiceClip 인덱스가 유효하지 않습니다.");
        }
    }
    
    // 외부에서 호출할 수 있는 새로운 메서드 추가
    public void PlayVoiceClipAndActivateButton()
    {
        if (SceneManager.GetActiveScene().name == "1")
        {
            StartCoroutine(PlayVoiceClipAndActivateButtonCoroutine(3, 9f));
        }
    }

    // 코루틴 메서드 정의
    IEnumerator PlayVoiceClipAndActivateButtonCoroutine(int clipIndex, float delayAfterClipStart)
    {
        PlayVoiceClip(clipIndex);
        yield return new WaitForSeconds(delayAfterClipStart);

        // Frame_2 내의 버튼 오브젝트를 활성화
        GameObject buttonObject = GameObject.Find("UI/Canvas/Frame_2/Button");
        if (buttonObject != null)
        {
            buttonObject.SetActive(true);
        }
        else
        {
            Debug.LogError("지정된 경로에 오브젝트를 찾을 수 없습니다.");
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
