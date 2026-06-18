using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class StaticEventHandler
{
    //房间改变事件
    public static event Action<RoomChangedEventArgs> OnRoomChanged;

    public static void CallRoomChangedEvent(Room room)
    {
        OnRoomChanged?.Invoke(new RoomChangedEventArgs() { room = room });
    }
    
    //击败房间总敌人事件
    public static event Action<RoomEnemiesDefeatedArgs> OnRoomEnemiesDefeated;

    public static void CallRoomEnemiesDefeatedEvent(Room room)
    {
        OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedArgs() { room = room });
    }
    
    //得分点数事件
    public static event Action<PointsScoredArgs> OnPointsScored;

    public static void CallPointsScoredEvent(int points)
    {
        OnPointsScored?.Invoke(new PointsScoredArgs() { points = points });
    }
    
    //得分改变事件
    public static event Action<ScoreChangedArgs> OnScoreChanged;

    public static void CallScoreChangedEvent(long score, int multiplier)
    {
        OnScoreChanged?.Invoke(new ScoreChangedArgs() { score = score, multiplier = multiplier });
    }
    
    //乘法事件
    public static event Action<MultiplierArgs> OnMultiplier;

    public static void CallMultiplierEvent(bool multiplier)
    {
        OnMultiplier?.Invoke(new MultiplierArgs() { multiplier = multiplier });
    }
}

public class RoomChangedEventArgs : EventArgs
{
    public Room room;
}

public class RoomEnemiesDefeatedArgs : EventArgs
{
    public Room room;
}

public class PointsScoredArgs : EventArgs
{
    public int points;
}

public class ScoreChangedArgs : EventArgs
{
    public long score;
    public int multiplier;
}

public class MultiplierArgs : EventArgs
{
    public bool multiplier;
}