using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombcollidedetect2 : MonoBehaviour
{
    private GameObject player;
    public ParticleSystem explosion;
    public int damage = 10;

    void Start()
    {
        GameObject[] maybe = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject plyer in maybe)
        {
            if (plyer.name == "Player2")
            {
                player = plyer;
            }
        }
    }

    void OnTriggerEnter2D()
    {
        Instantiate(explosion, this.transform.position, new Quaternion(0, 0, 0, 0));

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(this.transform.position, 1);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.name == "Player")
            {
                Vector2 dir = this.transform.position - hitCollider.transform.position;
                PlayerController controller = hitCollider.gameObject.GetComponent<PlayerController>();
                controller.enabled = false;
                StartCoroutine(enablecontrols(0.2f, controller));

                health health = hitCollider.GetComponent<health>();
                health.number += damage;
                float multiplier = health.number * 0.7f;
                health.number += damage;
               // c.GetComponent<Rigidbody2D>().AddForce(dir*hitforce / multiplier * 5);

                hitCollider.gameObject.GetComponent<Rigidbody2D>().AddForce(-dir * (100 + multiplier));
            }
        }   
    }

    IEnumerator enablecontrols(float time, PlayerController cont)
    {
        yield return new WaitForSeconds(time);
        cont.enabled = true;
        player.GetComponent<Player2Controller>().canAttack = true;
        Destroy(this);
    }
}
