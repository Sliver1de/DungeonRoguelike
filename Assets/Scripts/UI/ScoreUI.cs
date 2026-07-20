using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI scoreTextTMP;

    private void Awake()
    {
        scoreTextTMP = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnScoreChanged += StaticEventHandler_OnScoreChange;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnScoreChanged -= StaticEventHandler_OnScoreChange;
    }

    /// <summary>
    /// 处理比分变化事件
    /// </summary>
    /// <param name="scoreChangedArgs"></param>
    private void StaticEventHandler_OnScoreChange(ScoreChangedArgs scoreChangedArgs)
    {
        //更新UI
        scoreTextTMP.text = "得分: " + scoreChangedArgs.score.ToString("###,###0") + "\n倍数: x" +
                            scoreChangedArgs.multiplier;
    }
}
