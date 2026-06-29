using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    #region Header SOUND EFFECT DETAILS

    [Space(10)]
    [Header("SOUND EFFECT DETAILS")]

    #endregion

    #region Tooltip

    [Tooltip("The name for the sound effect")]

    #endregion

    public string soundEffectName;
    
    #region Tooltip

    [Tooltip("The prefab for the sound effect")]

    #endregion

    public GameObject soundPrefab;
    
    #region Tooltip

    [Tooltip("The audio clip for the sound effect")]

    #endregion

    public AudioClip soundEffectClip;
    
    #region Tooltip
    // 音效的最小音高变化。将根据最小值和最大值之间生成随机的音高变化。随机的音高变化使音效听起来更自然
    [Tooltip("The minimum pitch variation for the sound effect. " +
             "A random pitch variation will be generated between the minimum and maximum values." +
             " A random pitch variation makes sound effects sound more natural.")]

    #endregion

    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMin = 0.8f;

    #region Tooltip
    //音效的最大音高变化。将根据最小值和最大值之间生成随机的音高变化。随机的音高变化使音效听起来更自然
    [Tooltip("The maximum pitch variation for the sound effect. " +
             "A random pitch variation will be generated between the minimum and maximum values." +
             " A random pitch variation makes sound effects sound more natural.")]

    #endregion

    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMax  = 1.2f;
    
    #region Tooltip
    //音效的音量
    [Tooltip("The sound effect volume.")]

    #endregion

    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundPrefab), soundPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundEffectClip), soundEffectClip);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(soundEffectPitchRandomVariationMin),
            soundEffectPitchRandomVariationMin, nameof(soundEffectPitchRandomVariationMax),
            soundEffectPitchRandomVariationMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(soundEffectVolume), soundEffectVolume, true);
    }
#endif

    #endregion
}
