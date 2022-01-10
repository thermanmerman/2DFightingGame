using System.Collections;
using UnityEngine;
using Mirror;

public class hitarm : MonoBehaviour
{
    private CapsuleCollider2D thiscol;
    public float hitforce;
    //public Player2Controller controller;
    public int damage = 5; 
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        thiscol = GetComponent<CapsuleCollider2D>();
    }


    [Client]
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

            if (c.gameObject != this.transform.parent.gameObject)
            {
                health health = c.GetComponent<health>();
                health.number += damage;
                float multiplier = health.number * 0.7f;
                c.GetComponent<Rigidbody2D>().AddForce(dir*(hitforce + multiplier));
                try
                {
                    try
                    {
                        Player2Controller controller = c.gameObject.GetComponent<Player2Controller>();
                        controller.enabled = false;
                        StartCoroutine(enable2controls(0.1f, controller));
                    }
                    catch
                    {
                        PlayerController controller = c.gameObject.GetComponent<PlayerController>();
                        controller.enabled = false;
                        StartCoroutine(enablecontrols(0.1f, controller));
                    }
                    
                }
                catch
                {
                    MultiplayerController controller = c.gameObject.GetComponent<MultiplayerController>();
                    controller.enabled = false;
                    StartCoroutine(enableNetControls(0.1f, controller));
                }
            }
            

            

            

        }
    }

    IEnumerator enableNetControls(float time, MultiplayerController cont)
    {
        yield return new WaitForSeconds(time);
        cont.enabled = true;
    }
    IEnumerator enablecontrols(float time, PlayerController cont)
    {
        yield return new WaitForSeconds(time);
        cont.enabled = true;
    }
    IEnumerator enable2controls(float time, Player2Controller cont)
    {
        yield return new WaitForSeconds(time);
        cont.enabled = true;
    }
}
