using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    Vector2 movevec;
    float speed;
    float Lasttime;
    Rigidbody2D rb;
    enum Team
    {
        Red,Blue
    }
    Team myTeam ;
    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(1.0f,1.5f);
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        if (Random.Range(1, 2) == 1) SetTeam(1);
        else SetTeam(2);
    }
    void SetTeam(int Teams)
    { 
        if (Teams == 1) myTeam = Team.Red;
        if (Teams == 2) myTeam = Team.Blue;
    }

    // Update is called once per frame
    void Update()
    {
        if (Lasttime > 0) Lasttime -= Time.deltaTime;
        else
        {
            movevec = Random.insideUnitCircle.normalized;
            Lasttime = Random.Range(1.5f,3.0f);
            rb.velocity = movevec * speed;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ball"))
        {
            soccerball temp=collision.gameObject.GetComponent<soccerball>();
            temp.Kick(2,new Vector2((int )(myTeam)*2-1,0));
        }
    }
}
