using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(MaterializeEffect))]
public class ChestItem : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private TextMeshPro textTMP;
    private MaterializeEffect materializeEffect;
    [HideInInspector] public bool isItemMaterialized = false;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        textTMP = GetComponentInChildren<TextMeshPro>();
        materializeEffect = GetComponent<MaterializeEffect>();
    }

    /// <summary>
    /// 初始化宝箱物品
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="text"></param>
    /// <param name="spawnPosition"></param>
    /// <param name="materializeColor"></param>
    public void Initialize(Sprite sprite, string text, Vector3 spawnPosition, Color materializeColor)
    {
        spriteRenderer.sprite = sprite;
        transform.position = spawnPosition;

        StartCoroutine(MaterializeItem(materializeColor, text));
    }

    /// <summary>
    /// 使宝箱物品实体化
    /// </summary>
    /// <param name="materialColor"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerator MaterializeItem(Color materialColor, string text)
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader,
            materialColor, 1f, spriteRendererArray, GameResources.Instance.litMaterial));

        isItemMaterialized = true;
        
        textTMP.text = text;
    }
}
