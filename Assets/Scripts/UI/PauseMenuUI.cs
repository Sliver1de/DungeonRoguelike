using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    #region Tooltip
    //填充音乐音量级别
    [Tooltip("Populate with the music volume level")]

    #endregion

    [SerializeField]
    private TextMeshProUGUI musicLevelText;

    #region Tooltip
    //填充音效音量级别
    [Tooltip("Populate with the sounds volume level")]

    #endregion

    [SerializeField]
    private TextMeshProUGUI soundsLevelText;
    
    [SerializeField] private Button increaseMusicButton;
    [SerializeField] private Button decreaseMusicButton;
    [SerializeField] private Button increaseSoundsButton;
    [SerializeField] private Button decreaseSoundsButton;
    
    private void Start()
    {
        gameObject.SetActive(false);
        
        increaseMusicButton.onClick.AddListener(IncreaseMusicVolume);
        decreaseMusicButton.onClick.AddListener(DecreaseMusicVolume);
        increaseSoundsButton.onClick.AddListener(IncreaseSoundsVolume);
        decreaseSoundsButton.onClick.AddListener(DecreaseSoundsVolume);
    }

    /// <summary>
    /// 初始化UI文本
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitializeUI()
    {
        //等待一帧，以确保先前的音乐和音效级别已设置
        yield return null;
        
        //初始化 UI 文本
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
        
        //初始化UI文本
        StartCoroutine(InitializeUI());
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 退出和加载菜单 —— 从暂停UI菜单进入
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    public void IncreaseSoundsVolume()
    {
        SoundEffectManager.Instance.IncreaSoundsVolume();
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }

    public void DecreaseSoundsVolume()
    {
        SoundEffectManager.Instance.DecreaseSoundsVolume();
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicLevelText), musicLevelText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsLevelText), soundsLevelText);
    }
#endif

    #endregion
}
