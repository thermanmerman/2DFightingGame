using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float jumpStrength;

    private Rigidbody2D rb;
    private PlayerControls controls;

    private float movement;
    public bool crouching;

    public bool isGrounded;

    [Space]
    public GameObject arm;
    public GameObject foot;
    public GameObject slide;
    public LayerMask ground;
    public GameObject bomb;
    public GameObject arm1;
    public GameObject arm2;
    public GameObject cart;
    public ParticleSystem puff;
    public GameObject shield;
    public GameObject helihat;
    public GameObject heliprop;
    public GameObject baby;

    [Space]
    public bool defending = false;
    private bool jumping = false;
    public bool canAttack = true;
    public bool hitable = true;
    public float dashSpeed = 1.0f;
    public bool rotateheli = false;

    private int i = 0;
    private float t = 0;
    private Vector3 startScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
        startScale = this.transform.localScale;

        controls.Player.Movement.performed += ctx => movement = ctx.ReadValue<float>();
        controls.Player.Movement.canceled += _ => movement = 0;
        controls.Player.Jump.started += StartCoroutine_Auto => Jump();
        controls.Player.Jump.started += o => jumping = true;
        controls.Player.Jump.canceled += o => jumping = false;
        controls.Player.LightAttack.started += v => LightAttack();
        controls.Player.Crouch.started += g => crouching = true;
        controls.Player.Crouch.canceled += g => crouching = false;
        controls.Player.Special.started += y => special(); 
        controls.Player.Defense.started += z => defense();
        controls.Player.Defense.started += j => defending = true;
        controls.Player.Defense.canceled += j => defending = false;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
    void Update()
    {
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
        }
        else if (crouching == false)
        {
            Vector3 scale = transform.localScale;
            scale.y = startScale.y;
            transform.localScale = scale;
        }
        
        //Shield
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        Rigidbody2D rig = GetComponent<Rigidbody2D>();
        if (defending && rig.velocity.magnitude < 0.1f && isGrounded)
        {
            //Removing the ability to be damage
            canAttack = false;
            shield.SetActive(true);
            i = 1;
            col.enabled = false;
            rig.bodyType = RigidbodyType2D.Kinematic;
        }
        else if (!defending && i == 1 || movement != 0 && i == 1 || controls.Player.Jump.enabled && i == 1)
        {
            //Enabling the ability to be damaged
            shield.SetActive(false);
            StartCoroutine(canattack(0.3f));
            i = 0;
            col.enabled = true;
            rig.bodyType = RigidbodyType2D.Dynamic;
        }

        //Propeller hat
        if (rotateheli)
        {
            //Rotate propeller
            t += Time.deltaTime/3;
            Quaternion rot = heliprop.transform.localRotation;
            rot.eulerAngles = Vector3.Lerp(rot.eulerAngles, new Vector3(0,10000f,0), t);
            heliprop.transform.localRotation = rot;

            //Moving character up
            if (heliprop.transform.position.y < 1.159f)
                helihat.transform.parent.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 2.5f);
        }
    }

    private void FixedUpdate()
    {
        //Setting movement velocity
        rb.velocity = new Vector2(movement * movementSpeed, rb.velocity.y);

        //Ground check
        Vector2 bottom = new Vector2(foot.transform.position.x, foot.transform.position.y - 0.1f);
        isGrounded = Physics2D.OverlapArea(arm.transform.position, bottom, ground);
        
    }

    private void defense()
    {
        Vector3 leftDash = new Vector3(transform.position.x - 1.5f, transform.position.y, transform.position.z);
        Vector3 rightDash = new Vector3(transform.position.x + 1.5f, transform.position.y, transform.position.z);
        
        if (controls.Player.Movement.ReadValue<float>() < 0 && canAttack) //Dashing left
        {
            canAttack = false;
            ParticleSystem clone1 = Instantiate(puff, transform.position, new Quaternion(0, 0, 0, 0));
            transform.position = Vector3.Lerp(transform.position, leftDash, dashSpeed);
            ParticleSystem clone2 = Instantiate(puff, leftDash, new Quaternion(0, 0, 0, 0));
            StartCoroutine(stopPuff(clone1, clone2, 2f));

            StartCoroutine(canattack(0.3f));
        }
        else if (controls.Player.Movement.ReadValue<float>() > 0 && canAttack) //Dashing right
        {
            canAttack = false;
            ParticleSystem clone1 = Instantiate(puff, transform.position, new Quaternion(0, 0, 0, 0));
            transform.position = Vector3.Lerp(transform.position, rightDash, dashSpeed);
            ParticleSystem clone2 = Instantiate(puff, rightDash, new Quaternion(0, 0, 0, 0));
            StartCoroutine(stopPuff(clone1, clone2, 2f));

            StartCoroutine(canattack(0.3f));
        }
    }
    IEnumerator stopPuff(ParticleSystem one, ParticleSystem two, float time) //Stop particle effect from dash
    {
        yield return new WaitForSeconds(time);
        one.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        two.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    private void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
        }
        
    }

    private void LightAttack()
    {
        if (controls.Player.Movement.ReadValue<float>() == 0 && isGrounded && !crouching && canAttack) //Small punch
        {
            canAttack = false;
            arm.SetActive(true);
            StartCoroutine(disablearm(0.21f));
            StartCoroutine(canattack(0.21f));
        }
        else if (!isGrounded && controls.Player.Movement.ReadValue<float>() == 0 && !crouching && canAttack) //Small kick
        {
            canAttack = false;
            foot.SetActive(true);
            StartCoroutine(disablefoot(0.25f));
            StartCoroutine(canattack(0.25f));
        }
        else if (controls.Player.Movement.ReadValue<float>() > 0 && isGrounded && !crouching && canAttack || controls.Player.Movement.ReadValue<float>() < 0 && isGrounded && !crouching && canAttack)//Slide kick
        {
            canAttack = false;
            slide.SetActive(true);
            StartCoroutine(disableslide(0.25f));
            StartCoroutine(canattack(0.25f));
        }
        else if (!isGrounded && controls.Player.Movement.ReadValue<float>() > 0 && !crouching && canAttack) //Spin right
        {
            canAttack = false;
            rb.constraints -= RigidbodyConstraints2D.FreezeRotation;
            rb.AddTorque(200);
            StartCoroutine(restrict(0.3f));
        }
        else if (!isGrounded && controls.Player.Movement.ReadValue<float>() < 0 && !crouching && canAttack) //Spin left
        {
            canAttack = false;
            rb.constraints -= RigidbodyConstraints2D.FreezeRotation;
            rb.AddTorque(-200);
            StartCoroutine(restrict(0.3f));
        }
        else if (isGrounded && crouching && canAttack) //Crouch punch
        {
            canAttack = false;
            arm1.SetActive(true);
            arm2.SetActive(true);
            StartCoroutine(disablearms(0.21f));
            StartCoroutine(canattack(0.21f));
        }
        
        
    }

    private void special()
    {
        if (isGrounded && crouching && canAttack) //Bomb drop
        {
            canAttack = false;
            Instantiate(bomb, new Vector3(transform.position.x, transform.position.y + 2.663f, 0f), new Quaternion(0.707106829f,0,0,0.707106829f));
            StartCoroutine(canattack(3f));
        }
        else if (controls.Player.Movement.ReadValue<float>() < 0 && !crouching && canAttack) //Shopping cart left
        {
            canAttack = false;
            GameObject card = Instantiate(cart, new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z), new Quaternion(0,0.707106829f,0,0.707106829f));
            card.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 200);
            StartCoroutine(canattack(1f));
        }
        else if (controls.Player.Movement.ReadValue<float>() > 0 && !crouching && canAttack) //Shopping cart right
        {
            canAttack = false;
            GameObject card = Instantiate(cart, new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z), new Quaternion(0,-0.707106829f,0,0.707106829f));
            card.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 200);
            StartCoroutine(canattack(1f));
        }
        else if (!isGrounded && canAttack && jumping) //Propeller hat
        {
            GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / 2.5f;
            helihat.SetActive(true);
            rotateheli = true;
            canAttack = false;
            StartCoroutine(disablehat(3));
            StartCoroutine(canattack(3));
        }
        else if (canAttack && transform.localScale.x < 0) //Baby throw left
        {
            canAttack = false;
            GameObject threw = Instantiate(baby, new Vector3(transform.position.x - 0.5f, transform.position.y + 0.2f, transform.position.z), new Quaternion(0,0,0,1));
            threw.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 200);
            threw.GetComponent<Rigidbody2D>().AddTorque(1000);
            StartCoroutine(canattack(0.7f));
            StartCoroutine(destroybaby(2, threw));
        }
        else if (canAttack && transform.localScale.x > 0) //Baby throw right
        {
            canAttack = false;
            GameObject threw = Instantiate(baby, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.2f, transform.position.z), new Quaternion(0,0,0,1));
            threw.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 200);
            threw.GetComponent<Rigidbody2D>().AddTorque(-1000);
            StartCoroutine(canattack(0.7f));
            StartCoroutine(destroybaby(2, threw));
        }
    }

    IEnumerator disablehat(float time)
    {
        yield return new WaitForSeconds(time);
        helihat.SetActive(false);
        heliprop.transform.localRotation = new Quaternion(-0.0465362929f,0.0401151367f,0.019356763f,0.997923136f);
        rotateheli = false;
        t = 0;
    }
    IEnumerator destroybaby(float time, GameObject baby)
    {
        yield return new WaitForSeconds(time);
        Destroy(baby);
    }

    IEnumerator disablearm(float time)
    {
        yield return new WaitForSeconds(time);
        arm.SetActive(false);
        canAttack = true;
    }
    IEnumerator canattack(float time)
    {
        yield return new WaitForSeconds(time);
        canAttack = true;
    }
    IEnumerator disablefoot(float time)
    {
        yield return new WaitForSeconds(time);
        foot.SetActive(false);
        canAttack = true;
    }
    IEnumerator disableslide(float time)
    {
        yield return new WaitForSeconds(time);
        slide.SetActive(false);
        canAttack = true;
    }
    IEnumerator restrict(float time)
    {
        yield return new WaitForSeconds(time);
        rb.transform.eulerAngles = Vector3.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        canAttack = true;
    }
    IEnumerator disablearms(float time)
    {
        yield return new WaitForSeconds(time);
        arm1.SetActive(false);
        arm2.SetActive(false);
        canAttack = true;
    }
}
