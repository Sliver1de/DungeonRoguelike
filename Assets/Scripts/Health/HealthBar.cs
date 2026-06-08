using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Header GameObject Reference

    [Space(10)]
    [Header("GameObject Reference")]

    #endregion

    #region Tooltip

    //用子条填充
    [Tooltip("Populate with the child Bar gameobject")]

    #endregion

    [SerializeField]
    private GameObject healthBar;

    /// <summary>
    /// 启用血条
    /// </summary>
    public void EnableHealthBar()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 禁用血条
    /// </summary>
    public void DisableHealthBar()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 将血条数值设置为介于 0 到 1 之间的生命值百分比
    /// </summary>
    /// <param name="healthPercent"></param>
    public void SetHealthBarValue(float healthPercent)
    {
        healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);
    }
}
