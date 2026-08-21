using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES

    [Space(10)]
    [Header("OBJECT REFERENCES")]

    #endregion

    #region Tooltip
    //填充为“进入地牢”游戏按钮对象
    [Tooltip("Populate with the enter the dungeon play button gameobject")]

    #endregion

    [SerializeField]
    private GameObject playButton;

    #region Tooltip
    //填充为“退出游戏”游戏按钮对象
    [Tooltip("Populate with the quit button gameobject")]

    #endregion

    [SerializeField]
    private GameObject quitButton;

    #region Tooltip
    //填充为“高分榜”按钮游戏对象
    [Tooltip("Populate with the high scores button gameobject")]

    #endregion

    [SerializeField]
    private GameObject highScoreButton;

    #region Tooltip
    //填充为“游戏说明”游戏按钮对象
    [Tooltip("Populate with the instructions button gameobject")]

    #endregion

    [SerializeField]
    private GameObject instructionsButton;

    #region Tooltip
    //填充为“返回主菜单”按钮游戏对象
    [Tooltip("Populate with the return to main menu button gameobject")]

    #endregion
    
    [SerializeField] private GameObject returnToMainMenuButton;

    private bool isInstructionSceneLoaded = false;
    private bool isHighScoresSceneLoaded = false;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);
        
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
        
        returnToMainMenuButton.SetActive(false);
    }

    /// <summary>
    /// 从“开始游戏”/“进入地牢”按钮调用
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    /// <summary>
    /// 从“高分”按钮调用
    /// </summary>
    public void LoadHighScores()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        highScoreButton.SetActive(false);
        instructionsButton.SetActive(false);
        isHighScoresSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");
        
        returnToMainMenuButton.SetActive(true);
        
        //以附加模式加载高分场景
        SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// 从“返回主菜单”按钮调用
    /// </summary>
    public void LoadCharacterSelector()
    {
        returnToMainMenuButton.SetActive(false);

        if (isHighScoresSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("HighScoreScene");
            isHighScoresSceneLoaded = false;
        }
        else if (isInstructionSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("InstructionsScene");
            isInstructionSceneLoaded = false;
        }

        playButton.SetActive(true);
        quitButton.SetActive(true);
        highScoreButton.SetActive(true);
        instructionsButton.SetActive(true);
        
        //以叠加方式加载角色选择场景
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// 从说明按钮调用
    /// </summary>
    public void LoadInstructions()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        highScoreButton.SetActive(false);
        instructionsButton.SetActive(false);
        isInstructionSceneLoaded = true;
        
        SceneManager.UnloadSceneAsync("CharacterSelectorScene");
        
        returnToMainMenuButton.SetActive(true);
        
        //以附加模式加载说明场景
        SceneManager.LoadScene("InstructionsScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// 退出游戏——此方法从检查器中设置的 onClick 事件调用
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playButton), playButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(quitButton), quitButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(highScoreButton), highScoreButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(instructionsButton), instructionsButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
    }
#endif

    #endregion
}
