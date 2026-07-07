using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENT
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(WeaponFireEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
#endregion

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    private HealthEvent healthEvent;
    private Health health;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    private FireWeapon fireWeapon;
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private EnemyMovementAI enemyMovementAI;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    private MaterializeEffect materializeEffect;
    private CircleCollider2D circleCollider2D;
    private PolygonCollider2D polygonCollider2D;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        fireWeapon = GetComponent<FireWeapon>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        materializeEffect = GetComponent<MaterializeEffect>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>(); 
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }

    /// <summary>
    /// 处理生命值损失事件
    /// </summary>
    /// <param name="healthEvent"></param>
    /// <param name="healthEventArgs"></param>
    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0)
        {
            EnemyDestroyed();
        }
    }

    /// <summary>
    /// 敌人已被摧毁
    /// </summary>
    private void EnemyDestroyed()
    {
        DestroyedEvent destroyedEvent = GetComponent<DestroyedEvent>();
        destroyedEvent.CallDestroyedEvent(false, health.GetStartingHealth());
    }

    /// <summary>
    /// 初始化敌人
    /// </summary>
    /// <param name="enemyDetails"></param>
    /// <param name="enemySpawnNumber"></param>
    /// <param name="dungeonLevel"></param>
    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyDetails = enemyDetails;
        
        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        SetEnemyStartingHealth(dungeonLevel);

        SetEnemyStartingWeapon();

        SetEnemyAnimationSpeed();
        
        //Materialise enemy
        StartCoroutine(MaterializeEnemy());
    }

    /// <summary>
    /// 设置敌人移动更新的帧编号
    /// </summary>
    /// <param name="enemySpawnNumber"></param>
    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        //设置敌人处理其更新的帧编号
        enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber % Settings.targetFrameRateToSpreadPathfindingOver);
    }

    /// <summary>
    /// 设置敌人的初始生命值
    /// </summary>
    private void SetEnemyStartingHealth(DungeonLevelSO dungeonLevel)
    {
        //获取该地牢等级的敌人生命值
        foreach (EnemyHealthDetails enemyHealthDetails in enemyDetails.enemyHealthDetailsArray)
        {
            if (enemyHealthDetails.dungeonLevel == dungeonLevel)
            {
                health.SetStartingHealth(enemyHealthDetails.enemyHealthAmount);
                return;
            }
        }
        health.SetStartingHealth(Settings.defaultEnemyHealth);
    }

    /// <summary>
    /// 根据武器详情SO设置敌人的初始武器
    /// </summary>
    private void SetEnemyStartingWeapon()
    {
        //处理敌人是否有武器
        if (enemyDetails.enemyWeapon != null)
        {
            Weapon weapon = new Weapon()
            {
                weaponDetails = enemyDetails.enemyWeapon,
                weaponReloadTimer = 0f,
                weaponClipRemainingAmmo = enemyDetails.enemyWeapon.weaponClipAmmoCapacity,
                weaponRemainingAmmo = enemyDetails.enemyWeapon.weaponAmmoCapacity,
                isWeaponReloading = false
            };
            
            //设置敌人武器
            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        }
    }

    /// <summary>
    /// 将动画师的速度设置为与移动速度匹配
    /// </summary>
    private void SetEnemyAnimationSpeed()
    {
        animator.speed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimations;
    }

    private IEnumerator MaterializeEnemy()
    {
        //禁用碰撞体、移动 AI 和武器 AI
        EnemyEnable(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader,
            enemyDetails.enemyMaterializeColor, enemyDetails.enemyMaterializeTime, spriteRendererArray,
            enemyDetails.enemyStandardMaterial));
        
        EnemyEnable(true);
    }

    private void EnemyEnable(bool isEnable)
    {
        //启用/禁用碰撞体
        circleCollider2D.enabled = isEnable;
        polygonCollider2D.enabled = isEnable;
        
        //启用/禁用移动 AI
        enemyMovementAI.enabled = isEnable;
        
        //启用/禁用开火武器
        fireWeapon.enabled = isEnable;
    }
}
