using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerController : NetworkBehaviour
{
    //Private stuff
    private PlayerControls controls;
    private Rigidbody2D rb;
    private Vector3 startScale;
    private GameObject armObj;

    [Header("Booleans")]
    public bool canAttack = true;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool crouching;

    [Space]
    [Header("Changeable values")]
    public float movementSpeed = 2;
    public float distance = 0.16f;
    public int jumpStrength = 4;
    public LayerMask ground;
    private GameObject weaponHolder;
    [SerializeField] List<GameObject> weapons = new List<GameObject>();
    private List<Transform> weaponTransforms = new List<Transform>();
    public float offset = 1f;
    

    [Space]
    [Header("Static values")]
    [SerializeField] private float movement = new float();

    private PlayerControls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new PlayerControls();
        }
    }

    public override void OnStartAuthority()
    {
        controls.Player.Movement.performed += ctx => movement = ctx.ReadValue<float>();
        controls.Player.Movement.canceled += _ => movement = 0;

        controls.Player.Jump.started += StartCoroutine_Auto => Jump();

        controls.Player.Crouch.started += g => crouching = true;
        controls.Player.Crouch.canceled += g => crouching = false;

        controls.Player.LightAttack.started += v => LightAttack();
    }

    [Client]
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startScale = transform.localScale;

        weaponHolder = GameObject.Find("WeaponHolder");
        armObj = GameObject.FindWithTag("Weapon");
        armObj.SetActive(false);

        //attack.armObj = armObj;

        for (int i = 0; i < weaponHolder.transform.childCount; i++)
        {
            weapons.Add(weaponHolder.transform.GetChild(i).gameObject);
            weaponTransforms.Add(weapons[i].transform);
        }
        
    }

    private void OnEnable() => Controls.Enable();
    private void OnDisable() => Controls.Disable();

    [Client]
    private void Update()
    {
        if(!hasAuthority) { return; }

        if (movement < 0) //If moving left
        {
            //transform.localScale = new Vector3(-0.200599998f, 0.200599998f, 0.200599998f);
            Vector3 scale = transform.localScale;
            scale.x = -startScale.x;
            transform.localScale = scale;
        }
        else if (movement > 0) //If moving right
        {
            //transform.localScale = new Vector3(0.200599998f, 0.200599998f, 0.200599998f);
            Vector3 scale = transform.localScale;
            scale.x = startScale.x;
            transform.localScale = scale;
        }

        if (crouching == true)
        {

            Vector3 scale = transform.localScale;
            scale.y = startScale.y / 2;
            transform.localScale = scale;
            Debug.Log("current scale: " + transform.localScale);
        }
        else if (crouching == false)
        {
            Vector3 scale = transform.localScale;
            scale.y = startScale.y;
            transform.localScale = scale;
        }

        if(movement == 0){ return; }


        //Translation movement
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        pos += new Vector2(movement * movementSpeed * Time.deltaTime, 0);
        transform.position = pos;


        
    }

    [Client]
    private void FixedUpdate()
    {
        //Ground check
        Vector2 bottom = new Vector2(transform.position.x, transform.position.y - distance);
        isGrounded = Physics2D.OverlapArea(transform.position, bottom, ground);
    }

    private void Jump()
    {
        if (!hasAuthority) { return; }
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
        }
    }

    private void LightAttack()
    {
        if (!hasAuthority) { return; }
        
        //Attacks.melee melee;
        if (movement == 0 && isGrounded && !crouching && canAttack) //Small punch
        {
            canAttack = false;
            if (isServer)
            {
                RpcObject(0, true);
            }
            else
            {
                CmdObject(0, true);
            }
            //melee = Attacks.melee.arm;
            Debug.Log("bruh1");
            //attack.enableObjects(melee);
            StartCoroutine(disableObj(0.21f, 0));
            StartCoroutine(canattack(0.21f));
        }
        else
        {
            //melee = Attacks.melee.none;
        }
    }

    [Command]
    void CmdObject(int obj, bool enabled)
    {
        RpcObject(obj, enabled);
    }

    [ClientRpc]
    void RpcObject(int obj, bool enabled)
    {
        weapons[obj].SetActive(enabled);

        if (enabled)
        {

            weapons[obj].transform.SetParent(this.transform);
            weapons[obj].transform.localPosition = Vector3.zero;

            Vector3 pos = weapons[obj].transform.localPosition;

            if (transform.localScale.x > 0)
            {
                weapons[obj].transform.localPosition = new Vector3(pos.x + offset, pos.y + 0.1f, pos.z);
                Vector3 scale = weapons[obj].transform.localScale;
                if (scale.y > 0)
                {
                    scale.y *= -1;
                }
                weapons[obj].transform.localScale = new Vector3(scale.x, -scale.y, scale.z);
            }
            else if (transform.localScale.x < 0)
            {
                Vector3 scale = weapons[obj].transform.localScale;
                if (scale.y < 0)
                {
                    scale.y *= -1;
                }
                weapons[obj].transform.localScale = new Vector3(scale.x, -scale.y, scale.z);
                weapons[obj].transform.localPosition = new Vector3(pos.x + offset, pos.y + 0.1f, pos.z);
            }
        }
        else
        {
            weapons[obj].transform.SetParent(weaponHolder.transform);
            weapons[obj].transform.localScale = weaponTransforms[obj].localScale;
            weapons[obj].transform.localPosition = Vector3.zero;
            weapons[obj].transform.rotation = weaponTransforms[obj].rotation;
        }
    }

    [Client]
    private IEnumerator disableObj(float time, int obj)
    {
        yield return new WaitForSeconds(time);
        
        if (isServer)
        {
            RpcObject(obj, false);
        }
        else
        {
            CmdObject(obj, false);
        }
        
    }
    IEnumerator canattack(float time)
    {
        yield return new WaitForSeconds(time);
        canAttack = true;
    }
    
    
}
