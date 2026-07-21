using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//不要添加 RequireComponent 指令，因为当物品被销毁时，我们会销毁其组件
[DisallowMultipleComponent]
public class DestroyableItem : MonoBehaviour
{
    #region Header HEALTH

    [Header("HEALTH")]

    #endregion

    #region Tooltip
    //这个可破坏物品的初始生命值应设定为多少
    [Tooltip("What the starting health for this destroyable item should be")]

    #endregion

    [SerializeField]
    private int startingHealthAmount = 1;

    #region SOUND EFFECT

    [Header("SOUND EFFECT")]

    #endregion

    #region Tooltip
    //此物品被摧毁时播放的音效
    [Tooltip("The sound effect when this item is destroyed")]

    #endregion

    [SerializeField]
    private SoundEffectSO destroySoundEffect;
    
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private HealthEvent healthEvent;
    private Health health;
    private ReceiveContactDamage receiveContactDamage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        health.SetStartingHealth(startingHealthAmount);
        receiveContactDamage = GetComponent<ReceiveContactDamage>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }

    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0f)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        //摧毁触发碰撞体
        Destroy(boxCollider2D);
        
        //播放音效
        if (destroySoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(destroySoundEffect);
        }
        
        //触发销毁动画
        animator.SetBool(Settings.destroy, true);
        
        //让动画完整播放
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.stateDestroyed))
        {
            yield return null;
        }
        
        //然后销毁除 Sprite Renderer 之外的所有组件，以仅显示动画的最终精灵
        Destroy(animator);
        Destroy(receiveContactDamage);
        Destroy(health);
        Destroy(healthEvent);
        Destroy(this);
    }
}
