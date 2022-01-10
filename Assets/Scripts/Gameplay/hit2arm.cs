using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hit2arm : MonoBehaviour
{
    private CapsuleCollider2D thiscol;
    public float hitforce;
    public PlayerController controller;
    public int damage = 5; 
    
    // Start is called before the first frame update
    void Start()
    {
        thiscol = GetComponent<CapsuleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            // Calculate Angle Between the collision point and the player
            Vector3 dir = new Vector2(c.transform.position.x, c.transform.position.y) - new Vector2 (transform.position.x, transform.position.y);
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            // And finally we add force in the direction of dir and multiply it by force. 
            // This will push back the player

            if (c.gameObject.name != "Player2")
            {
                health health = c.GetComponent<health>();
                health.number += damage;
                float multiplier = health.number * 0.7f;
                c.GetComponent<Rigidbody2D>().AddForce(dir*(hitforce + multiplier));
            }
            

            controller.enabled = false;
            StartCoroutine(enablecontrols(0.1f, controller));

            

        }
    }
    IEnumerator enablecontrols(float time, PlayerController cont)
    {
        yield return new WaitForSeconds(time);
        cont.enabled = true;
    }
}
