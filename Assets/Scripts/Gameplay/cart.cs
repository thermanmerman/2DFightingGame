using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cart : MonoBehaviour
{
    private GameObject player;
    int damage = 5;
    private float fraction = 0.7f;
    public int force = 200;
    void Start()
    {
        GameObject[] maybe = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject plyer in maybe)
        {
            if (plyer.name == "Player")
            {
                player = plyer;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject.name == "Player2")
            {
                Vector2 dir = player.transform.position - hitCollider.transform.position;
                dir.y = dir.y + 0.5f;
                Player2Controller controller = hitCollider.gameObject.GetComponent<Player2Controller>();
                controller.enabled = false;
                

                health health = hitCollider.GetComponent<health>();
                health.number += damage;
                float multiplier = health.number * fraction;
                health.number += damage;

                Vector2 newDir = new Vector2(-dir.x, dir.y);
                hitCollider.gameObject.GetComponent<Rigidbody2D>().AddForce(newDir * (force + multiplier));
                StartCoroutine(enablecontrols(0.2f, controller));
            }
    }

    IEnumerator enablecontrols(float time, Player2Controller cont)
    {
        yield return new WaitForSeconds(time);
        cont.enabled = true;
        player.GetComponent<PlayerController>().canAttack = true;
        Destroy(this.gameObject);
    }
}
