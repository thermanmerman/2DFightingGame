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
    private Attacks attacks;

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
        if (!hasAuthority) { return; }

        attacks = this.gameObject.GetComponent<Attacks>();
        attacks.init();
    
        rb = GetComponent<Rigidbody2D>();

        startScale = transform.localScale;

    }

    private void OnEnable() => Controls.Enable();
    private void OnDisable() => Controls.Disable();

    [Client]
    private void Update()
    {
        if(!hasAuthority) { return; }
        attacks.Update();
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

    [Client]
    private void LightAttack()
    {
        if (!hasAuthority) { return; }
        
        if (movement == 0 && isGrounded && !crouching && canAttack && isActiveAndEnabled) //Small punch
        {
            attacks.lightPunch();
        }
        else
        {
            //Other attacks
        }
    }


    [Client]
    public IEnumerator canattack(float time)
    {
        yield return new WaitForSeconds(time);
        canAttack = true;
    }

   public bool getAuthority()
   {
       return hasAuthority;
   }
    
    
}
