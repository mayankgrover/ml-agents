using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerAgentConfigurableBiped : Agent
{

    public float strength;
    float x_position;
    [HideInInspector]
    public bool[] leg_touching;
    [HideInInspector]
    public bool fell;
    Vector3 past_velocity;
    Transform body;
    Rigidbody bodyRB;
    public Transform[] limbs;
    public ConfigurableJoint[] joints;
    public Rigidbody[] limbRBs;
    Dictionary<GameObject, Vector3> transformsPosition;
    Dictionary<GameObject, Quaternion> transformsRotation;
    public float totalCharMass; //total mass of this agent
    public bool visualizeMassDistribution;

    public override void InitializeAgent()
    {
        body = transform.Find("Body");
        bodyRB = body.GetComponent<Rigidbody>();
        transformsPosition = new Dictionary<GameObject, Vector3>();
        transformsRotation = new Dictionary<GameObject, Quaternion>();
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            transformsPosition[child.gameObject] = child.position;
            transformsRotation[child.gameObject] = child.rotation;
        }
        leg_touching = new bool[2];
        limbRBs = new Rigidbody[limbs.Length];
        totalCharMass = 0; //reset to 0
        for (int i = 0; i < limbs.Length; i++)
        {
            limbRBs[i] = limbs[i].gameObject.GetComponent<Rigidbody>();
            joints[i] = limbs[i].gameObject.GetComponent<ConfigurableJoint>();
            // if(limbRBs[i])
            // {
            //     limbRBs[i].maxAngularVelocity = 50;
            //     limbRBs[i].centerOfMass += limbRBs[i].transform.TransformPoint(joints[i].anchor);
            //     totalCharMass += limbRBs[i].mass;
            // }
        }
        for (int i = 0; i < limbs.Length; i++)
        {
            // limbRBs[i] = limbs[i].gameObject.GetComponent<Rigidbody>();
            // joints[i] = limbs[i].gameObject.GetComponent<ConfigurableJoint>();
            if(limbRBs[i])
            {
                limbRBs[i].maxAngularVelocity = 50;
                // if(joints[i])
                // limbRBs[i].centerOfMass += joints[i].anchor;
                limbRBs[i].centerOfMass += Vector3.Scale(joints[i].anchor, limbRBs[i].transform.localScale);
                totalCharMass += limbRBs[i].mass;
            }
        }
    }
    public Quaternion GetJointRotation(ConfigurableJoint joint)
    {
        return(Quaternion.FromToRotation(joint.axis, joint.connectedBody.transform.rotation.eulerAngles));
    }

    public override void CollectObservations()
    {
        // AddVectorObs(body.transform.rotation);
        AddVectorObs(bodyRB.rotation.eulerAngles);
        // AddVectorObs(body.transform.rotation.eulerAngles);

        AddVectorObs(bodyRB.velocity);
        AddVectorObs(limbRBs[8].position.y); //head height
        // AddVectorObs(bodyRB.position.y);

        //let ml handle body part mass
        AddVectorObs(bodyRB.mass);

        // AddVectorObs((bodyRB.velocity - past_velocity) / Time.fixedDeltaTime);
        // past_velocity = bodyRB.velocity;

        for (int i = 0; i < limbs.Length; i++)
        {
            AddVectorObs(limbs[i].localPosition);
            AddVectorObs(limbs[i].localRotation.eulerAngles);
            // print("localrotation: " + limbs[i].localRotation.eulerAngles);
            // AddVectorObs(limbs[i].localRotation);
            AddVectorObs(limbRBs[i].velocity);
            AddVectorObs(limbRBs[i].angularVelocity);

            AddVectorObs(GetJointRotation(joints[i]).eulerAngles); //get the joint rotation
            // print(GetJointRotation(joints[i]).eulerAngles);


            //let ml handle body part mass
            AddVectorObs(limbRBs[i].mass);
        }

        for (int index = 0; index < 2; index++)
        {
            if (leg_touching[index])
            {
                AddVectorObs(1);
            }
            else
            {
                AddVectorObs(0);
            }
            leg_touching[index] = false;
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        for (int k = 0; k < vectorAction.Length; k++)
        {
            vectorAction[k] = Mathf.Clamp(vectorAction[k], -1f, 1f);
        }

        limbRBs[0].AddTorque(-limbs[0].transform.right * strength * vectorAction[0], ForceMode.Force);
        limbRBs[1].AddTorque(-limbs[1].transform.right * strength * vectorAction[1], ForceMode.Force);
        // limbRBs[2].AddTorque(-limbs[2].transform.right * strength * vectorAction[2], ForceMode.VelocityChange);
        // limbRBs[3].AddTorque(-limbs[3].transform.right * strength * vectorAction[3], ForceMode.VelocityChange);
        // limbRBs[0].AddTorque(-limbs[0].transform.forward * strength * vectorAction[4]);
        // limbRBs[1].AddTorque(-limbs[1].transform.forward * strength * vectorAction[5]);
        // limbRBs[2].AddTorque(-limbs[2].transform.forward * strength * vectorAction[6]);
        // limbRBs[3].AddTorque(-limbs[3].transform.forward * strength * vectorAction[7]);
        // limbRBs[0].AddTorque(-limbs[0].transform.forward * strength * vectorAction[2], ForceMode.VelocityChange);
        // limbRBs[1].AddTorque(-limbs[1].transform.forward * strength * vectorAction[3], ForceMode.VelocityChange);
        limbRBs[0].AddTorque(-body.transform.up * strength * vectorAction[2], ForceMode.VelocityChange);
        limbRBs[1].AddTorque(-body.transform.up * strength * vectorAction[3], ForceMode.VelocityChange);
        // limbRBs[2].AddTorque(-body.transform.up * strength * vectorAction[6], ForceMode.VelocityChange);
        // limbRBs[3].AddTorque(-body.transform.up * strength * vectorAction[7], ForceMode.VelocityChange);
        limbRBs[2].AddTorque(-limbs[2].transform.right * strength * vectorAction[4], ForceMode.VelocityChange);
        limbRBs[3].AddTorque(-limbs[3].transform.right * strength * vectorAction[5], ForceMode.VelocityChange);


        limbRBs[6].AddTorque(-limbs[6].transform.forward * strength * vectorAction[6], ForceMode.VelocityChange);
        limbRBs[7].AddTorque(-limbs[7].transform.forward * strength * vectorAction[7], ForceMode.VelocityChange);
        limbRBs[8].AddTorque(-limbs[8].transform.forward * strength * vectorAction[8], ForceMode.VelocityChange);
        limbRBs[6].AddTorque(-limbs[6].transform.up * strength * vectorAction[9], ForceMode.VelocityChange);
        limbRBs[7].AddTorque(-limbs[7].transform.up * strength * vectorAction[10], ForceMode.VelocityChange);
        limbRBs[8].AddTorque(-limbs[8].transform.up * strength * vectorAction[11], ForceMode.VelocityChange);
        // limbRBs[4].AddTorque(-limbs[4].transform.up * strength * vectorAction[7], ForceMode.VelocityChange);
        // limbRBs[6].AddTorque(-limbs[6].transform.right * strength * vectorAction[10], ForceMode.VelocityChange);
        // limbRBs[7].AddTorque(-limbs[7].transform.right * strength * vectorAction[11], ForceMode.VelocityChange);
        
        // limbRBs[0].AddTorque(-limbs[0].transform.right * strength * vectorAction[0]);
        // limbRBs[1].AddTorque(-limbs[1].transform.right * strength * vectorAction[1]);
        // limbRBs[2].AddTorque(-limbs[2].transform.right * strength * vectorAction[2]);
        // limbRBs[3].AddTorque(-limbs[3].transform.right * strength * vectorAction[3]);
        // // limbRBs[0].AddTorque(-limbs[0].transform.forward * strength * vectorAction[4]);
        // // limbRBs[1].AddTorque(-limbs[1].transform.forward * strength * vectorAction[5]);
        // // limbRBs[2].AddTorque(-limbs[2].transform.forward * strength * vectorAction[6]);
        // // limbRBs[3].AddTorque(-limbs[3].transform.forward * strength * vectorAction[7]);
        // limbRBs[0].AddTorque(-body.transform.up * strength * vectorAction[4]);
        // limbRBs[1].AddTorque(-body.transform.up * strength * vectorAction[5]);
        // limbRBs[2].AddTorque(-body.transform.up * strength * vectorAction[6]);
        // limbRBs[3].AddTorque(-body.transform.up * strength * vectorAction[7]);
        // limbRBs[4].AddTorque(-limbs[4].transform.right * strength * vectorAction[8]);
        // limbRBs[5].AddTorque(-limbs[5].transform.right * strength * vectorAction[9]);
        // limbRBs[6].AddTorque(-limbs[6].transform.right * strength * vectorAction[10]);
        // limbRBs[7].AddTorque(-limbs[7].transform.right * strength * vectorAction[11]);





        //let ml handle body part mass
        int actIndex = 12;
        for (int i = 0; i < limbRBs.Length; i++)
        {
            limbRBs[i].mass = Mathf.Clamp(vectorAction[actIndex], 0.1f, 1f) * 20;
            actIndex++;
        }
        bodyRB.mass = Mathf.Clamp(vectorAction[21], 0.1f, 1f) * 20;






        float torque_penalty = vectorAction[0] * vectorAction[0] + 
            vectorAction[1] * vectorAction[1] + 
            vectorAction[2] * vectorAction[2] + 
            vectorAction[3] * vectorAction[3] +
            vectorAction[4] * vectorAction[4] + 
            vectorAction[5] * vectorAction[5] +
            vectorAction[6] * vectorAction[6] +
            vectorAction[7] * vectorAction[7] +
            vectorAction[8] * vectorAction[8] + 
            vectorAction[9] * vectorAction[9] + 
            vectorAction[10] * vectorAction[10] + 
            vectorAction[11] * vectorAction[11]
            ;

        if (!IsDone())
        {
            SetReward(
            0 - 0.01f * torque_penalty 
            // + .5f * limbRBs[8].velocity.x
            + .5f * bodyRB.velocity.x
            // + 1.0f * bodyRB.velocity.x
            + 1.0f * limbRBs[8].position.y //head height
            // + 1f * bodyRB.position.y
            - 0.05f * Mathf.Abs(body.transform.position.z - body.transform.parent.transform.position.z)
            // - 0.05f * Mathf.Abs(bodyRB.velocity.y)
            - 0.05f * Mathf.Abs(bodyRB.angularVelocity.sqrMagnitude)
            );
            
        }
        if (fell)
        {
            Done();
            AddReward(-1f);
        }
    }

    void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            if(visualizeMassDistribution)
            {
                Gizmos.color = new Color(0,1,1,.5f);
                float drawCOMRadius = 0; //our center of mass radius is relative to the mass of the body part's proportional mass vs the whole body
                //limbs
                for (int i = 0; i < limbs.Length; i++)
                {
                    if(limbRBs[i])
                    {
                        drawCOMRadius = limbRBs[i].mass/totalCharMass;
                        // var COMPosition = limbRBs[i].worldCenterOfMass + limbRBs[i].transform.TransformPoint(limbRBs[i].transform.up + joints[i].anchor);
                        // var COMPosition = limbRBs[i].transform.TransformPoint(joints[i].anchor);
                        var COMPosition = limbRBs[i].worldCenterOfMass;
                        // var COMPosition = limbRBs[i].worldCenterOfMass + limbRBs[i].transform.TransformPoint(joints[i].anchor);
                        Gizmos.DrawSphere(COMPosition, drawCOMRadius);

                    }
                }
                // foreach(Rigidbody rb in limbRBs)
                // {
                //     drawCOMRadius = rb.mass/totalCharMass;
                //     Gizmos.DrawSphere(rb.worldCenterOfMass, drawCOMRadius);
                // }
                // body
                if(bodyRB)
                {
                    drawCOMRadius = bodyRB.mass/totalCharMass;
                    Gizmos.DrawSphere(bodyRB.worldCenterOfMass, drawCOMRadius);
                }
            }

        }
    }

    public override void AgentReset()
    {
        fell = false;
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if ((child.gameObject.name.Contains("Crawler"))
                || (child.gameObject.name.Contains("parent")))
            {
                continue;
            }
            child.position = transformsPosition[child.gameObject];
            child.rotation = transformsRotation[child.gameObject];
            if(child.gameObject.GetComponent<Rigidbody>())
            {
                child.gameObject.GetComponent<Rigidbody>().velocity = default(Vector3);
                child.gameObject.GetComponent<Rigidbody>().angularVelocity = default(Vector3);
            }
        }
        // gameObject.transform.rotation *= Quaternion.Euler(new Vector3(0, 90, 0));
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
        // gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, Random.value * 90 - 45, 0));
    }

    public override void AgentOnDone()
    {

    }
}
