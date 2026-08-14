using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MusicManager : SingletonMonobehaviour<MusicManager>
{
    private AudioSource musicAudioSource = null;
    private AudioClip currentAudioClip = null;
    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInMusicCoroutine;
    public int musicVolume = 10;

    protected override void Awake()
    {
        base.Awake();
        
        //加载组件
        musicAudioSource = GetComponent<AudioSource>();
        
        //开始时声音关闭
        GameResources.Instance.musicOffSnapshot.TransitionTo(0f);
    }

    private void Start()
    {
        //检查音量设置是否已保存在 PlayerPrefs 中，如果有，则检索并设置它们
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetInt("MusicVolume");
        }

        SetMusicVolume(musicVolume);
    }

    private void OnDisable()
    {
        //Save volume settings in playerprefs   将音量设置保存在 PlayerPrefs 中
        PlayerPrefs.SetInt("MusicVolume", musicVolume);
    }

    public void PlayMusic(MusicTrackSO musicTrack, float fadeOutTime = Settings.musicFadeOutTime,
        float fadeInTime = Settings.musicFadeInTime)
    {
        //播放音轨
        StartCoroutine(PlayMusicRoutine(musicTrack, fadeOutTime, fadeInTime));
    }

    /// <summary>
    /// 播放房间音乐协程
    /// </summary>
    /// <param name="musicTrack"></param>
    /// <param name="fadeOutTime"></param>
    /// <param name="fadeInTime"></param>
    /// <returns></returns>
    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrack, float fadeOutTime, float fadeInTime)
    {
        //如果淡出协程已在运行，则停止该协程
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }
        
        //若淡入协程已在运行，则将其停止
        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }
        
        //若音乐曲目已改变，则播放新的音乐曲目
        if (musicTrack.musicClip != currentAudioClip)
        {
            currentAudioClip = musicTrack.musicClip;

            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));
            
            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrack, fadeInTime));
        }

        yield return null;
    }

    /// <summary>
    /// 淡出音乐协程
    /// </summary>
    /// <param name="fadeOutTime"></param>
    /// <returns></returns>
    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        GameResources.Instance.musicLowSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }

    /// <summary>
    /// 淡入音乐协程
    /// </summary>
    /// <param name="musicTrack"></param>
    /// <param name="fadeInTime"></param>
    /// <returns></returns>
    private IEnumerator FadeInMusic(MusicTrackSO musicTrack, float fadeInTime)
    {
        //设置片段并播放
        musicAudioSource.clip = musicTrack.musicClip;
        musicAudioSource.volume = musicTrack.musicVolume;
        musicAudioSource.Play();

        GameResources.Instance.musicOnFullSnapshot.TransitionTo(fadeInTime);
        
        yield return new WaitForSeconds(fadeInTime);
    }

    /// <summary>
    /// 增大音量
    /// </summary>
    public void IncreaseMusicVolume()
    {
        int maxMusicVolume = 20;

        if (musicVolume >= maxMusicVolume) return;

        musicVolume += 1;
        
        SetMusicVolume(musicVolume);
    }

    /// <summary>
    /// 减小音量
    /// </summary>
    public void DecreaseMusicVolume()
    {
        if (musicVolume == 0) return;
        
        musicVolume -= 1;
        
        SetMusicVolume(musicVolume);
    }

    /// <summary>
    /// 设置音乐音量
    /// </summary>
    /// <param name="musicVolume"></param>
    public void SetMusicVolume(int musicVolume)
    {
        float muteDecibels = -80f;

        if (musicVolume == 0)
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume",
                HelperUtilities.LinearToDecibels(musicVolume));
        }
    }
}
