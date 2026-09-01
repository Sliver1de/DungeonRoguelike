using UnityEngine;

public class DisplayHighScoreUI : MonoBehaviour
{
    #region Header OBJECT REFERENCE

    [Space(10)]
    [Header("OBJECT REFERENCE")]

    #endregion

    #region Tooltip

    [Tooltip("Populate with the child Content gameobject Transform component")]

    #endregion
    
    [SerializeField] private Transform contentAnchorTransform;

    private void Start()
    {
        DisplayScores();
    }

    /// <summary>
    /// 显示分数
    /// </summary>
    private void DisplayScores()
    {
        HighScores highScores = HighScoreManager.Instance.GetHighScores();
        GameObject scoreGameObject;

        int rank = 0;
        foreach (Score score in highScores.scoreList)
        {
            rank++;
            
            //实例化分数游戏对象
            scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);
            
            ScorePrefab scorePrefab = scoreGameObject.GetComponent<ScorePrefab>();
            
            //填充
            scorePrefab.rankTMP.text = rank.ToString();
            scorePrefab.nameTMP.text = score.playerName;
            scorePrefab.levelTMP.text = score.levelDescription;
            scorePrefab.scoreTMP.text = score.playerScore.ToString("###,###0");
        }
        
        //插入空行
        //实例化分数游戏对象
        scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);
    }
}
