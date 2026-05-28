using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    public static Camera mainCamera;
    
    /// <summary>
    /// 获取鼠标世界位置
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 mouseScreenPosition = Input.mousePosition;
        
        //将鼠标位置固定到屏幕尺寸
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        
        worldPosition.z = 0f;

        return worldPosition;
    }

    /// <summary>
    /// 获取摄像机视口的下限和上限
    /// </summary>
    /// <param name="cameraWorldPositionLowerBounds"></param>
    /// <param name="cameraWorldPositionUpperBounds"></param>
    /// <param name="camera"></param>
    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLowerBounds,
        out Vector2Int cameraWorldPositionUpperBounds, Camera camera)
    {
        Vector3 worldPositionViewPortBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewPortTopRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        cameraWorldPositionLowerBounds = new Vector2Int((int)worldPositionViewPortBottomLeft.x,
            (int)worldPositionViewPortBottomLeft.y);
        cameraWorldPositionUpperBounds =
            new Vector2Int((int)worldPositionViewPortTopRight.x, (int)worldPositionViewPortTopRight.y);
    }

    /// <summary>
    /// 从方向向量获取角度（以度为单位）
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float degrees = radians * Mathf.Rad2Deg;
        
        return degrees;
    }

    /// <summary>
    /// 从角度（度）获取方向向量
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    /// <summary>
    /// 从传入的角度（angleDegrees）获取 AimDirection 枚举值
    /// </summary>
    /// <param name="angleDegrees"></param>
    /// <returns></returns>
    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection;
        
        //设置玩家方向
        if (angleDegrees >= 22f && angleDegrees <= 67f)
        {
            aimDirection = AimDirection.UpRight;
        }
        else if (angleDegrees > 67f && angleDegrees <= 112f)
        {
            aimDirection = AimDirection.Up;
        }
        else if (angleDegrees > 112f && angleDegrees <= 158f)
        {
            aimDirection = AimDirection.UpLeft;
        }
        else if ((angleDegrees > 158f && angleDegrees <= 180f) || (angleDegrees > -180f && angleDegrees <= -135f))
        {
            aimDirection = AimDirection.Left;
        }
        else if (angleDegrees > -135f && angleDegrees <= -45f)
        {
            aimDirection = AimDirection.Down;
        }
        else if ((angleDegrees > -45f && angleDegrees <= 0f) || (angleDegrees > 0 && angleDegrees < 22f))
        {
            aimDirection = AimDirection.Right;
        }
        else
        {
            aimDirection = AimDirection.Right;
        }
        return aimDirection;
    }

    /// <summary>
    /// 将线性音量尺度转换为分贝（dB）
    /// </summary>
    /// <param name="linear"></param>
    /// <returns></returns>
    public static float LinearToDecibels(int linear)
    {
        float linearScaleRange = 20f;
        
        //将线性音量尺度转换为对数分贝（dB）尺度
        //dB=20×log.10(linear_value)
        return Mathf.Log10((float)linear / linearScaleRange) * 20;
    }

    /// <summary>
    /// 空字符串调试检查
    /// </summary>
    /// <param name="thisOnject"></param>
    /// <param name="fileName"></param>
    /// <param name="stringToCheck"></param>
    /// <returns></returns>
    public static bool ValidateCheckEmptyString(Object thisObject, string fileName, string stringToCheck)
    {
        //检测传入的字符串是否为空字符
        if (stringToCheck == "")
        {
            //打印空信息
            Debug.Log(fileName + " is empty and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    /// <summary>
    /// 空值调试检查
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fileName"></param>
    /// <param name="objectToCheck"></param>
    /// <returns></returns>
    public static bool ValidateCheckNullValue(Object thisObject, string fileName, Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fileName + " is null and must contain a value in object" + thisObject.name.ToString());
            return true;
        }

        return false;
    }

    /// <summary>
    /// 列表为空或包含空值检查 - 如果出现错误，返回 true
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fileName"></param>
    /// <param name="enumerableObjectToCheck"></param>
    /// <returns></returns>
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fileName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fileName + " is null in object " + thisObject.name.ToString());
            return true;
        }

        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(fileName + " has null values in object" + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fileName + " has no values in object" + thisObject.name.ToString());
            error = true;
        }
        return error;
    }

    /// <summary>
    /// 正值调试检查，如果允许零值，则将 isZeroAllowed 设置为 true。如果存在错误，则返回 true。”
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fileName"></param>
    /// <param name="valueToCheck"></param>
    /// <param name="isZeroAllowed"></param>
    /// <returns></returns>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fileName, int valueToCheck,
        bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fileName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fileName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }

    public static bool ValidateCheckPositiveValue(Object thisObject, string fileName, float valueToCheck,
        bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fileName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fileName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }

    /// <summary>
    /// 正范围调试检查 —— 如果最小值和最大值范围都可以为零，则将 isZeroAllowed 设置为 true。如果存在错误，则返回 true。
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldNameMinimum"></param>
    /// <param name="valueToCheckMinimum"></param>
    /// <param name="fieldNameMaximum"></param>
    /// <param name="valueToCheckMaximum"></param>
    /// <param name="isZeroAllowed"></param>
    /// <returns></returns>
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float valueToCheckMinimum,
        string fieldNameMaximum, float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to " + fieldNameMaximum + " in object " +
                      thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed))
        {
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed))
        {
            error = true;
        }

        return error;
    }
    
    /// <summary>
    /// 正范围调试检查 —— 如果最小值和最大值范围都可以为零，则将 isZeroAllowed 设置为 true。如果存在错误，则返回 true。
    /// </summary>
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, int valueToCheckMinimum,
        string fieldNameMaximum, float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to " + fieldNameMaximum + " in object " +
                      thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed))
        {
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed))
        {
            error = true;
        }

        return error;
    }

    /// <summary>
    /// 获取距离玩家最近的生成位置
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(10000f, 10000f, 0f);
        
        //遍历房间生成位置
        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            //将生成网格位置转换为世界位置
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if (Vector3.Distance(spawnPositionWorld, playerPosition) <
                Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                nearestSpawnPosition = spawnPositionWorld;
            }
        }
        return nearestSpawnPosition;
    }
}