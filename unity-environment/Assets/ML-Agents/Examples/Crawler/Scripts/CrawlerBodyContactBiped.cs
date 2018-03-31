using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerBodyContactBiped : MonoBehaviour {

    CrawlerAgentConfigurableBiped agent;

    void Start(){
        agent = gameObject.transform.root.gameObject.GetComponent<CrawlerAgentConfigurableBiped>();
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.name == "Ground")
        {
            agent.fell = true;
        }
    }
}
