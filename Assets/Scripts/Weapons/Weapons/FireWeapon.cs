using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFireEvent))]

[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float firePreChargeTimer = 0f;
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponFireEvent weaponFireEvent;

    private void Awake()
    {
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponFireEvent = GetComponent<WeaponFireEvent>();
    }

    private void OnEnable()
    {
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        //减少冷却时间计时器
        fireRateCoolDownTimer -= Time.deltaTime;
        //Debug.Log(firePreChargeTimer);
    }

    /// <summary>
    /// 处理开火武器事件
    /// </summary>
    /// <param name="fireWeaponEvent"></param>
    /// <param name="fireWeaponEventArgs"></param>
    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

    /// <summary>
    /// 开火逻辑
    /// </summary>
    /// <param name="fireWeaponEventArgs"></param>
    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        //处理武器预充能计时器
        WeaponPreCharge(fireWeaponEventArgs);
        
        //开火
        if (fireWeaponEventArgs.fire)
        {
            //测试武器是否准备好开火
            if (IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle,
                    fireWeaponEventArgs.weaponAimDirectionVector);

                ResetCoolDownTimer();

                ResetPreChargeTimer();
            }
        }
    }

    /// <summary>
    /// 处理武器预充能
    /// </summary>
    /// <param name="fireWeaponEventArgs"></param>
    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        //武器预充能
        if (fireWeaponEventArgs.firePreviousFrame)
        {
            //如果上一帧按住开火按钮，则减少预充能计时器
            firePreChargeTimer -= Time.deltaTime;
        }
        else
        {
            //否则重置预充能计时器
            ResetPreChargeTimer();
        }
    }

    /// <summary>
    /// 如果武器做好开火准备返回true，否则返回false
    /// </summary>
    /// <returns></returns>
    private bool IsWeaponReadyToFire()
    {
        //如果没有弹药且武器没有无限弹药，则返回 false
        if (activeWeapon.GetCurrentWeapon().weaponRemainingAmmo <= 0 &&
            !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo)
        {
            return false;
        }
        
        //如果武器正在换弹，则返回 false
        if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
        {
            return false;
        }
        
        //如果武器不处于预充能状态或正在冷却，则返回 false
        if (firePreChargeTimer > 0f || fireRateCoolDownTimer > 0f) 
        {
            return false;
        }
        
        //如果弹夹中没有弹药且武器没有无限弹夹容量，则返回 false
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity &&
            activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo <= 0)
        {
            //触发武器装弹事件
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);
            
            return false;
        }
        
        //武器已准备好开火 —— 返回true
        return true;
    }

    /// <summary>
    /// 使用对象池中的弹药游戏对象和组件来设置弹药
    /// </summary>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAimAngle"></param>
    /// <param name="weaponAimDirectionVector"></param>
    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            //开火弹药协程
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }

    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle,
        Vector3 weaponAimDirectionVector)
    {
        int ammoCounter = 0;
        
        //获取每次射击的随机弹药消耗量
        int ammoPershot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax);
        
        //获取弹药之间的随机间隔
        float ammoSpawnInterval;

        if (ammoPershot > 1)
        {
            ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
        }
        else
        {
            ammoSpawnInterval = 0f;
        }
        
        //根据发射弹药数量进行循环
        while (ammoCounter < ammoPershot) 
        {
            ammoCounter++;
            
            //从数组中获取弹药预制体
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];
            
            //获取随机速度值
            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);
            
            //获取带有 IFireable 组件的游戏对象
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(),
                Quaternion.identity);
            
            //初始化弹药
            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);
            
            //等待每次射击的弹药时间间隔
            yield return new WaitForSeconds(ammoSpawnInterval);
        }
            
        //如果没有无限弹夹容量，则减少弹夹中的弹药数量
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
        {
            activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
            activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
        }
            
        //调用武器开火事件
        weaponFireEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
        
        //显示武器射击特效
        WeaponShootEffect(aimAngle);
        
        //武器开火音效
        WeaponSoundEffect();
        
        //Debug.Log("11111");
    }

    /// <summary>
    /// 重置冷却时间
    /// </summary>
    private void ResetCoolDownTimer()
    {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    /// <summary>
    /// 重置预充能计时器
    /// </summary>
    private void ResetPreChargeTimer()
    {
        firePreChargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }

    /// <summary>
    /// 显示武器射击效果
    /// </summary>
    /// <param name="aimAngle"></param>
    private void WeaponShootEffect(float aimAngle)
    {
        //处理是否存在射击特效与预制体
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect != null &&
            activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab != null)
        {
            //从对象池中获取带有粒子系统组件的武器射击效果游戏对象
            WeaponShootEffect weaponShootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent(
                activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab,
                activeWeapon.GetShootEffectPosition(), Quaternion.identity);
            
            //设置射击特效
            weaponShootEffect.SetShootEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect, aimAngle);
            
            //设置游戏对象为激活状态（粒子系统完成后会自动禁用该游戏对象）
            weaponShootEffect.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 播放武器射击音效
    /// </summary>
    private void WeaponSoundEffect()
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails
                .weaponFiringSoundEffect);
        }
    }
}
