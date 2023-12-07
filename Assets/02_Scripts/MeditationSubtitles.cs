using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MeditationSubtitles : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI number;
    public AudioSource meditationVoice;
    private bool subtitlesStarted = false; // 자막 시작 플래그
    public GameObject meditationCircle;
    public GameObject meditationWave;
    private ParticleSystem waveParticleSystem;
    private ParticleSystem circleParticleSystem;

    private void Start()
    {
        waveParticleSystem = meditationWave.GetComponent<ParticleSystem>();
        circleParticleSystem = meditationCircle.GetComponent<ParticleSystem>();
        number.text = "";
    }

    public void StartSubtitles()
    {
        subtitlesStarted = true; // 자막 시작 표시
        StartCoroutine(UpdateSubtitles()); // 코루틴 시작
    }
    IEnumerator UpdateSubtitles()
    {
        while (subtitlesStarted)
        {
            if (meditationVoice.isPlaying)
            {
                float currentTime = meditationVoice.time; // 오디오 재생 시간을 기준으로 타이머 설정
                string currentScene = SceneManager.GetActiveScene().name;

                if (currentScene == "1_Meditation30s" || currentScene == "2_Meditation30s" || currentScene == "3_Meditation30s")
                {
                    DisplaySubtitlesForSpecificScenes(currentTime);
                }
                else if (currentScene == "Meditation")
                {
                    DisplaySubtitlesForMeditationScene(currentTime);
                }
            }
            else
            {
                HandleAudioEnd();
            }

            yield return new WaitForSeconds(0.1f); // 예를 들어, 0.1초 간격으로 업데이트
        }
    }

    // 30초 명상
    void DisplaySubtitlesForSpecificScenes(float currentTime)
    {
        if (currentTime < 7f)
                subtitleText.text = "발표 중 나도 모르게 긴장했던 근육들을\n" +
                                    "풀어주기 위해 심호흡을 하겠습니다.";
        else if (currentTime < 12f)
        {
                subtitleText.text = "천천히 숨을 들이마시며 복식호흡 해보세요.";
                
                ParticleSystem.SizeOverLifetimeModule sizeOverLifeTime = waveParticleSystem.sizeOverLifetime;

                sizeOverLifeTime.enabled = true;
            
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0.0f, 1.0f); // 시간 0에서의 크기 
                curve.AddKey(1.0f, 0.0f); // 시간 1에서의 크기

               sizeOverLifeTime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
        }
        else if (currentTime < 13f)
        {
            subtitleText.text = "하나";
            number.text = "1";
        }
        else if (currentTime < 14f)
        {
            subtitleText.text = "둘";
            number.text = "2";
        }
        else if (currentTime < 16f)
        {
            subtitleText.text = "셋";
            number.text = "3";
        }
        else if (currentTime < 17f)
        {
            subtitleText.text = "넷";
            number.text = "4";
        }
        else if (currentTime < 21f)
        {
            subtitleText.text = "잠깐 그대로 숨을 멈추세요.";
            number.text = ""; 
            meditationWave.SetActive(false);
        }
        else if (currentTime < 22f)
        { 
            subtitleText.text = "하나";
            number.text = "1";
        }
        else if (currentTime < 23f)
        {
            subtitleText.text = "둘";
            number.text = "2";
        }
        else if (currentTime < 25f)
        {
            subtitleText.text = "셋";
            number.text = "3";
        }
        else if (currentTime < 28f)
        {
            subtitleText.text = "다시 천천히 숨을 내쉬세요.";
            number.text = "";
            
            meditationWave.SetActive(true);
            
                var mainModule = circleParticleSystem.main;
                mainModule.startLifetime = 10.0f;
            
                ParticleSystem.SizeOverLifetimeModule sizeOverLifeTime = waveParticleSystem.sizeOverLifetime;

                sizeOverLifeTime.enabled = true;
            
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0.0f, 0.0f); // 시간 0에서의 크기 
                curve.AddKey(1.0f, 1.0f); // 시간 1에서의 크기

                sizeOverLifeTime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
        }
        else if (currentTime < 29f)
        {
            subtitleText.text = "하나";
            number.text = "1";
        }
        else if (currentTime < 31f)
        {
                subtitleText.text = "둘";
                number.text = "2";
        }
        else if (currentTime < 32f)
        {
                subtitleText.text = "셋";
                number.text = "3";
        }
        else if (currentTime < 34f)
        {
                subtitleText.text = "넷";
                number.text = "4";
        }
        else if (currentTime < 35f)
        {
                subtitleText.text = "다섯";
                number.text = "5";
        }
        else if (currentTime < 37f)
        {
            subtitleText.text = "잘 하셨습니다.";
            number.text = "";    
            meditationWave.SetActive(false);
        }
    }

    // 3분 명상
    void DisplaySubtitlesForMeditationScene(float currentTime)
    {
        if (currentTime < 3.5f )
            subtitleText.text = "발표 연습에 참여하신 사용자님\n" +
                                "반갑습니다.";
        else if (currentTime < 8f)
            subtitleText.text = "우리는 당신이 발표를 하기 전,\n" +
                                "불안한 마음을 충분히 가다듬고.";
        else if (currentTime < 13f)
            subtitleText.text = "당신의 열정을 마음 깊은 곳에서부터\n" +
                                "이끌어 내기를 원합니다.";
        else if (currentTime < 18f)
            subtitleText.text = "먼저, 안정적인 호흡을 통해\n" +
                                "몸의 긴장을 풀어보겠습니다.";
        else if (currentTime < 22f)
            subtitleText.text = "편안한 자세로 서서\n" +
                                "한쪽 손을 가슴에 올려두세요.";
        else if (currentTime < 28f)
        {
            subtitleText.text = "4초 동안 천천이 숨을 들이마시며,\n" +
                                "배가 부풀어 오르는 것을 느껴보세요.";

            ParticleSystem.SizeOverLifetimeModule sizeOverLifeTime = waveParticleSystem.sizeOverLifetime;

            sizeOverLifeTime.enabled = true;
            
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 1.0f); // 시간 0에서의 크기 
            curve.AddKey(1.0f, 0.0f); // 시간 1에서의 크기

            sizeOverLifeTime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
        }
        else if (currentTime < 29f)
        {
            subtitleText.text = "하나";
            number.text = "1";
        }
        else if (currentTime < 32f)
        {
            subtitleText.text = "둘";
            number.text = "2";
        }
        else if (currentTime < 34f)
        {
            subtitleText.text = "셋";
            number.text = "3";
        }
        else if (currentTime < 36f)
        {
            subtitleText.text = "넷";
            number.text = "4"; 
        }
        else if (currentTime < 39f)
        {
            subtitleText.text = "이제 3초 동안 호흡을 잠깐 멈추세요.";
            number.text = "";
            
            meditationWave.SetActive(false);
        }
        else if (currentTime < 41f)
        {
            subtitleText.text = "하나";
            number.text = "1";
        }
        else if (currentTime < 43f)
        {
            subtitleText.text = "둘";
            number.text = "2";
        }
        else if (currentTime < 45f)
        {
            subtitleText.text = "셋";
            number.text = "3";
        }
        else if (currentTime < 50f)
        {
            subtitleText.text = "다시 배가 홀쭉해지도록\n" +
                                "5초에 거쳐 천천히 숨을 내쉽니다.";
            number.text = "";
            
            meditationWave.SetActive(true);
            
            var mainModule = circleParticleSystem.main;
            mainModule.startLifetime = 10.0f;
            
            ParticleSystem.SizeOverLifetimeModule sizeOverLifeTime = waveParticleSystem.sizeOverLifetime;

            sizeOverLifeTime.enabled = true;
            
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 0.0f); // 시간 0에서의 크기 
            curve.AddKey(1.0f, 1.0f); // 시간 1에서의 크기

            sizeOverLifeTime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
        }
        else if (currentTime < 52f)
        {
            subtitleText.text = "하나";
            number.text = "1";
        }
        else if (currentTime < 54f)
        {
            subtitleText.text = "둘";
            number.text = "2";
        }
        else if (currentTime < 55f)
        {
            subtitleText.text = "셋";
            number.text = "3";
        }
        else if (currentTime < 57f)
        {
            subtitleText.text = "넷";
            number.text = "4";
        }
        else if (currentTime <59f)
        {
            subtitleText.text = "다섯";
            number.text = "5";
        }
        else if (currentTime < 62f)
        {
            subtitleText.text = "잘 하셨습니다.";
            number.text = "";
        }
        else if (currentTime < 66f)
            subtitleText.text = "자, 이제 눈을 감고\n" +
                                "목소리가 이끄는 대로 행동합니다.";
        else if (currentTime < 73f)
            subtitleText.text = "자신이 가장 안전하다고 느끼는 장소,\n" +
                                "편안하다고 느끼는 장소를 떠올려보세요.";
        else if (currentTime <77)
        {
            subtitleText.text = "";
        }
        else if (currentTime < 80f)
            subtitleText.text = "그 장소를 천천히 둘러보세요.";
        else if (currentTime <85)
        {
            subtitleText.text = "";
        }
        else if (currentTime < 89f)
            subtitleText.text = "앞과 옆, 뒤를 둘러보면서\n" +
                                "무엇이 보이는지 바라봅니다.";
        else if (currentTime < 94f)
            subtitleText.text = "";
        else if (currentTime < 98f)
            subtitleText.text = "이제 그 장소에서 들리는\n" +
                                "소리에 귀를 기울여보세요.";
        else if (currentTime < 101f)
            subtitleText.text = "어떤 소리가 들리는지 집중해봅시다.";
        else if (currentTime < 106f)
            subtitleText.text = "";
        else if (currentTime < 109f)
            subtitleText.text = "그 장소에서 편안한 자세를 취해보세요.";
        else if (currentTime < 114f)
            subtitleText.text = "";
        else if (currentTime < 118.0f)
            subtitleText.text = "그 장소에서 어떤 냄새가 나는지\n" +
                                "한 번 맡아봅니다.";
        else if (currentTime < 122f)
            subtitleText.text = "";
        else if (currentTime < 130f)
            subtitleText.text = "때때로 우리는 과거의 부정적인 경험으로 하여금\n" +
                                "현재에 집중하지 못하고 헤맬 수 있습니다.";
        else if (currentTime < 136f)
            subtitleText.text = "또한, 우리가 생각하고 원하는 상황과\n" +
                                "일치하지 않는 데서 오는 긴장감으로 인해";
        else if (currentTime <141f)
            subtitleText.text = "마음이 불편하거나\n" +
                                "그 상황을 빠르게 벗어나고 싶어하죠.";
        else if (currentTime < 148f)
            subtitleText.text = "하지만, 그 이후에 돌아오는 건\n" +
                                "그 긴장감을 견뎌내지 못한\n" +
                                "자신을 원망하는 마음입니다.";
        else if (currentTime < 152f)
            subtitleText.text = "그것이 자신을 끊임없이 괴롭게 만들죠.";
        else if (currentTime < 157f)
            subtitleText.text = "그럴 때 마다 안전한 장소를 떠올리며\n" +
                                "긴장과 불안을 없애보세요.";
        else if (currentTime < 161f)
            subtitleText.text = "그 장소에서 편안한 느낌을 생각하며\n" +
                                "눈을 뜨세요.";
        else if (currentTime < 166f)
            subtitleText.text = "";
        else if (currentTime < 169f)
            subtitleText.text = "발표를 시작할 준비를 끝 마쳤습니다.";
        else if (currentTime < 172f)
            subtitleText.text = "본격적으로 발표 연습을 시작해보겠습니다.";
    }
    
    void HandleAudioEnd()
    {
            subtitleText.text = ""; // 자막 숨기기
            subtitlesStarted = false;
            StageManager.Instance.SceneChange(); // 씬 전환
    }
}
