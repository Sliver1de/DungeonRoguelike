public enum Orientation
{
    north,
    east,
    south,
    west,
    none
}

public enum AimDirection
{
    Up,
    UpRight,
    UpLeft,
    Right,
    Left,
    Down
}

public enum ChestSpawnEvent
{
    onRoomEntry,
    onEnemiesDefeated
}

public enum ChestSpawnPosition
{
    atSpawnerPosition,
    atPlayerPosition
}

public enum ChestState
{
    closed,
    healthItem,
    ammoItem,
    weaponItem,
    empty
}

public enum GameState
{
    gameStarted,
    playingLevel,
    engagingEnemies,        //与敌人交战
    bossStage,              //Boss阶段
    engagingBoss,           //与Boss交战
    levelCompleted,         //升级完成
    gameWon,
    gameLost,
    gamePaused,
    dungeonOverviewMap,     //地下城概览地图
    restartGame,
}
