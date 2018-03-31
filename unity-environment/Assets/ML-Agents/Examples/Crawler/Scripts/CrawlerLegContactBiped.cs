using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerLegContactBiped : MonoBehaviour {

    public int index;
    public CrawlerAgentConfigurableBiped agent;

    void Start(){

    }

    void OnCollisionStay(Collision other){
        if (other.gameObject.name == "Platform")
        {
            agent.leg_touching[index] = true;
        }
    }

}
