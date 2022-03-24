using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class Attacks : NetworkBehaviour
{
    private GameObject armObj;
    [SerializeField] private List<GameObject> weapons = new List<GameObject>();
    private List<Transform> weaponTransforms = new List<Transform>();
    public GameObject Player;
    private float offset = 0.25f;
    public MultiplayerController cont;
    public GameObject armObjPre;
    private Weapons _weapons;
    private DisableWeapons disableWeapons;
    public bool isAwake = false;

    public void init()
    {
        if (!hasAuthority || !isLocalPlayer) { return; }
        isAwake = true;

        armObj = localSpawnGameObj(armObjPre);
        if (isServer)
        {
            RpcSpawnGameObj(armObjPre);
        }
        else
        {
            CmdSpawnGameObj(armObjPre);
        }
        

        weapons.Add(armObj); //Adding all weapons in game to a list
        //MUST ADD ALL WEAPONS TO weapons LIST
        
        for (int i = 0; i < weapons.Count; i++)
        {
            weaponTransforms.Add(weapons[i].transform); //Gets all weapons' starting transforms (scale is most important here)
        }

        armObj.transform.localPosition = Vector3.zero;

        _weapons = new Weapons(armObj);
        disableWeapons = this.gameObject.AddComponent<DisableWeapons>();
    }


    [Client]
    public void Update()
    {
        if (!hasAuthority || !isLocalPlayer) { return; }
        //if (!isAwake){ Debug.Log("Not awake"); Awake(); }
        moveWeapons();
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
        if (!isAwake || !isLocalPlayer) { return; }

        cont.canAttack = false;
        _weapons.localEnableArm(true);
        if (isServer)
        {
            Debug.Log(armObj.name);
            _weapons.RpcArm(true, isLocalPlayer);
        }
        else
        {
            _weapons.CmdArm(true, isLocalPlayer);
        }
 
        disableWeapons.StartCoroutine(disableWeapons.disableArm(0.21f, isServer, _weapons, isLocalPlayer));
        cont.StartCoroutine(cont.canattack(0.21f));
    }

    [Command]
    public void CmdHolder()
    {
        moveWeapons();
        RpcHolder();
    }

    [ClientRpc]
    public void RpcHolder()
    {
        if (isLocalPlayer) { return; }
        moveWeapons();
    }

    private void moveWeapons()
    {
        if (!isAwake) { return; }
        Vector3 position = Player.transform.position;
        Vector3 size = Player.transform.localScale;

        if (size.x < 0)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].transform.localScale = new Vector3(weaponTransforms[i].localScale.x, -weaponTransforms[i].localScale.y, weaponTransforms[i].localScale.z);

                weapons[i].transform.position = new Vector3(position.x - offset, position.y + 0.03f, position.z);
            }
        }
        else
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].transform.localScale = new Vector3(weaponTransforms[i].localScale.x, weaponTransforms[i].localScale.y, weaponTransforms[i].localScale.z);

                weapons[i].transform.position = new Vector3(position.x + offset, position.y + 0.03f, position.z);
            }
        }
    }

    private GameObject localSpawnGameObj(GameObject item)
    {
        GameObject obj = Instantiate(item);
        //NetworkServer.Spawn(obj);
        return obj;
    }

    [Command]
    private void CmdSpawnGameObj(GameObject item)
    {
        localSpawnGameObj(item);
        RpcSpawnGameObj(item);   
    }

    [ClientRpc]
    private void RpcSpawnGameObj(GameObject item)
    {
        if (isLocalPlayer) { return; }
        localSpawnGameObj(item);
        NetworkServer.Spawn(item);
    }
    
}
