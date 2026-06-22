using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    #region Tooltip

    [Tooltip("Populate with child TrailRenderer component")]

    #endregion

    [SerializeField]
    private TrailRenderer trailRenderer;

    private float ammoRange = 0f;   //每种子弹射程
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement;
    private bool isColliding = false;

    private void Awake()
    {
        //缓存精灵渲染器
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //弹药充能效果
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!isAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        //如果弹药的移动已被重写（例如该弹药是弹药模式的一部分），则不要移动弹药
        if (!overrideAmmoMovement)
        {
            //计算移动弹药的距离向量
            Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;
            
            transform.position += distanceVector;
        
            //达到最大范围后禁用
            ammoRange -= distanceVector.magnitude;

            if (ammoRange < 0f)
            {
                if (ammoDetails.isPlayerAmmo)
                {
                    //无倍数
                    StaticEventHandler.CallMultiplierEvent(false);
                }

                DisableAmmo();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果已经与某物碰撞则返回
        if (isColliding) return;
        
        //对碰撞对象造成伤害
        DealDamage(collision);
        
        //显示子弹击中特效
        AmmoHitEffect();
        
        DisableAmmo();
    }

    private void DealDamage(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        bool enemyHit = false;

        if (health != null)
        {
            //设置 isColliding 以防止弹药多次造成伤害
            isColliding = true;
            
            health.TakeDamage(ammoDetails.ammoDamage);
            
            //敌人被击中
            if (health.enemy != null)
            {
                enemyHit = true;
            }
        }
        
        //如果玩家有弹药，则更新倍率
        if (ammoDetails.isPlayerAmmo)
        {
            if (enemyHit)
            {
                //乘倍数（累乘）
                StaticEventHandler.CallMultiplierEvent(true);
            }
            else
            {
                //不累乘
                StaticEventHandler.CallMultiplierEvent(false);
            }
        }
    }

    /// <summary>
    /// 初始化正在射击的弹药 —— 使用 ammoDetails、aimAngle、weaponAngle、weaponAimDirectionVector。
    /// 如果该弹药是模式的一部分，弹药的移动可以通过将 overrideAmmoMovement 设置为 true 来覆盖。”
    /// </summary>
    /// <param name="ammoDetails"></param>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAimAngle"></param>
    /// <param name="ammoSpeed"></param>
    /// <param name="weaponAimDirectionVector"></param>
    /// <param name="overrideAmmoMovement"></param>
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed,
        Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo

        this.ammoDetails = ammoDetails;
        
        //Initialise isColliding
        isColliding = false;
        
        //Set fire Direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);
        
        //Set ammo sprite
        spriteRenderer.sprite = ammoDetails.ammoSprite;
        
        //set initial ammo material depending on whether there is an ammo charge period 根据是否有弹药充能周期设置初始弹药材质
        if (ammoDetails.ammoChargeTime > 0f)
        {
            //Set ammo charge timer
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            isAmmoMaterialSet = false;
        }
        else
        {
            ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }
        
        //Set ammo range
        ammoRange = ammoDetails.ammoRange;
        
        //Set ammoSpeed
        this.ammoSpeed = ammoSpeed;
        
        //Override ammo movement
        this.overrideAmmoMovement = overrideAmmoMovement;
        
        //Active ammo gameobject
        gameObject.SetActive(true);

        #endregion

        #region Trail

        if (ammoDetails.isAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;   //拖尾粒子效果
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }

        #endregion
    }

    /// <summary>
    /// 根据输入角度和方向，调整随机散布后的弹药射击方向和角度
    /// </summary>
    /// <param name="ammoDetails"></param>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAimAngle"></param>
    /// <param name="weaponAimDirectionVector"></param>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle,
        Vector3 weaponAimDirectionVector)
    {
        //计算最小值和最大值之间的随机散布角度
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);
        
        //获取随机散布，切换为 1 或 -1
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }
        
        //通过随机散布调整弹药射击角度
        fireDirectionAngle += spreadToggle * randomSpread;
        
        //设置子弹旋转
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);
        
        //设置开火方向
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    /// <summary>
    /// 禁用弹药 - 从而将其返回到对象池
    /// </summary>
    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示弹药命中效果
    /// </summary>
    private void AmmoHitEffect()
    {
        //处理是否已指定命中效果
        if (ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
        {
            //从对象池中获取弹药命中效果游戏对象（包含粒子系统组件）
            AmmoHitEffect ammoHitEffect =
                (AmmoHitEffect)PoolManager.Instance.ReuseComponent(ammoDetails.ammoHitEffect.ammoHitEffectPrefab,
                    transform.position, Quaternion.identity);
            
            //设置命中效果
            ammoHitEffect.SetHitEffect(ammoDetails.ammoHitEffect);
            
            //设置游戏对象为激活状态（粒子系统完成后会自动禁用该游戏对象
            ammoHitEffect.gameObject.SetActive(true);
        }
    }

    public void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this,nameof(trailRenderer), trailRenderer);
    }
#endif

    #endregion
}
