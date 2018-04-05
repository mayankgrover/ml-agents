using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyBananaAgent : Agent
{
    public GameObject myAcademyObj;
    BananaAcademy myAcademy;
    public GameObject area;
    BananaArea myArea;
    bool frozen;
    bool poisioned;
    bool satiated;
    bool shoot;
    float frozenTime;
    float effectTime;
    Rigidbody agentRB;

    // Speed of agent rotation.
    public float turnSpeed = 300;

    // Speed of agent movement.
    public float moveSpeed = 2;
    public Material normalMaterial;
    public Material badMaterial;
    public Material goodMaterial;
    int bananas;
    public bool contribute;
    RayPerception rayPer;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        agentRB = GetComponent<Rigidbody>();
        Monitor.verticalOffset = 1f;
        myArea = area.GetComponent<BananaArea>();
        rayPer = GetComponent<RayPerception>();
        myAcademy = myAcademyObj.GetComponent<BananaAcademy>();
    }

    public override void CollectObservations()
    {
        
        
    }

    public Color32 ToColor(int HexVal)
    {
        byte R = (byte)((HexVal >> 16) & 0xFF);
        byte G = (byte)((HexVal >> 8) & 0xFF);
        byte B = (byte)((HexVal) & 0xFF);
        return new Color32(R, G, B, 255);
    }

    public void MoveAgent(float[] act)
    {
        if (Time.time > effectTime + 0.5f)
        {
            if (poisioned)
            {
                Unpoison();
            }
            if (satiated)
            {
                Unsatiate();
            }
        }

        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;


        if (!frozen)
        {
            switch ((int)act[0])
            {
                case 1:
                    dirToGo = transform.forward;
                    break;
                case 2:
                    dirToGo = -transform.forward;
                    break;
                case 3:
                    rotateDir = -transform.up;
                    break;
                case 4:
                    rotateDir = transform.up;
                    break;
            }
            
            agentRB.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
            transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
        }

        if (agentRB.velocity.sqrMagnitude > 25f) // slow it down
        {
            agentRB.velocity *= 0.85f;
        }
    }


    void Poison()
    {
        poisioned = true;
        effectTime = Time.time;
        gameObject.GetComponent<Renderer>().material = badMaterial;
    }

    void Unpoison()
    {
        poisioned = false;
        gameObject.GetComponent<Renderer>().material = normalMaterial;
    }

    void Satiate()
    {
        satiated = true;
        effectTime = Time.time;
        gameObject.GetComponent<Renderer>().material = goodMaterial;
    }

    void Unsatiate()
    {
        satiated = false;
        gameObject.GetComponent<Renderer>().material = normalMaterial;
    }



    public override void AgentAction(float[] vectorAction, string textAction)
    {
        MoveAgent(vectorAction);
    }

    public override void AgentReset()
    {
        Unpoison();
        Unsatiate();
        shoot = false;
        agentRB.velocity = Vector3.zero;
        bananas = 0;
        transform.position = new Vector3(Random.Range(-myArea.range, myArea.range),
                                         2f, Random.Range(-myArea.range, myArea.range))
            + area.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("banana"))
        {
            Satiate();
            collision.gameObject.GetComponent<BananaLogic>().OnEaten();
            AddReward(1f);
            bananas += 1;
            if (contribute)
            {
                myAcademy.totalScore += 1;
            }
        }
        
        if (collision.gameObject.CompareTag("badBanana"))
        {
            Poison();
            collision.gameObject.GetComponent<BananaLogic>().OnEaten();

            AddReward(-1f);
            if (contribute)
            {
                myAcademy.totalScore -= 1;
            }
        }
    }

    public override void AgentOnDone()
    {

    }
}
