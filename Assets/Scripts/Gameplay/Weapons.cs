using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapons
{
    private GameObject Arm;
    public Weapons(GameObject arm)
    {
        Arm = arm;
    }

    [Client]
    public void CmdArm(bool enabled, bool isLocalPlayer)
    {
        localEnableArm(enabled);
        RpcArm(enabled, isLocalPlayer);
    }
    [ClientRpc]
    public void RpcArm(bool enabled, bool isLocalPlayer)
    {
        if (isLocalPlayer) { return; }
        localEnableArm(enabled);
    }


    public void localEnableArm(bool enabled)
    {
        Arm.SetActive(enabled);
    }
    
}

public class DisableWeapons : MonoBehaviour
{
    public IEnumerator disableArm(float time, bool isServer, Weapons weapons, bool isLocalPlayer)
    {
        yield return new WaitForSeconds(time);

        weapons.localEnableArm(false);
        if (isServer)
        {
            weapons.RpcArm(false, isLocalPlayer);
        }
        else
        {
            weapons.CmdArm(false, isLocalPlayer);
        }
    }
}
