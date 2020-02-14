using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    Vector2 movevec;
    float freezetime;
    Rigidbody2D rb;
    public GameObject BlueSprite;
    public GameObject RedSprite;
    public soccerball Ball;
    float firstspd;
    public Vector2 GoalPos;
    public player passtarget;
    //パラメータ系
    float speed;
    float driblespdrate;//ドリブルの速度0~10:0.7~0.9
    float judgetime;//どれだけ先の情報を見るか
    float driblegoodness;//ドリブルのうまさ 
    float kickgoodness;//蹴るうまさ0.8~0.98
    float kickpower;//蹴る強さ
    TextMesh textMesh;
    //float 
    public Vector2 FirstPosition;
    Vector2 MovableArea;
    //パラメータ系
    public enum PlayerState
    {
        Idle,
        Drible,
        Chase,
        Back,
        Freeze//ドリブルで負けた側がなる
    }
    public PlayerState myState;
    public enum Team
    {
        Red,Blue
    }
    public Team myTeam ;
    // Start is called before the first frame update
    void Start()
    {
        myState = PlayerState.Idle;
        speed = Random.Range(0.6f,1.0f);
        firstspd = speed;
        MovableArea = new Vector2(3,3);
        textMesh = this.gameObject.GetComponent<TextMesh>();
        
        driblespdrate=0.8f;//ドリブルの速度0~10:0.7~0.9
        judgetime=0.3f;//どれだけ先の情報を見るか
        driblegoodness=0.8f;//ドリブルのうまさ 
        kickgoodness=0.8f;//蹴るうまさ0.8~0.98
        kickpower=2;//蹴る強さ
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        //if (Random.Range(1, 3) == 1) SetTeam(1);
        //else SetTeam(2);
        FirstPosition = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
    }
    public void SetTeam(int Teams)
    {
        if (Teams == 1) 
        {
            myTeam = Team.Red;
            BlueSprite.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            GoalPos = GameManager.Manager.RedGoalPos;
        }
        if (Teams == 2) 
        {
            myTeam = Team.Blue;
            RedSprite.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            GoalPos = GameManager.Manager.BlueGoalPos;
        }
    }
    void Playeridle()
    {
        rb.velocity = new Vector2(0, 0);
    }
    void Playerback()
    {
        Vector2 firstdis = -new Vector2(this.transform.position.x, this.transform.position.y) + FirstPosition;
        if (firstdis.sqrMagnitude<0.1)
        {
            myState = PlayerState.Idle;
            return ;
        }
        movevec = new Vector2(firstdis.x, firstdis.y);
        speed = firstspd;
        movevec.Normalize();
        rb.velocity = movevec * speed; 
    }
    void Playerchase()
    {
        if(Ball.bHaved!=null)
        {
            player temp=Ball.bHaved.GetComponent<player>();
            if (temp.myTeam.Equals(myTeam)) myState = PlayerState.Back;
        }
        Vector2 balldis = new Vector2(Ball.transform.position.x+Ball.rb.velocity.x*judgetime-this.transform.position.x, 
            Ball.transform.position.y + Ball.rb.velocity.y * judgetime - this.transform.position.y) ;
        movevec = new Vector2(balldis.x, balldis.y);
        speed = firstspd;
        movevec.Normalize();

        Vector2 firstdis = new Vector2(this.transform.position.x, this.transform.position.y) - FirstPosition;
        //Debug.Log(firstdis.x * firstdis.x / MovableArea.x / MovableArea.x + firstdis.y * firstdis.y / MovableArea.y / MovableArea.y);
        if (firstdis.x * firstdis.x / MovableArea.x / MovableArea.x + firstdis.y * firstdis.y / MovableArea.y / MovableArea.y >= 1)
        {
            float temp=Mathf.Sqrt(firstdis.x* firstdis.x / MovableArea.x / MovableArea.x + firstdis.y * firstdis.y / MovableArea.y / MovableArea.y)-1;
            this.transform.position=new Vector3(this.transform.position.x-firstdis.x*temp, this.transform.position.y - firstdis.y * temp, 0);
            rb.velocity =movevec*speed;
        }
        else
        {
            rb.velocity = movevec * speed;
        }
    }
    void Playerdrible()
    {
        //ドリブルするうえでパスのしやすさとドリブルとシュートの期待値を数値化、期待値の勝った方の行動をとる
        float passpoint = 0;
        float driblepoint = 0;
        float shootpoint = 0;
        Vector2 driblevec=new Vector2(0,0);
        Vector2 shootvec = new Vector2(0, 0);
        Vector2 passvec = new Vector2(0, 0);

        //パスに関する期待値計算、パス相手の距離、邪魔する人に奪われる可能性の数、ゴールに近づく具合、（キックのうまさ）
        passpoint =GameManager.Manager.PassCheck(this,ref passtarget,kickgoodness,kickpower);
        //ドリブルに関する期待値計算、（ドリブルのうまさ、走るスピード）、動ける範囲の外にならないかどうか
        driblepoint = GameManager.Manager.DribleCheck(this,driblegoodness,driblespdrate,speed,ref driblevec);
        //シュートに関する期待値計算、ゴールまでの距離、邪魔する人に奪われる可能性の数、キックのうまさ
        shootpoint = GameManager.Manager.ShootCheck(this,kickgoodness,kickpower,ref shootvec);
        print("ドリブル："+driblepoint.ToString() + ",パス：" + passpoint.ToString() + ",シュート" + shootpoint.ToString());
        if(driblepoint>=passpoint&&driblepoint>=shootpoint)//ドリブル
        {
            Ball.transform.position = this.transform.position + new Vector3(driblevec.x, driblevec.y);
            rb.velocity = driblevec.normalized * speed *driblespdrate;
            Debug.Log("ドリブル");
        }
        else if (passpoint >= driblepoint && passpoint >= shootpoint)//パス
        {
            passvec.x = passtarget.transform.position.x - this.transform.position.x;
            passvec.y = passtarget.transform.position.y - this.transform.position.y;
            passvec.x *= Random.Range(kickgoodness, 1.0f);
            passvec.y *= Random.Range(kickgoodness, 1.0f);
            passvec.Normalize();
            Ball.Kick(kickpower, passvec);
            Driblefinish();
            Ball.bHaved = null;
            Ball.transform.position = this.transform.position + new Vector3(passvec.x, passvec.y) * 0.2f;
            //Debug.Log("パス");
        }
        else //シュート
        {
            shootvec.x *= Random.Range(kickgoodness,1.0f);
            shootvec.y *= Random.Range(kickgoodness, 1.0f);
            shootvec.Normalize();
            Ball.Kick(kickpower, shootvec);
            Driblefinish();
            Ball.bHaved = null;
            Ball.transform.position = this.transform.position + new Vector3(shootvec.x, shootvec.y) * 0.2f;
            //Debug.Log("シュート");
        }
    }
    void Playerfreeze()
    {
        rb.velocity = new Vector2(0, 0);
        freezetime -= Time.deltaTime;
        if(freezetime<0)
        {
            myState = PlayerState.Chase;
        }
    }
    void TrytoRob(player enemy)
    {
        Debug.Log("ドリブルに対して奪いに行く");
        if (Random.Range(0,this.driblegoodness+enemy.driblegoodness)<enemy.driblegoodness)////////////////////////////////////////////////////////PARAM
        {
            this.Driblefinish();
        }
        else
        {
            enemy.Driblefinish();
            this.myState = PlayerState.Drible;
            Ball.bHaved = this.gameObject;
        }
    }
    void Driblefinish()
    {
        this.myState = PlayerState.Freeze;
        this.freezetime = 1.0f;////////////////////////////////////////////////////////PARAM
    }
    // Update is called once per frame
    void Update()
    {
        switch(myState)
        {
            case PlayerState.Back:
                Playerback();
                break;
            case PlayerState.Chase:
                Playerchase();
                break;
            case PlayerState.Drible:
                Playerdrible();
                break;
            case PlayerState.Idle:
                Playeridle();
                break;
            case PlayerState.Freeze:
                Playerfreeze();
                break;
        }
        textMesh.text = myState.ToString();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        
    /*}
    private void OnCollisionEnter2D(Collision2D collision)
    {*/
        //Debug.Log("範囲に当たった");
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (this.myState != PlayerState.Freeze)
            {
                Ball = collision.gameObject.GetComponent<soccerball>();
                if (Ball.bHaved == null)
                {
                    myState = PlayerState.Drible;
                    Ball.bHaved = this.gameObject;
                }
                else
                {
                    player enemy = Ball.bHaved.GetComponent<player>();
                    if (enemy.myTeam != this.myTeam)
                    {
                        this.TrytoRob(enemy);
                    }
                }

            }
            /*soccerball temp = collision.gameObject.GetComponent<soccerball>();
            temp.Kick(2, new Vector2((int)(myTeam) * 2 - 1, 0));*/
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("範囲に入った");
        if (collision.gameObject.CompareTag("Ball"))
        {
            //Debug.Log("ボールに向かいます");
            if (myState != PlayerState.Drible&& myState != PlayerState.Freeze)
            { 
                myState = PlayerState.Chase;
                Ball = collision.gameObject.GetComponent<soccerball>();
            }
            /*
            movevec = collision.gameObject.transform.position - this.gameObject.transform.position;
            Vector2 firstdis = new Vector2(this.transform.position.x, this.transform.position.y) - FirstPosition;
            Debug.Log(firstdis.x * firstdis.x / MovableArea.x / MovableArea.x + firstdis.y * firstdis.y / MovableArea.y / MovableArea.y);
            if (firstdis.x * firstdis.x / MovableArea.x / MovableArea.x + firstdis.y * firstdis.y / MovableArea.y / MovableArea.y >= 1)
            {
                movevec = new Vector2(0f, 0f);
                speed = 0f;
            }
            else speed = firstspd;
            movevec.Normalize();
            rb.velocity = movevec * speed;
            */
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Ball = null;
            myState = PlayerState.Back;
            //Debug.Log("視認範囲から出ました");
            /*Vector2 firstdis =- new Vector2(this.transform.position.x, this.transform.position.y) + FirstPosition;
            movevec = new Vector2(firstdis.x,firstdis.y);
            speed = firstspd;
            movevec.Normalize();
            rb.velocity = movevec * speed;*/
        }
    }
}
