using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    
    private PlayerControls controls;
    public float waitTime;
    public bool crouching;
    private bool keyUp = false;
    private bool jumping = false;

    private float waitTime2;
    public bool crouching2;
    private bool keyUp2 = false;
    private bool jumping2 = false;
    private BoxCollider2D col;
    public PolygonCollider2D p1Col;
    public PolygonCollider2D p2Col;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        controls = new PlayerControls();
        effector = GetComponent<PlatformEffector2D>();

        controls.Player.Crouch.started += g => crouching = true;
        controls.Player.Crouch.canceled += g => crouching = false;

        controls.Player.Jump.started += o => jumping = true;
        controls.Player.Jump.canceled += o => jumping = false;


        controls.Player2.Crouch.started += g => crouching2 = true;
        controls.Player2.Crouch.canceled += g => crouching2 = false;

        controls.Player2.Jump.started += o => jumping2 = true;
        controls.Player2.Jump.canceled += o => jumping2 = false;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Update()
    {
        if (!crouching && keyUp)
        {
            waitTime = 0.1f;
            keyUp = false;
            effector.rotationalOffset = 0;
        }

        if (!crouching2 && keyUp2)
        {
            waitTime2 = 0.1f;
            keyUp2 = false;
            effector.rotationalOffset = 0;
        }
        
        if (crouching && col.IsTouching(p1Col))
        {
            keyUp = true;
            if (waitTime <= 0)
            {
                effector.rotationalOffset = 180f;
                waitTime = 0.1f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (crouching2 && col.IsTouching(p2Col))
        {
            keyUp2 = true;
            if (waitTime <= 0)
            {
                effector.rotationalOffset = 180f;
                waitTime = 0.1f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        if (jumping || jumping2)
        {
            effector.rotationalOffset = 0;
        }
    }
}
