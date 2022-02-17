using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class Attacks : NetworkBehaviour
{
    private GameObject armObj;
    [SerializeField] private static List<GameObject> weapons = new List<GameObject>();
    public GameObject Player;
    private float offset = 0.25f;
    public MultiplayerController cont;
    public GameObject weaponHolderPrefab;
    private GameObject weaponHolder;
    private static Vector3 holderScale = new Vector3();

    public void init()
    {
        if (!hasAuthority) { return; }
        if (isServer)
        {
            RpcInstiantiate(weaponHolderPrefab);
        }
        else
        {
            CmdInstantiate(weaponHolderPrefab);
        }
        armObj = weaponHolder.transform.GetChild(0).gameObject;

        armObj.transform.localPosition = Vector3.zero;
        holderScale = weaponHolder.transform.localScale;
    }

    [Client]
    void Update()
    {
        if (!hasAuthority) { return; }
        if (isServer)
        {
            RpcHolder();
        }
        else
        {
            CmdHolder();
        }
    }


    [Client]
    public void lightPunch()
    {
        cont.canAttack = false;
        if (isServer)
        {
            Debug.Log(armObj.name);
            RpcObj(armObj.GetComponent<NetworkIdentity>(), true);
        }
        else
        {
            CmdObj(armObj.GetComponent<NetworkIdentity>(), true);
        }
        cont.StartCoroutine(cont.disableObj(0.21f, armObj));
        cont.StartCoroutine(cont.canattack(0.21f));
    }

    [Command]
    public void CmdObj(NetworkIdentity obj, bool enabling)
    {
        RpcObj(obj, enabling);
    }

    [ClientRpc]
    public void RpcObj(NetworkIdentity obj, bool enabling)
    {
        obj.gameObject.SetActive(enabling);
    }

    [Command]
    public GameObject CmdInstantiate(GameObject obj)
    {
        return RpcInstiantiate(obj);
    }

    [ClientRpc]
    public GameObject RpcInstiantiate(GameObject obj)
    {
        return Instantiate(obj);
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
            weaponHolder.transform.localScale = holderScale;

            weaponHolder.transform.position = new Vector3(position.x - offset, position.y + 0.03f, position.z);
        }
        else
        {
            weaponHolder.transform.localScale = new Vector3(-holderScale.x, holderScale.y, holderScale.z);

            weaponHolder.transform.position = new Vector3(position.x + offset, position.y + 0.03f, position.z);
        }
    }


    
}
