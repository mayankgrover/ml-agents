using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerGroundContact : MonoBehaviour {

    public int index;
    // WalkerAgent agent;
    WalkerAgentMotorJoints agent;

    void Start(){
        agent = transform.root.GetComponent<WalkerAgentMotorJoints>();
    }

    void OnCollisionStay(Collision other){
        if (other.transform.CompareTag("ground"))
        {
            agent.leg_touching[index] = true;
        }
    }

}
