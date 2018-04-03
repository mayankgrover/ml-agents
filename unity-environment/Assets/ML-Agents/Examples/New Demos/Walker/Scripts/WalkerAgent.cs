using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerAgent : Agent
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

    public Transform hips;
    public Transform chest;
    public Transform spine;
    public Transform head;
    public Transform thighL;
    public Transform shinL;
    public Transform footL;
    public Transform thighR;
    public Transform shinR;
    public Transform footR;
    public Transform armL;
    public Transform forearmL;
    public Transform handL;
    public Transform armR;
    public Transform forearmR;
    public Transform handR;

    public Dictionary<Transform, BodyPart> bodyParts = new Dictionary<Transform, BodyPart>();

    [System.Serializable]
    public class BodyPart
    {
        public ConfigurableJoint joint;
        public Rigidbody rb;
        public Vector3 startingPos;
        public Quaternion startingRot;
    }

    public void SetupBodyPart(Transform t)
    {
        BodyPart bp = new BodyPart();
        bp.rb = t.GetComponent<Rigidbody>();
        bp.joint = t.GetComponent<ConfigurableJoint>();
        bp.startingPos = t.position;
        bp.startingRot = t.rotation;
        bodyParts.Add(t, bp);
    }

    public override void InitializeAgent()
    {
        SetupBodyPart(hips);
        SetupBodyPart(chest);
        SetupBodyPart(spine);
        SetupBodyPart(head);
        SetupBodyPart(thighL);
        SetupBodyPart(shinL);
        SetupBodyPart(footL);
        SetupBodyPart(thighR);
        SetupBodyPart(shinR);
        SetupBodyPart(footR);
        SetupBodyPart(armL);
        SetupBodyPart(forearmL);
        SetupBodyPart(handL);
        SetupBodyPart(armR);
        SetupBodyPart(forearmR);
        SetupBodyPart(handR);

        // body = transform.Find("Body");
        // bodyRB = body.GetComponent<Rigidbody>();
        // transformsPosition = new Dictionary<GameObject, Vector3>();
        // transformsRotation = new Dictionary<GameObject, Quaternion>();
        // Transform[] allChildren = GetComponentsInChildren<Transform>();
        // foreach (Transform child in allChildren)
        // {
        //     transformsPosition[child.gameObject] = child.position;
        //     transformsRotation[child.gameObject] = child.rotation;
        // }
        leg_touching = new bool[2];
        limbRBs = new Rigidbody[limbs.Length];
        totalCharMass = 0; //reset to 0
        // for (int i = 0; i < limbs.Length; i++)
        // {
        //     limbRBs[i] = limbs[i].gameObject.GetComponent<Rigidbody>();
        //     joints[i] = limbs[i].gameObject.GetComponent<ConfigurableJoint>();
        //     // if(limbRBs[i])
        //     // {
        //     //     limbRBs[i].maxAngularVelocity = 50;
        //     //     limbRBs[i].centerOfMass += limbRBs[i].transform.TransformPoint(joints[i].anchor);
        //     //     totalCharMass += limbRBs[i].mass;
        //     // }
        // }

        foreach(var item in bodyParts)
        {
            if(item.Value.rb)
            {
                  item.Value.rb.maxAngularVelocity = 50;
                // if(joints[i])
                // limbRBs[i].centerOfMass += joints[i].anchor;
                if(item.Value.joint)
                {
                    item.Value.rb.centerOfMass += Vector3.Scale(item.Value.joint.anchor, item.Value.rb.transform.localScale);
                }
                totalCharMass += item.Value.rb.mass;
            }
        }
        // for (int i = 0; i < limbs.Length; i++)
        // {
        //     // limbRBs[i] = limbs[i].gameObject.GetComponent<Rigidbody>();
        //     // joints[i] = limbs[i].gameObject.GetComponent<ConfigurableJoint>();
        //     if(limbRBs[i])
        //     {
        //         limbRBs[i].maxAngularVelocity = 50;
        //         // if(joints[i])
        //         // limbRBs[i].centerOfMass += joints[i].anchor;
        //         limbRBs[i].centerOfMass += Vector3.Scale(joints[i].anchor, limbRBs[i].transform.localScale);
        //         totalCharMass += limbRBs[i].mass;
        //     }
        // }
    }
    public Quaternion GetJointRotation(ConfigurableJoint joint)
    {
        return(Quaternion.FromToRotation(joint.axis, joint.connectedBody.transform.rotation.eulerAngles));
    }

    public override void CollectObservations()
    {

        AddVectorObs(bodyParts[hips].rb.rotation.eulerAngles);
        // AddVectorObs(bodyParts[hips].rb.velocity);
        // AddVectorObs(bodyParts[head].rb.position.y); //head height

        foreach(var item in bodyParts)
        {
                var rb = item.Value.rb;
                // AddVectorObs(item.Key.localPosition);
                AddVectorObs(bodyParts[hips].rb.worldCenterOfMass - item.Value.rb.worldCenterOfMass);
                                        // Gizmos.DrawSphere(item.Value.rb.worldCenterOfMass + (bodyParts[hips].rb.worldCenterOfMass - item.Value.rb.worldCenterOfMass), drawCOMRadius);
                // AddVectorObs(item.Key.localRotation.eulerAngles);
                AddVectorObs(Quaternion.FromToRotation(hips.transform.forward, item.Key.forward).eulerAngles); //can't parent to hips because it skews model so have to do this instead of local rotation
                AddVectorObs(rb.velocity);
                AddVectorObs(rb.angularVelocity);


            if(item.Key != hips)
            {
                AddVectorObs(GetJointRotation(item.Value.joint).eulerAngles); //get the joint rotation
            }
                // print(GetJointRotation(joints[i]).eulerAngles);


                //let ml handle body part mass
                AddVectorObs(rb.mass);

            // }
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
    // public override void CollectObservations()
    // {
    //     // AddVectorObs(body.transform.rotation);
    //     AddVectorObs(bodyRB.rotation.eulerAngles);
    //     // AddVectorObs(body.transform.rotation.eulerAngles);

    //     AddVectorObs(bodyRB.velocity);
    //     AddVectorObs(limbRBs[8].position.y); //head height
    //     // AddVectorObs(bodyRB.position.y);

    //     //let ml handle body part mass
    //     AddVectorObs(bodyRB.mass);

    //     // AddVectorObs((bodyRB.velocity - past_velocity) / Time.fixedDeltaTime);
    //     // past_velocity = bodyRB.velocity;

    //     for (int i = 0; i < limbs.Length; i++)
    //     {
    //         AddVectorObs(limbs[i].localPosition);
    //         AddVectorObs(limbs[i].localRotation.eulerAngles);
    //         // print("localrotation: " + limbs[i].localRotation.eulerAngles);
    //         // AddVectorObs(limbs[i].localRotation);
    //         AddVectorObs(limbRBs[i].velocity);
    //         AddVectorObs(limbRBs[i].angularVelocity);

    //         AddVectorObs(GetJointRotation(joints[i]).eulerAngles); //get the joint rotation
    //         // print(GetJointRotation(joints[i]).eulerAngles);


    //         //let ml handle body part mass
    //         AddVectorObs(limbRBs[i].mass);
    //     }

    //     for (int index = 0; index < 2; index++)
    //     {
    //         if (leg_touching[index])
    //         {
    //             AddVectorObs(1);
    //         }
    //         else
    //         {
    //             AddVectorObs(0);
    //         }
    //         leg_touching[index] = false;
    //     }
    // }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        for (int k = 0; k < vectorAction.Length; k++)
        {
            vectorAction[k] = Mathf.Clamp(vectorAction[k], -1f, 1f);
        }
        ForceMode forceModeToUse = ForceMode.Force;

        bodyParts[thighL].rb.AddTorque(thighL.right * strength * vectorAction[0], forceModeToUse);
        bodyParts[thighR].rb.AddTorque(thighR.right * strength * vectorAction[1], forceModeToUse);
        bodyParts[thighL].rb.AddTorque(thighL.forward * strength * vectorAction[2], forceModeToUse);
        bodyParts[thighR].rb.AddTorque(thighR.forward * strength * vectorAction[3], forceModeToUse);
        bodyParts[shinL].rb.AddTorque(shinL.right * strength * vectorAction[4], forceModeToUse);
        bodyParts[shinR].rb.AddTorque(shinR.right * strength * vectorAction[5], forceModeToUse);
        bodyParts[spine].rb.AddTorque(spine.up * strength * vectorAction[6], forceModeToUse);
        bodyParts[spine].rb.AddTorque(spine.forward * strength * vectorAction[7], forceModeToUse);
        bodyParts[chest].rb.AddTorque(chest.up * strength * vectorAction[8], forceModeToUse);
        bodyParts[chest].rb.AddTorque(chest.forward * strength * vectorAction[9], forceModeToUse);
        bodyParts[head].rb.AddTorque(head.up * strength * vectorAction[10], forceModeToUse);
        bodyParts[head].rb.AddTorque(head.forward * strength * vectorAction[11], forceModeToUse);
        bodyParts[armL].rb.AddTorque(armL.forward * strength * vectorAction[12], forceModeToUse);
        bodyParts[armL].rb.AddTorque(armL.right * strength * vectorAction[13], forceModeToUse);
        bodyParts[armR].rb.AddTorque(armR.forward * strength * vectorAction[14], forceModeToUse);
        bodyParts[armR].rb.AddTorque(armR.right * strength * vectorAction[15], forceModeToUse);
        bodyParts[forearmR].rb.AddTorque(forearmR.right * strength * vectorAction[16], forceModeToUse);
        bodyParts[forearmL].rb.AddTorque(forearmL.right * strength * vectorAction[17], forceModeToUse);

        float torque_penalty = 0; 
        for (int k = 0; k < 17; k++)
        {
            torque_penalty += vectorAction[k] * vectorAction[k];
        }






        //let ml handle body part mass
        int actIndex = 18;
        foreach(var item in bodyParts)
        {
            item.Value.rb.mass = Mathf.Clamp(vectorAction[actIndex], 0.1f, 1f) * 20;
            actIndex++;
        }



        if (!IsDone())
        {
            SetReward(
            0 - 0.01f * torque_penalty 
            // + .5f * limbRBs[8].velocity.x
            + .5f * bodyParts[hips].rb.velocity.x
            // + 1.0f * bodyRB.velocity.x
            + 1.0f * bodyParts[head].rb.position.y //head height
            // + 1f * bodyRB.position.y
            - 0.05f * Mathf.Abs(hips.transform.position.z - hips.transform.parent.transform.position.z)
            // - 0.05f * Mathf.Abs(bodyRB.velocity.y)
            - 0.05f * Mathf.Abs(bodyParts[hips].rb.angularVelocity.sqrMagnitude)
            );
            
        }
        if (fell)
        {
            Done();
            AddReward(-1f);
        }
    }



    // public override void AgentAction(float[] vectorAction, string textAction)
    // {
    //     for (int k = 0; k < vectorAction.Length; k++)
    //     {
    //         vectorAction[k] = Mathf.Clamp(vectorAction[k], -1f, 1f);
    //     }

    //     limbRBs[0].AddTorque(-limbs[0].transform.right * strength * vectorAction[0], ForceMode.Force);
    //     limbRBs[1].AddTorque(-limbs[1].transform.right * strength * vectorAction[1], ForceMode.Force);
    //     // limbRBs[2].AddTorque(-limbs[2].transform.right * strength * vectorAction[2], ForceMode.VelocityChange);
    //     // limbRBs[3].AddTorque(-limbs[3].transform.right * strength * vectorAction[3], ForceMode.VelocityChange);
    //     // limbRBs[0].AddTorque(-limbs[0].transform.forward * strength * vectorAction[4]);
    //     // limbRBs[1].AddTorque(-limbs[1].transform.forward * strength * vectorAction[5]);
    //     // limbRBs[2].AddTorque(-limbs[2].transform.forward * strength * vectorAction[6]);
    //     // limbRBs[3].AddTorque(-limbs[3].transform.forward * strength * vectorAction[7]);
    //     // limbRBs[0].AddTorque(-limbs[0].transform.forward * strength * vectorAction[2], ForceMode.VelocityChange);
    //     // limbRBs[1].AddTorque(-limbs[1].transform.forward * strength * vectorAction[3], ForceMode.VelocityChange);
    //     limbRBs[0].AddTorque(-body.transform.up * strength * vectorAction[2], ForceMode.VelocityChange);
    //     limbRBs[1].AddTorque(-body.transform.up * strength * vectorAction[3], ForceMode.VelocityChange);
    //     // limbRBs[2].AddTorque(-body.transform.up * strength * vectorAction[6], ForceMode.VelocityChange);
    //     // limbRBs[3].AddTorque(-body.transform.up * strength * vectorAction[7], ForceMode.VelocityChange);
    //     limbRBs[2].AddTorque(-limbs[2].transform.right * strength * vectorAction[4], ForceMode.VelocityChange);
    //     limbRBs[3].AddTorque(-limbs[3].transform.right * strength * vectorAction[5], ForceMode.VelocityChange);


    //     limbRBs[6].AddTorque(-limbs[6].transform.forward * strength * vectorAction[6], ForceMode.VelocityChange);
    //     limbRBs[7].AddTorque(-limbs[7].transform.forward * strength * vectorAction[7], ForceMode.VelocityChange);
    //     limbRBs[8].AddTorque(-limbs[8].transform.forward * strength * vectorAction[8], ForceMode.VelocityChange);
    //     limbRBs[6].AddTorque(-limbs[6].transform.up * strength * vectorAction[9], ForceMode.VelocityChange);
    //     limbRBs[7].AddTorque(-limbs[7].transform.up * strength * vectorAction[10], ForceMode.VelocityChange);
    //     limbRBs[8].AddTorque(-limbs[8].transform.up * strength * vectorAction[11], ForceMode.VelocityChange);
    //     // limbRBs[4].AddTorque(-limbs[4].transform.up * strength * vectorAction[7], ForceMode.VelocityChange);
    //     // limbRBs[6].AddTorque(-limbs[6].transform.right * strength * vectorAction[10], ForceMode.VelocityChange);
    //     // limbRBs[7].AddTorque(-limbs[7].transform.right * strength * vectorAction[11], ForceMode.VelocityChange);
        
    //     // limbRBs[0].AddTorque(-limbs[0].transform.right * strength * vectorAction[0]);
    //     // limbRBs[1].AddTorque(-limbs[1].transform.right * strength * vectorAction[1]);
    //     // limbRBs[2].AddTorque(-limbs[2].transform.right * strength * vectorAction[2]);
    //     // limbRBs[3].AddTorque(-limbs[3].transform.right * strength * vectorAction[3]);
    //     // // limbRBs[0].AddTorque(-limbs[0].transform.forward * strength * vectorAction[4]);
    //     // // limbRBs[1].AddTorque(-limbs[1].transform.forward * strength * vectorAction[5]);
    //     // // limbRBs[2].AddTorque(-limbs[2].transform.forward * strength * vectorAction[6]);
    //     // // limbRBs[3].AddTorque(-limbs[3].transform.forward * strength * vectorAction[7]);
    //     // limbRBs[0].AddTorque(-body.transform.up * strength * vectorAction[4]);
    //     // limbRBs[1].AddTorque(-body.transform.up * strength * vectorAction[5]);
    //     // limbRBs[2].AddTorque(-body.transform.up * strength * vectorAction[6]);
    //     // limbRBs[3].AddTorque(-body.transform.up * strength * vectorAction[7]);
    //     // limbRBs[4].AddTorque(-limbs[4].transform.right * strength * vectorAction[8]);
    //     // limbRBs[5].AddTorque(-limbs[5].transform.right * strength * vectorAction[9]);
    //     // limbRBs[6].AddTorque(-limbs[6].transform.right * strength * vectorAction[10]);
    //     // limbRBs[7].AddTorque(-limbs[7].transform.right * strength * vectorAction[11]);





    //     //let ml handle body part mass
    //     int actIndex = 12;
    //     for (int i = 0; i < limbRBs.Length; i++)
    //     {
    //         limbRBs[i].mass = Mathf.Clamp(vectorAction[actIndex], 0.1f, 1f) * 20;
    //         actIndex++;
    //     }
    //     bodyRB.mass = Mathf.Clamp(vectorAction[21], 0.1f, 1f) * 20;






    //     float torque_penalty = vectorAction[0] * vectorAction[0] + 
    //         vectorAction[1] * vectorAction[1] + 
    //         vectorAction[2] * vectorAction[2] + 
    //         vectorAction[3] * vectorAction[3] +
    //         vectorAction[4] * vectorAction[4] + 
    //         vectorAction[5] * vectorAction[5] +
    //         vectorAction[6] * vectorAction[6] +
    //         vectorAction[7] * vectorAction[7] +
    //         vectorAction[8] * vectorAction[8] + 
    //         vectorAction[9] * vectorAction[9] + 
    //         vectorAction[10] * vectorAction[10] + 
    //         vectorAction[11] * vectorAction[11]
    //         ;

    //     if (!IsDone())
    //     {
    //         SetReward(
    //         0 - 0.01f * torque_penalty 
    //         // + .5f * limbRBs[8].velocity.x
    //         + .5f * bodyRB.velocity.x
    //         // + 1.0f * bodyRB.velocity.x
    //         + 1.0f * limbRBs[8].position.y //head height
    //         // + 1f * bodyRB.position.y
    //         - 0.05f * Mathf.Abs(body.transform.position.z - body.transform.parent.transform.position.z)
    //         // - 0.05f * Mathf.Abs(bodyRB.velocity.y)
    //         - 0.05f * Mathf.Abs(bodyRB.angularVelocity.sqrMagnitude)
    //         );
            
    //     }
    //     if (fell)
    //     {
    //         Done();
    //         AddReward(-1f);
    //     }
    // }

    void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            if(visualizeMassDistribution)
            {
                // Gizmos.color = new Color(0,1,1,.5f);
                float drawCOMRadius = 0; //our center of mass radius is relative to the mass of the body part's proportional mass vs the whole body
                totalCharMass = 0;
                foreach(var item in bodyParts)
                {
                    if(item.Value.rb)
                    {
                        totalCharMass += item.Value.rb.mass;
                    }
                }
                foreach(var item in bodyParts)
                {
                    if(item.Value.rb)
                    {
                        Gizmos.color = new Color(0,1,1,.5f);
                        drawCOMRadius = item.Value.rb.mass/totalCharMass;
                        // var COMPosition = limbRBs[i].worldCenterOfMass + limbRBs[i].transform.TransformPoint(limbRBs[i].transform.up + joints[i].anchor);
                        // var COMPosition = limbRBs[i].transform.TransformPoint(joints[i].anchor);
                        var COMPosition = item.Value.rb.worldCenterOfMass;
                        // var COMPosition = limbRBs[i].worldCenterOfMass + limbRBs[i].transform.TransformPoint(joints[i].anchor);
                        Gizmos.DrawSphere(COMPosition, drawCOMRadius);

                        // Gizmos.color = Color.red;
                        // // Gizmos.DrawSphere(bodyParts[hips].rb.worldCenterOfMass + (bodyParts[hips].rb.worldCenterOfMass - item.Value.rb.worldCenterOfMass), drawCOMRadius);
                        // Gizmos.DrawSphere(item.Value.rb.worldCenterOfMass + (bodyParts[hips].rb.worldCenterOfMass - item.Value.rb.worldCenterOfMass), drawCOMRadius);
                        // Gizmos.DrawSphere(item.Key.transform.TransformPoint(bodyParts[hips].rb.worldCenterOfMass), drawCOMRadius);
                        // Gizmos.DrawSphere(item.Value.rb.position, drawCOMRadius);
                        // Gizmos.DrawSphere(bodyParts[hips].rb.worldCenterOfMass, drawCOMRadius);


                    }
                }
                
                
                
                // //limbs
                // for (int i = 0; i < limbs.Length; i++)
                // {
                //     if(limbRBs[i])
                //     {

                //     }
                // }
                // // foreach(Rigidbody rb in limbRBs)
                // // {
                // //     drawCOMRadius = rb.mass/totalCharMass;
                // //     Gizmos.DrawSphere(rb.worldCenterOfMass, drawCOMRadius);
                // // }
                // // body
                // if(bodyRB)
                // {
                //     drawCOMRadius = bodyRB.mass/totalCharMass;
                //     Gizmos.DrawSphere(bodyRB.worldCenterOfMass, drawCOMRadius);
                // }
            }

        }
    }
    // void OnDrawGizmos()
    // {
    //     if(Application.isPlaying)
    //     {
    //         if(visualizeMassDistribution)
    //         {
    //             Gizmos.color = new Color(0,1,1,.5f);
    //             float drawCOMRadius = 0; //our center of mass radius is relative to the mass of the body part's proportional mass vs the whole body
    //             //limbs
    //             for (int i = 0; i < limbs.Length; i++)
    //             {
    //                 if(limbRBs[i])
    //                 {
    //                     drawCOMRadius = limbRBs[i].mass/totalCharMass;
    //                     // var COMPosition = limbRBs[i].worldCenterOfMass + limbRBs[i].transform.TransformPoint(limbRBs[i].transform.up + joints[i].anchor);
    //                     // var COMPosition = limbRBs[i].transform.TransformPoint(joints[i].anchor);
    //                     var COMPosition = limbRBs[i].worldCenterOfMass;
    //                     // var COMPosition = limbRBs[i].worldCenterOfMass + limbRBs[i].transform.TransformPoint(joints[i].anchor);
    //                     Gizmos.DrawSphere(COMPosition, drawCOMRadius);

    //                 }
    //             }
    //             // foreach(Rigidbody rb in limbRBs)
    //             // {
    //             //     drawCOMRadius = rb.mass/totalCharMass;
    //             //     Gizmos.DrawSphere(rb.worldCenterOfMass, drawCOMRadius);
    //             // }
    //             // body
    //             if(bodyRB)
    //             {
    //                 drawCOMRadius = bodyRB.mass/totalCharMass;
    //                 Gizmos.DrawSphere(bodyRB.worldCenterOfMass, drawCOMRadius);
    //             }
    //         }

    //     }
    // }

    public override void AgentReset()
    {
        fell = false;
        foreach(var item in bodyParts)
        {
            item.Key.position = item.Value.startingPos;
            item.Key.rotation = item.Value.startingRot;
            item.Value.rb.velocity = Vector3.zero;
            item.Value.rb.angularVelocity = Vector3.zero;
        }
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));





        // Transform[] allChildren = GetComponentsInChildren<Transform>();
        // foreach (Transform child in allChildren)
        // {
        //     if ((child.gameObject.name.Contains("Crawler"))
        //         || (child.gameObject.name.Contains("parent")))
        //     {
        //         continue;
        //     }
        //     child.position = transformsPosition[child.gameObject];
        //     child.rotation = transformsRotation[child.gameObject];
        //     if(child.gameObject.GetComponent<Rigidbody>())
        //     {
        //         child.gameObject.GetComponent<Rigidbody>().velocity = default(Vector3);
        //         child.gameObject.GetComponent<Rigidbody>().angularVelocity = default(Vector3);
        //     }
        // }
        // // gameObject.transform.rotation *= Quaternion.Euler(new Vector3(0, 90, 0));
        // gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
        // // gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, Random.value * 90 - 45, 0));
    }

    public override void AgentOnDone()
    {

    }
}
