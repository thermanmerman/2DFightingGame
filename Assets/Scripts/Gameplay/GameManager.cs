using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera cam;
    public bool is1Visible;
    public bool is2Visible;
    public GameObject Player1;
    public GameObject Player2;
    public ParticleSystem armpart;
    public ParticleSystem bigexplosion;
    private bool died = false;
    private PlayerController p1cont;
    private Player2Controller p2cont;

    void Start()
    {
        p1cont = Player1.GetComponent<PlayerController>();
        p2cont = Player2.GetComponent<Player2Controller>();
    }
    void Update()
    {
        if (!died)
        {
            is1Visible = cam.IsObjectVisible(Player1.GetComponent<SpriteRenderer>());
            is2Visible = cam.IsObjectVisible(Player2.GetComponent<SpriteRenderer>());
        }
        else
        {
            is1Visible = true;
            is2Visible = true;
        }
        
        if (!is1Visible)
        {
            died = true;
            playarms(Player1);

            GameObject box = Player1.transform.GetChild(4).gameObject;
            box.SetActive(false);
            Player1.GetComponent<PlayerController>().rotateheli = false;
        }

        if (!is2Visible)
        {
            died = true;
            playarms(Player2);

            GameObject box = Player2.transform.GetChild(4).gameObject;
            box.SetActive(false);
            Player2.GetComponent<Player2Controller>().rotateheli = false;
        }
    }

    void playarms(GameObject player)
    {   
        Vector3 center = Camera.main.ViewportToWorldPoint(new Vector3(.5f, .5f, 10f));

        Vector3 dir = player.transform.position - center;
        dir = -dir.normalized;

        Quaternion rot = new Quaternion();
        rot.SetLookRotation(dir);
        Instantiate(armpart, player.transform.position, rot);
        Instantiate(bigexplosion, player.transform.position, rot);
        player.SetActive(false);
        
        StartCoroutine(respawn(2f, player));
    }

    IEnumerator respawn(float sec, GameObject player)
    {
        yield return new WaitForSeconds(sec);
        player.SetActive(true);
        player.transform.position = new Vector2(0, 1.18f);
        died = false;
        player.GetComponent<health>().number = 0;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (player.name == "Player")
        {
            PlayerController cont = player.GetComponent<PlayerController>();
            cont.enabled = true;
            cont.canAttack = true;
            Quaternion rot = player.transform.rotation;
            rot.eulerAngles = Vector3.zero;
            player.transform.rotation = rot;

            for (int i = 0; i < player.transform.childCount; i++)
            {
                player.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else if (player.name == "Player2")
        {
            Player2Controller cont = player.GetComponent<Player2Controller>();
            cont.enabled = true;
            Quaternion rot = player.transform.rotation;
            rot.eulerAngles = Vector3.zero;
            player.transform.rotation = rot;
            cont.enabled = true;
            cont.canAttack = true;
            
            for (int i = 0; i < player.transform.childCount; i++)
            {
                player.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        
    }


    
}

public static class CameraEx
{
    public static bool IsObjectVisible(this UnityEngine.Camera @this, Renderer renderer)
    {
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(@this), renderer.bounds);
    }
}
