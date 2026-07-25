using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Keep this namespace in for when 2D lights become non experimental 保留此命名空间，以备 2D 灯光功能不再是实验性功能时使用
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class LightFlicker : MonoBehaviour
{
    private Light2D light2D;
    [SerializeField] private float lightIntensityMin;
    [SerializeField] private float lightIntensityMax;
    [SerializeField] private float lightFlickerTimeMin;
    [SerializeField] private float lightFlickerTimeMax;
    private float lightFlickerTimer;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    private void Start()
    {
        lightFlickerTimer = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax);
    }

    private void Update()
    {
        if (light2D == null) return;
        
        lightFlickerTimer -= Time.deltaTime;

        if (lightFlickerTimer < 0f)
        {
            lightFlickerTimer = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax);

            RandomiseLightIntensity();
        }
    }

    private void RandomiseLightIntensity()
    {
        light2D.intensity = Random.Range(lightIntensityMin, lightIntensityMax);
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(lightIntensityMin), lightIntensityMin,
            nameof(lightIntensityMax), lightIntensityMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(lightFlickerTimeMin), lightFlickerTimeMin,
            nameof(lightFlickerTimeMax), lightFlickerTimeMax, false);
    }
#endif

    #endregion
}
