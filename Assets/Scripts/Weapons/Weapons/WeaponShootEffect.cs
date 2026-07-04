using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem shootEffectParticleSystem;

    private void Awake()
    {
        shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// 根据传入的 WeaponShootEffectSO 和 aimAngle 设置射击效果
    /// </summary>
    /// <param name="shootEffect"></param>
    /// <param name="aimAngle"></param>
    public void SetShootEffect(WeaponShootEffectSO shootEffect, float aimAngle)
    {
        //设置射击效果的颜色渐变
        SetShootEffectColorGradient(shootEffect.colorGradient);
        
        //设置射击效果粒子系统的起始值
        SetShootEffectParticleStartingValues(shootEffect.duration, shootEffect.startParticleSize,
            shootEffect.startParticleSpeed, shootEffect.startLifetime, shootEffect.effectGravity,
            shootEffect.maxParticleNumber);
        
        //设置射击效果粒子系统的粒子爆发数量
        SetShootEffectParticleEmission(shootEffect.emissionRate, shootEffect.burstParticleNumber);
        
        //设置发射器旋转角度
        SetEmitterRotation(aimAngle);
        
        //设置射击效果粒子的精灵
        SetShootEffectParticleSprite(shootEffect.sprite);
        
        //设置射击效果粒子的最小和最大生命周期速度
        SetShootEffectVelocityOverLifeTime(shootEffect.velocityOverLifetimeMin, shootEffect.velocityOverLifetimeMax);
    }

    /// <summary>
    /// 设置射击效果颜色渐变
    /// </summary>
    /// <param name="gradient"></param>
    private void SetShootEffectColorGradient(Gradient gradient)
    {
        //设置颜色渐变
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// <summary>
    /// 设置射击特效粒子系统的初始值
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="startParticleSize"></param>
    /// <param name="startParticleSpeed"></param>
    /// <param name="startLifetime"></param>
    /// <param name="effectGravity"></param>
    /// <param name="maxParticles"></param>
    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed,
        float startLifetime, float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = shootEffectParticleSystem.main;
        
        //设置粒子系统持续时间
        mainModule.duration = duration;
        
        //设置粒子初始大小
        mainModule.startSize = startParticleSize;
        
        //设置粒子初始速度
        mainModule.startSpeed = startParticleSpeed;
        
        //设置粒子初始生命周期
        mainModule.startLifetime = startLifetime;
        
        //设置粒子初始重力系数
        mainModule.gravityModifier = effectGravity;
        
        //设置最大粒子数量
        mainModule.maxParticles = maxParticles;
    }

    /// <summary>
    /// 设置射击特效粒子系统的粒子爆发数量
    /// </summary>
    /// <param name="emissionRate"></param>
    /// <param name="burstParticleNumber"></param>
    private void SetShootEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = shootEffectParticleSystem.emission;
        
        //设置粒子爆发数量
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0,burst);
        
        //设置粒子的发射速率
        emissionModule.rateOverTime = emissionRate;
    }

    /// <summary>
    /// 设置射击特效粒子系统的精灵图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetShootEffectParticleSprite(Sprite sprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule =
            shootEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    /// <summary>
    /// 设置发射器的旋转角度以匹配瞄准角度
    /// </summary>
    /// <param name="angle"></param>
    private void SetEmitterRotation(float aimAngle)
    {
        transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
    }

    /// <summary>
    /// 设置射击特效生命周期内的速度
    /// </summary>
    /// <param name="minVelocity"></param>
    /// <param name="maxVelocity"></param>
    private void SetShootEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule =
            shootEffectParticleSystem.velocityOverLifetime;
        
        //定义 最小/最大 X 轴速度
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = minVelocity.x;
        minMaxCurveX.constantMax = maxVelocity.x;
        velocityOverLifetimeModule.x = minMaxCurveX;
        
        //定义 最小/最大 Y 轴速度
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = minVelocity.y;
        minMaxCurveY.constantMax = maxVelocity.y;
        velocityOverLifetimeModule.y = minMaxCurveY;
        
        //定义 最小/最大 Z 轴速度
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = minVelocity.z;
        minMaxCurveZ.constantMax = maxVelocity.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }
}
