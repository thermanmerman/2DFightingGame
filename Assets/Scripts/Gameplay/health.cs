using UnityEngine;
using Mirror;

public class health : MonoBehaviour
{
    public int number = 0;
    public SpriteRenderer sprite;

    [Client]
    void Update()
    {
        float GB = (100 - number) * 2.55f;
        Color newcolor = new Color(1, 1 - number*0.01f, 1 - number*0.01f, 1);
        sprite.color = newcolor;

        if (transform.gameObject.tag == "Player")
        {
            try
            {
                try
                {
                    PlayerController cont = GetComponent<PlayerController>();
                    if (!cont.enabled)
                    {
                        transform.rotation = new Quaternion(0,0,0.145567641f,0.989348292f);
                    }
                    else
                    {
                        transform.rotation = new Quaternion(0,0,0,1);
                    }
                }
                catch
                {
                    Player2Controller cont = GetComponent<Player2Controller>();
                    if (!cont.enabled)
                    {
                        transform.rotation = new Quaternion(0,0,0.107281633f,-0.994228661f);
                    }
                    else
                    {
                        transform.rotation = new Quaternion(0,0,0,1);
                    }
                }
                
            }
            catch
            {
                MultiplayerController cont = GetComponent<MultiplayerController>();
                if (!cont.enabled)
                {
                    transform.rotation = new Quaternion(0,0,0.145567641f,0.989348292f);
                }
                else
                {
                    transform.rotation = new Quaternion(0,0,0,1);
                }
                
            }
            
        }
        
    }
}
