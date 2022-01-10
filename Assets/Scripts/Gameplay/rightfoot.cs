using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rightfoot : MonoBehaviour
{
    public Material boxmat;
    public Material boxmat2;
    private Vector3 startpos;

    public GameObject box1;
    public GameObject box2;

    [Space]
    public GameObject leftFoot;
    public bool kicked = false;
    public bool leftkick = false;
    
    void Start()
    {
        startpos = transform.position;
        boxmat.color = Color.white;
    }
    
    void Update()
    {        

        //Moving box to foot
        if (boxmat.color == Color.red && transform.position.x >= box1.transform.position.x + 0.02f && !kicked)
        {
            float distance = box1.transform.position.x - box2.transform.position.x;
            if (distance > 0)
            {
                transform.position = Vector3.Lerp(transform.position, box1.transform.position, Time.time * 0.008f);
            }
        }
        if (boxmat2.color == Color.red && transform.position.x >= box2.transform.position.x + 0.02f && !kicked)
        {
            float distance = box2.transform.position.x - box1.transform.position.x;
            if (distance > 0)
            {
                transform.position = Vector3.Lerp(transform.position, box2.transform.position, Time.time * 0.008f);
            }
        }

        if (transform.position.x <= box1.transform.position.x + 0.1f)
        {
            boxmat.color = Color.white;
            Debug.Log("true");
            GameObject player1 = box1.transform.parent.gameObject;
            player1.GetComponent<PlayerController>().enabled = false;
            player1.GetComponent<PolygonCollider2D>().enabled = true;
            player1.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        if (transform.position.x <= box2.transform.position.x + 0.1f)
        {
            boxmat2.color = Color.white;
            Debug.Log("true");
            GameObject player2 = box2.transform.parent.gameObject;
            player2.GetComponent<Player2Controller>().enabled = false;
            player2.GetComponent<PolygonCollider2D>().enabled = true;
            player2.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }

        if (kicked)
        {
            transform.position = Vector3.Lerp(transform.position, startpos, Time.time * 0.01f);
            if (transform.position == startpos && !leftkick)
            {
                kicked = false;
            }
        }
        
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.name == "Player")
        {
            try
            {
                Debug.Log("Got it");
                GameObject player1 = box1.transform.parent.gameObject;
                StartCoroutine(pCont(0.5f, player1.GetComponent<PlayerController>()));

                boxmat.color = Color.white;
                kicked = true;
                leftFoot.GetComponent<leftfoot>().rightkick = true;
                StartCoroutine(leftcomm(1));
                box1.transform.parent.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 3500);
            }
            catch
            {
                return;
            }
        }

        if (coll.gameObject.name == "Player2")
        {
            try
            {
                Debug.Log("Got it");
                GameObject player2 = box2.transform.parent.gameObject;
                StartCoroutine(p2Cont(0.5f, player2.GetComponent<Player2Controller>()));

                boxmat2.color = Color.white;
                kicked = true;
                leftFoot.GetComponent<leftfoot>().rightkick = true;
                StartCoroutine(leftcomm(1));
                box2.transform.parent.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 3500);
            }
            catch
            {
                return;
            }
        }
        
    }
    void OnTrigger2D(Collider2D coll)
    {
        if (coll.gameObject.name == "Player")
            boxmat.color = Color.Lerp(Color.red, Color.white, 1);
        else if (coll.gameObject.name == "Player2")
            boxmat2.color = Color.Lerp(Color.red, Color.white, 1);
    }

    IEnumerator pCont(float time, PlayerController p)
    {
        yield return new WaitForSeconds(time);
        p.enabled = true;
    }
    IEnumerator p2Cont(float time, Player2Controller p)
    {
        yield return new WaitForSeconds(time);
        p.enabled = true;
    }
    IEnumerator leftcomm(float time)
    {
        yield return new WaitForSeconds(time);
        leftFoot.GetComponent<leftfoot>().rightkick = false;
    }

}
