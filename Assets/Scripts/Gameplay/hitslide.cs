using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitslide : MonoBehaviour
{
    private CapsuleCollider2D thiscol;
    public float yforce;
    public float xforce;
    public Player2Controller controller;
    public int damage = 5;
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
                float multiplier = health.number * 0.7f;

                health.number += damage;
    

                if (transform.parent.localScale.x > 0)
                {
                    Vector2 hitvec = new Vector2(xforce + multiplier, yforce + multiplier);
                    c.GetComponent<Rigidbody2D>().AddForce(hitvec);
                }
                else
                {
                    Vector2 hitvec = new Vector2(-xforce - multiplier, yforce + multiplier);
                    c.GetComponent<Rigidbody2D>().AddForce(hitvec);
                }
                controller.enabled = false;
                StartCoroutine(enablecontrols(0.15f));
            }

            
        
        }
    }

    IEnumerator enablecontrols(float time)
    {
        yield return new WaitForSeconds(time);
        controller.enabled = true;
    }
}
