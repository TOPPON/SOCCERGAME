using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soccerball : MonoBehaviour
{
    float speed;
    Vector2 movevec;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        movevec = new Vector2(0.7f, 0.7f);
        speed = 3;
    }

    public void Kick(float addspeed, Vector2 addvec)
    {
        Vector2 a = speed * movevec + addspeed * addvec;
        speed = a.magnitude;
        movevec = a.normalized;
    }
    void Refresh()
    {
        Vector2 a = rb.velocity;
        speed = a.magnitude;
        movevec = a.normalized;
        rb.velocity=movevec*speed;
    }
    // Update is called once per frame
    void Update()
    {
        //Refresh();
        rb.velocity = movevec * speed;
        if (speed > 0)
        {
            speed -= Time.deltaTime*2;
        }
        else speed = 0;
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Kick(3, new Vector2(Random.Range(-1.0f,1.0f), Random.Range(-1.0f, 1.0f)));
        }
    }
}
