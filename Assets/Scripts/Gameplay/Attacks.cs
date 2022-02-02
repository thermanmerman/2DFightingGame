using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class Attacks : NetworkBehaviour
{
    private static GameObject weaponHolder;
    private static GameObject armObj;
    [SerializeField] private static List<GameObject> weapons = new List<GameObject>();
    public GameObject Player;
    private float offset = 0.25f;
    //public bool isServer;
    public MultiplayerController cont;
    private Vector3 holderScale;

    public Attacks()
    {

    }
    public void init()
    {
        
        if (isServer)
        {
            weaponHolder = GameObject.Find("WeaponHolder");
            armObj = weaponHolder.transform.GetChild(0).gameObject;
            RpcObj(armObj, false);
            //armObj.SetActive(false);

            for (int i = 0; i < weaponHolder.transform.childCount; i++)
            {
                weapons.Add(weaponHolder.transform.GetChild(i).gameObject);
                //weaponHolder.transform.GetChild(i).gameObject.transform.SetParent(this.transform);
            }




            armObj.transform.localPosition = Vector3.zero;

            holderScale = weaponHolder.transform.localScale;
        }

        else
        {
            //Player 2 stuff
        }
        

        
/*
        try
        {
            cont = Player.GetComponent<MultiplayerController>();
            Debug.Log(cont.gameObject.name);
        }
        catch
        {
            Debug.Log("well what the fuck");
        }
        */
    }


    [Client]
    public void lightPunch()
    {
        cont.canAttack = false;
        if (isServer)
        {
            RpcObj(armObj, true);
        }
        else
        {
            CmdObj(armObj, true);
        }
        cont.StartCoroutine(cont.disableObj(0.21f, armObj));
        cont.StartCoroutine(cont.canattack(0.21f));
    }

    [Command]
    public void CmdObj(GameObject obj, bool enabling)
    {
        RpcObj(obj, enabling);
    }

    [ClientRpc]
    public void RpcObj(GameObject obj, bool enabling)
    {
        obj.SetActive(enabling);
    }

    [Command]
    public void CmdHolder()
    {
        RpcHolder();
    }

    [ClientRpc]
    public void RpcHolder()
    {
        Vector3 position = Player.transform.position;
        Vector3 size = Player.transform.localScale;

        if (size.x < 0)
        {
            weaponHolder.transform.localScale = new Vector3(-holderScale.x, holderScale.y, holderScale.z);

            weaponHolder.transform.position = new Vector3(position.x - offset, position.y + 0.05f, position.z);
        }
        else
        {
            weaponHolder.transform.localScale = holderScale;

            weaponHolder.transform.position = new Vector3(position.x + offset, position.y + 0.05f, position.z);
        }
    }


    
}
