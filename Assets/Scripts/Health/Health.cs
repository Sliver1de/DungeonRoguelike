using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    #region Header References

    [Space(10)]
    [Header("References")]

    #endregion

    #region Tooltip
    //填充 HealthBar 组件到 HealthBar 游戏对象
    [Tooltip("Populate with the HealthBar component on the HealthBar gameobject")]

    #endregion

    [SerializeField]
    private HealthBar healthBar;
    
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    private Player player;
    private Coroutine immunityCoroutine;
    private bool isImmuneAfterHit = false;
    private float immunityTime = 0f;
    private SpriteRenderer spriteRenderer = null;
    private const float spriteFlashInterval = 0.2f;
    private WaitForSeconds waitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);
    
    [HideInInspector] public bool isDamageable = true;
    [HideInInspector] public Enemy enemy;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        //触发生命值事件以更新 UI
        CallHealthEvent(0);
        
        //尝试加载敌人/玩家组件
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
        
        //获取玩家/敌人受击免疫详情
        if (player != null)
        {
            if (player.playerDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = player.playerDetails.hitImmunityTime;
                spriteRenderer = player.spriteRenderer;
            }
        }
        else if (enemy != null)
        {
            if (enemy.enemyDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = enemy.enemyDetails.hitImmunityTime;
                spriteRenderer = enemy.spriteRendererArray[0];
            }
        }
        
        //如果需要，启用血条。
        if (enemy != null && enemy.enemyDetails.isHealthBarDisplayed == true && healthBar != null)
        {
            healthBar.EnableHealthBar();
        }
        else if (healthBar != null)
        {
            healthBar.DisableHealthBar();
        }
    }

    /// <summary>
    /// 受到伤害时调用的公共方法
    /// </summary>
    /// <param name="damageAmount"></param>
    public void TakeDamage(int damageAmount)
    {
        bool isRolling = false;

        if (player != null)
        {
            isRolling = player.playerControl.isPlayerRolling;
        }

        if (isDamageable && !isRolling)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);

            PostHitImmunity();
            
            //将血条设置为剩余生命值的百分比
            if (healthBar != null)
            {
                healthBar.SetHealthBarValue((float)currentHealth / (float)startingHealth);
            }
        }

        // if (isDamageable && isRolling)
        // {
        //     Debug.Log("Dodged Bullet By Rolling");
        // }
        //
        // if (!isDamageable && !isRolling)
        // {
        //     Debug.Log("Avoid Damage Due To Immunity");
        // }
    }

    /// <summary>
    /// 指示一次击中并给予一些击后免疫。
    /// </summary>
    private void PostHitImmunity()
    {
        //检查游戏对象是否处于激活状态，如果不是，则返回
        if (gameObject.activeSelf == false)
        {
            return;
        }
        
        //如果有击中后的免疫期，那么
        if (isImmuneAfterHit)
        {
            if (immunityCoroutine != null)
            {
                StopCoroutine(immunityCoroutine);
            }
            
            //闪烁红色并给与一段免疫期
            immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immunityTime, spriteRenderer));
        }
    }

    /// <summary>
    /// 击中并给予一定的后击免疫时间的协程
    /// </summary>
    /// <param name="immunityTime"></param>
    /// <param name="spriteRenderer"></param>
    /// <returns></returns>
    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);

        isDamageable = false;

        while (iterations > 0)
        {
            spriteRenderer.color = Color.red;

            yield return waitForSecondsSpriteFlashInterval;
            
            spriteRenderer.color=Color.white;
            
            yield return waitForSecondsSpriteFlashInterval;

            iterations--;

            yield return null;
        }

        isDamageable = true;
    }

    private void CallHealthEvent(int damageAmount)
    {
        //触发血量事件
        healthEvent.CallHealthChangedEvent(((float)currentHealth / (float)startingHealth), currentHealth, damageAmount);
    }


    /// <summary>
    /// 设置初始血条
    /// </summary>
    /// <param name="startingHealth"></param>
    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    /// <summary>
    /// 获取初始血量
    /// </summary>
    /// <returns></returns>
    public int GetStartingHealth()
    {
        return startingHealth;
    }

    /// <summary>
    /// 按指定百分比增加生命值
    /// </summary>
    /// <param name="healthPercent"></param>
    public void AddHealth(int healthPercent)
    {
        int healthIncrease = Mathf.RoundToInt((startingHealth * healthPercent) / 100f);

        int totalHealth = currentHealth + healthIncrease;

        if (totalHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }
        else
        {
            currentHealth = totalHealth;
        }
        
        CallHealthEvent(0);
    }
}
