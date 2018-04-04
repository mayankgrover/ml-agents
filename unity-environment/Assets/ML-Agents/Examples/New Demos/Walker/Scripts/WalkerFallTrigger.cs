using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerFallTrigger : MonoBehaviour {

    // WalkerAgent agent;
    WalkerAgentMotorJoints agent;

    void Start(){
        agent = transform.root.gameObject.GetComponent<WalkerAgentMotorJoints>();
    }

    void OnTriggerEnter(Collider other){
        if (other.transform.CompareTag("ground"))
        {
            agent.fell = true;
        }
    }
}
