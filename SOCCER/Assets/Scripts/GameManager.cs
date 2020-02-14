using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager Manager;
    public Vector2 RedGoalPos;
    public Vector2 BlueGoalPos;
    public GameObject RedGoal;
    public GameObject BlueGoal;
    List<player> RedTeam = new List<player>();
    List<player> BlueTeam = new List<player>();
    private float StageLength;
    // Start is called before the first frame update
    void Start()
    {
        Manager = this.gameObject.GetComponent<GameManager>();
        RedGoalPos = new Vector2(RedGoal.transform.position.x, RedGoal.transform.position.y);
        BlueGoalPos = new Vector2(BlueGoal.transform.position.x, BlueGoal.transform.position.y);
        StageLength = 16;
        PlayerPut();
    }
    void PlayerPut()
    {
        for (int i=0;i<6;i++)
        {
            GameObject obj=(GameObject)Resources.Load("player");
            GameObject ins = Instantiate(obj);
            RedTeam.Add(ins.GetComponent<player>());
            RedTeam[i].SetTeam(1);
            RedTeam[i].gameObject.transform.position = new Vector3(Random.Range(-7.0f, 7.0f), Random.Range(-4.0f, 4.0f));
        }
        for (int i = 0; i < 6; i++)
        {
            GameObject obj = (GameObject)Resources.Load("player");
            GameObject ins = Instantiate(obj);
            BlueTeam.Add(ins.GetComponent<player>());
            BlueTeam[i].SetTeam(2);
            BlueTeam[i].gameObject.transform.position = new Vector3(Random.Range(-7.0f, 7.0f), Random.Range(-4.0f, 4.0f));
        }
    }
    public float DribleCheck(player me,float driblegoodness,float driblespdrate,float speed,ref Vector2 driblevec)
    {
        if (me.myTeam == player.Team.Red)
        {
            float maxpoint = 0;
            int maxindex=0;
            for (int i=0;i<32;i++)
            {
                float point = 0;
                Vector2 temppos=new Vector2(me.gameObject.transform.position.x+Mathf.Cos(i * 3.14f / 16) * 0.3f,me.gameObject.transform.position.y+ Mathf.Sin(i * 3.14f / 16) * 0.3f);
                for (int j=0;j<6;j++)
                {
                    Vector2 enemypos=new Vector2(BlueTeam[j].transform.position.x,BlueTeam[j].transform.position.y);
                    float temp=Vector2.Distance(temppos, enemypos);
                    point += 1 / (temp * temp);
                }
                float megoaldis = Vector2.Distance(me.transform.position,RedGoalPos);
                float drigoaldis = Vector2.Distance(temppos,RedGoalPos);
                point = 20 / point;
                point += (megoaldis - drigoaldis) / StageLength*100;
                if(point>maxpoint)
                {
                    maxindex = i;
                    maxpoint = point;
                }
            }
            driblevec = new Vector2(Mathf.Cos(maxindex * 3.14f / 16) * 0.3f, Mathf.Sin(maxindex * 3.14f / 16) * 0.3f);
            return maxpoint;
        }
        else
        {
            float maxpoint = 0;
            int maxindex = 0;
            for (int i = 0; i < 32; i++)
            {
                float point = 0;
                Vector2 temppos = new Vector2(me.gameObject.transform.position.x + Mathf.Cos(i * 3.14f / 16) * 0.3f, me.gameObject.transform.position.y + Mathf.Sin(i * 3.14f / 16) * 0.3f);
                for (int j = 0; j < 6; j++)
                {
                    Vector2 enemypos = new Vector2(RedTeam[j].transform.position.x, RedTeam[j].transform.position.y);
                    float temp = Vector2.Distance(temppos, enemypos);
                    point += 1 / (temp * temp);
                }
                float megoaldis = Vector2.Distance(me.transform.position, BlueGoalPos);
                float drigoaldis = Vector2.Distance(temppos, BlueGoalPos);
                point = 20 / point;
                point += (megoaldis - drigoaldis) / StageLength * 100;
                if (point > maxpoint)
                {
                    maxindex = i;
                    maxpoint = point;
                }
            }
            driblevec = new Vector2(Mathf.Cos(maxindex * 3.14f / 16) * 0.3f, Mathf.Sin(maxindex * 3.14f / 16) * 0.3f);
            return maxpoint;
        }

    }
    public float PassCheck(player me,ref player partytarget,float kickgoodness,float kickpower)
    {
        if(me.myTeam==player.Team.Red)
        {
            float maxpoint = 0;
            int maxindex = 0;
            for (int i = 0; i < 6; i++)
            {
                if(RedTeam[i]!=me)
                {
                    float minval = 100000;
                    Vector2 tarvec = new Vector2(RedTeam[i].gameObject.transform.position.x- me.gameObject.transform.position.x,
                         RedTeam[i].gameObject.transform.position.y - me.gameObject.transform.position.y);
                    for (int j=0;j<6;j++)
                    {
                        Vector2 enemyvec = new Vector2(BlueTeam[j].gameObject.transform.position.x- me.gameObject.transform.position.x,
                         BlueTeam[j].gameObject.transform.position.y - me.gameObject.transform.position.y);
                        float cos = Vector2.Dot(tarvec, enemyvec)/ (tarvec.sqrMagnitude * enemyvec.sqrMagnitude);
                        if (cos == 0) continue;
                        float tan = Mathf.Sqrt(1/(cos*cos)-1);
                        if(cos>0)
                        {
                            if (tan < minval) minval = tan*Mathf.Sqrt(enemyvec.magnitude);
                        }
                    }
                    float passgoaldis = Vector2.SqrMagnitude(new Vector2(RedTeam[i].gameObject.transform.position.x-RedGoalPos.x, RedTeam[i].gameObject.transform.position.y - RedGoalPos.y));
                    float megoaldis = Vector2.SqrMagnitude(new Vector2(me.gameObject.transform.position.x - RedGoalPos.x, me.gameObject.transform.position.y - RedGoalPos.y));
                    minval += (passgoaldis-megoaldis) / StageLength;
                    if (minval>maxpoint)
                    {
                        maxindex = i;
                        maxpoint = minval;
                    }
                }
            }
            partytarget = RedTeam[maxindex];
            return maxpoint*kickgoodness;
        }
        else
        {
            float maxpoint = 0;
            int maxindex = 0;
            for (int i = 0; i < 6; i++)
            {
                if (BlueTeam[i] != me)
                {
                    float minval = 100000;
                    Vector2 tarvec = new Vector2(BlueTeam[i].gameObject.transform.position.x - me.gameObject.transform.position.x,
                         BlueTeam[i].gameObject.transform.position.y - me.gameObject.transform.position.y);
                    for (int j = 0; j < 6; j++)
                    {
                        Vector2 enemyvec = new Vector2(RedTeam[j].gameObject.transform.position.x - me.gameObject.transform.position.x,
                         RedTeam[j].gameObject.transform.position.y - me.gameObject.transform.position.y);
                        float cos = Vector2.Dot(tarvec, enemyvec) / (tarvec.sqrMagnitude * enemyvec.sqrMagnitude);
                        if (cos == 0) continue;
                        float tan = Mathf.Sqrt(1 / (cos * cos) - 1);
                        if (cos > 0)
                        {
                            if (tan < minval) minval = tan * Mathf.Sqrt(enemyvec.magnitude);
                        }
                    }
                    float passgoaldis = Vector2.SqrMagnitude(new Vector2(RedTeam[i].gameObject.transform.position.x - RedGoalPos.x, RedTeam[i].gameObject.transform.position.y - RedGoalPos.y));
                    float megoaldis = Vector2.SqrMagnitude(new Vector2(me.gameObject.transform.position.x - RedGoalPos.x, me.gameObject.transform.position.y - RedGoalPos.y));
                    minval += (passgoaldis - megoaldis) / StageLength;
                    if (minval > maxpoint)
                    {
                        maxindex = i;
                        maxpoint = minval;
                    }
                }
            }
            partytarget = BlueTeam[maxindex];
            return maxpoint*kickgoodness*kickgoodness;
        }
    }
    public float ShootCheck(player me,float kickgoodness,float kickpower,ref Vector2 kickvec)
    {
        if (me.myTeam == player.Team.Red)
        {
            float maxpoint = 0;
            int maxindex = 0;
            for (int i = 0; i < 3; i++)
            {
                float minval = 100000;
                Vector2 tarvec = new Vector2(RedGoalPos.x - me.gameObject.transform.position.x,
                        RedGoalPos.y+i-1 - me.gameObject.transform.position.y);
                for (int j = 0; j < 6; j++)
                {
                    Vector2 enemyvec = new Vector2(BlueTeam[j].gameObject.transform.position.x - me.gameObject.transform.position.x,
                        BlueTeam[j].gameObject.transform.position.y - me.gameObject.transform.position.y);
                    float cos = Vector2.Dot(tarvec, enemyvec) / (tarvec.sqrMagnitude * enemyvec.sqrMagnitude);
                    if (cos == 0) continue;
                    float tan = Mathf.Sqrt(1 / (cos * cos) - 1);
                    if (cos > 0)
                    {
                        if (tan < minval) minval = tan;
                    }
                }
                float megoaldis = Vector2.SqrMagnitude(new Vector2(me.gameObject.transform.position.x - RedGoalPos.x, me.gameObject.transform.position.y - RedGoalPos.y));
                minval -=( megoaldis / StageLength)* (megoaldis / StageLength);
                if (minval > maxpoint)
                {
                    maxindex = i;
                    maxpoint = minval;
                }
            }
            kickvec = new Vector2(RedGoalPos.x - me.gameObject.transform.position.x,
                        RedGoalPos.y + maxindex - 1 - me.gameObject.transform.position.y);
            kickvec.Normalize();
            return maxpoint*kickgoodness*kickpower;
        }
        else
        {
            float maxpoint = 0;
            int maxindex = 0;
            for (int i = 0; i < 3; i++)
            {
                float minval = 100000;
                Vector2 tarvec = new Vector2(BlueGoalPos.x - me.gameObject.transform.position.x,
                        BlueGoalPos.y + i - 1 - me.gameObject.transform.position.y);
                for (int j = 0; j < 6; j++)
                {
                    Vector2 enemyvec = new Vector2(RedTeam[j].gameObject.transform.position.x - me.gameObject.transform.position.x,
                        RedTeam[j].gameObject.transform.position.y - me.gameObject.transform.position.y);
                    float cos = Vector2.Dot(tarvec, enemyvec) / (tarvec.sqrMagnitude * enemyvec.sqrMagnitude);
                    if (cos == 0) continue;
                    float tan = Mathf.Sqrt(1 / (cos * cos) - 1);
                    if (cos > 0)
                    {
                        if (tan < minval) minval = tan;
                    }
                }
                float megoaldis = Vector2.SqrMagnitude(new Vector2(me.gameObject.transform.position.x - RedGoalPos.x, me.gameObject.transform.position.y - RedGoalPos.y));
                minval -= (megoaldis / StageLength) * (megoaldis / StageLength);
                if (minval > maxpoint)
                {
                    maxindex = i;
                    maxpoint = minval;
                }
            }
            kickvec = new Vector2(BlueGoalPos.x - me.gameObject.transform.position.x,
                        BlueGoalPos.y + maxindex - 1 - me.gameObject.transform.position.y);
            kickvec.Normalize();
            return maxpoint * kickgoodness * kickpower;
        }
    }
    public 
    // Update is called once per frame
    void Update()
    {
        
    }
}
