using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
        private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// 根据传入的 AmmoHitEffectSO 设置受击效果
    /// </summary>
    /// <param name="aimHitEffect"></param>
    public void SetHitEffect(AmmoHitEffectSO ammoHitEffect)
    {
        //设置受击效果的颜色渐变
        SetHitEffectColorGradient(ammoHitEffect.colorGradient);
        
        //设置受击效果粒子系统的起始值
        SetHitEffectParticleStartingValues(ammoHitEffect.duration, ammoHitEffect.startParticleSize,
            ammoHitEffect.startParticleSpeed, ammoHitEffect.startLifetime, ammoHitEffect.effectGravity,
            ammoHitEffect.maxParticleNumber);
        
        //设置受击效果粒子系统的粒子爆发数量
        SetHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstParticleNumber);
        
        //设置射击效果粒子的精灵
        SetHitEffectParticleSprite(ammoHitEffect.sprite);
        
        //设置射击效果粒子的最小和最大生命周期速度
        SetHitEffectVelocityOverLifeTime(ammoHitEffect.velocityOverLifetimeMin, ammoHitEffect.velocityOverLifetimeMax);
    }

    /// <summary>
    /// 设置射击效果颜色渐变
    /// </summary>
    /// <param name="gradient"></param>
    private void SetHitEffectColorGradient(Gradient gradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
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
    private void SetHitEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed,
        float startLifetime, float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;
        
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
    private void SetHitEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticleSystem.emission;
        
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
    private void SetHitEffectParticleSprite(Sprite sprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule =
            ammoHitEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    /// <summary>
    /// 设置射击特效生命周期内的速度
    /// </summary>
    /// <param name="minVelocity"></param>
    /// <param name="maxVelocity"></param>
    private void SetHitEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule =
            ammoHitEffectParticleSystem.velocityOverLifetime;
        
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
