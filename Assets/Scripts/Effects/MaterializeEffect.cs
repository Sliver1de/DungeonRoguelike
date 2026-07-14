using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    /// <summary>
    /// 显现效果协程——用于显现特殊效果
    /// </summary>
    /// <param name="materializedShader"></param>
    /// <param name="materializedColor"></param>
    /// <param name="materializeTime"></param>
    /// <param name="spriteRendererArray"></param>
    /// <param name="normalMaterial"></param>
    /// <returns></returns>
    public IEnumerator MaterializeRoutine(Shader materializeShader, Color materializeColor, float materializeTime,
        SpriteRenderer[] spriteRendererArray, Material normalMaterial)
    {
        Material materializeMaterial = new Material(materializeShader);

        materializeMaterial.SetColor("_EmissionColor", materializeColor);
        
        //在精灵渲染器中设置显现材质
        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = materializeMaterial;
        }

        float dissolveAmount = 0f;  //溶解程度
        
        //显现敌人
        while (dissolveAmount < 1f)
        {
            dissolveAmount += Time.deltaTime / materializeTime;

            materializeMaterial.SetFloat("_DissolveAmount", dissolveAmount);

            yield return null;
        }
        
        //在精灵渲染器中设置标准材质
        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = normalMaterial;
        }
    }
}
