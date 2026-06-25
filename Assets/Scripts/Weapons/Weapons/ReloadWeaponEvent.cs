using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ReloadWeaponEvent : MonoBehaviour
{
    public event Action<ReloadWeaponEvent, ReloadWeaponEventArgs> OnReloadWeapon;

    /// <summary>
    /// 指定武器进行弹夹重装。如果总弹药也需要增加，则指定 topUpAmmoPercent（补充弹药百分比
    /// </summary>
    /// <param name="weapon"></param>
    /// <param name="topUpAmmoPercent"></param>
    public void CallReloadWeaponEvent(Weapon weapon, int topUpAmmoPercent)
    {
        OnReloadWeapon?.Invoke(this,new ReloadWeaponEventArgs()
        {
            weapon = weapon, 
            topUpAmmoPercent = topUpAmmoPercent
        });
    }
}

public class ReloadWeaponEventArgs : EventArgs
{
    public Weapon weapon;
    public int topUpAmmoPercent;
}