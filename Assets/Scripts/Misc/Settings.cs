using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region UNITS

    public const float pixelPerUnit = 16f;   //每单位像素数
    public const float tileSizePixels = 16f;    //瓦片大小

    #endregion
    
    #region Dungeon Build Settings

    public const int maxDungeonRebulidAttemptsForRoomGraph = 1000;  //用于设置重建房间图时的最大尝试次数
    public const int maxDungeonBuildAttempts = 10;          //用于设置生成地下城时的最大尝试次数

    #endregion
    
    #region Room Settings

    //Max number of child corridors leading from a room. - maximum should be 3 although this is not recommended since it
    //can cause the dungeon building to fail since the rooms are more likely to not fit together
    //从一个房间通向其他房间的子走廊的最大数量。 - 最大应为 3 个，但不建议这样做，因为这可能导致地牢建造失败，因为房间更有可能无法组合在一起
    public const int maxChildCorridors = 3;
    //Time to fade in the room  进入房间的淡入时间
    public const float fadeInTime = 0.5f;
    public const float doorUnlockDelay = 1f;

    #endregion
    
    #region Animator Parameters
    //Animator parameters - player
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int flipUp = Animator.StringToHash("flipUp");
    public static int flipDown = Animator.StringToHash("flipDown");
    public static int flipLeft = Animator.StringToHash("flipLeft");
    public static int flipRight = Animator.StringToHash("flipRight");
    public static int use = Animator.StringToHash("use");

    public static float baseSpeedForPlayerAnimations = 8f;
    
    //Animator parameters - Enemy
    public static float baseSpeedForEnemyAnimations = 3f;

    //Animator parameters - Door
    public static int open = Animator.StringToHash("open");
    
    //Animator parameters - DamageableDecoration
    public static int destroy = Animator.StringToHash("destroy");
    public static String stateDestroyed = "Destroyed";

    #endregion
    
    #region GAMEOBJECT TAGS
    
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";

    #endregion

    #region AUDIO

    public const float musicFadeOutTime = 0.5f;     //Default Music Fade Out Transition 默认音乐淡出过渡
    public const float musicFadeInTime = 0.5f;      //Default Music Fade in Transition  默认音乐淡入过渡

    #endregion

    #region FIRING CONTROL
    //if the target distance is less than this then the aim angle will be used (calculated from player),
    //else the weapon aim angle will be used (calculate from the weapon shoot position).
    //如果目标距离小于此值，则将使用瞄准角度（从玩家位置计算）；否则，将使用武器瞄准角度（从武器射击位置计算）
    public const float useAimAngleDistance = 3.5f;

    #endregion

    #region ASTAR PATHFINDING PARSMETERS

    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const int targetFrameRateToSpreadPathfindingOver = 60;
    public const float playerMoveDistaanceToRebuildPath = 3f;
    public const float enemyPathbuildCooldown = 2f;

    #endregion

    #region ENEMY PARAMETERS

    public const int defaultEnemyHealth = 20;

    #endregion

    #region UI PARAMETERS

    public const float uiHeartSpacing = 16f;
    public const float uiAmmoIconSpacing = 4f;

    #endregion

    #region CONTACT DAMAGE PARAMETER

    public const float contactDamageCollisionResetDelay = 0.5f;

    #endregion

    #region HIGHSCORES

    public const int numberOfHighScoresToSave = 100;

    #endregion
}
