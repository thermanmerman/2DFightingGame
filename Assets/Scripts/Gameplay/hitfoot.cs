using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitfoot : MonoBehaviour
{
    private CapsuleCollider2D thiscol;
    public float hitforce;
    public Player2Controller controller;
    public int damage = 10; 
    void Start()
    {
        thiscol = GetComponent<CapsuleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            if (c.gameObject.name != "Player")
            {
                health health = c.GetComponent<health>();
                health.number += damage;
                float multiplier = health.number * 0.7f;

                Vector2 hitvec = new Vector2(0f, hitforce + multiplier);
                c.GetComponent<Rigidbody2D>().AddForce(hitvec);
            }
            
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, hitforce));

            controller.enabled = false;
            StartCoroutine(enablecontrols(0.2f, controller));
        
        }
    }

    IEnumerator enablecontrols(float time, Player2Controller cont)
    {
        yield return new WaitForSeconds(time);
        cont.enabled = true;
    }
}
