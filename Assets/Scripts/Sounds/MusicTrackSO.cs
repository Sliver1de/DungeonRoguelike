using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "Scriptable Objects/Sounds/Music Track")]
public class MusicTrackSO : ScriptableObject
{
    #region Header MUSIC TRACK DETAILS

    [Space(10)]
    [Header("MUSIC TRACK DETAILS")]

    #endregion

    #region Tooltip
    //音乐曲目的音频片段
    [Tooltip("The audio clip for the music track")]

    #endregion

    public string musicName;

    #region Tooltip
    //音乐轨道的音频剪辑
    [Tooltip("The audio clip for the music track")]

    #endregion
    
    public AudioClip musicClip;

    #region Tooltip
    //音乐轨道的音量
    [Tooltip("The volume for the music track")]

    #endregion

    [Range(0, 1)]
    public float musicVolume = 1f;

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(musicName), musicName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicClip), musicClip);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(musicVolume), musicVolume, true);
    }
#endif

    #endregion
}
