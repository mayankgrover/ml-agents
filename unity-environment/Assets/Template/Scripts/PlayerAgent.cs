using UnityEngine;

public class PlayerAgent : Agent
{
    public GameObject target;
    public GameObject area;

    public float minX, maxX;
    public float minZ, maxZ;

    PlayerAcademy academy;
    Rigidbody agentRB;
    RayPerception rayPerception;

    private string[] detectableObjects;
    private float[] rayAngles;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        academy = FindObjectOfType<PlayerAcademy>();
        agentRB = GetComponent<Rigidbody>();
        rayPerception = GetComponent<RayPerception>();
        detectableObjects = new string[] { "Finish", "Obstacle" };
        rayAngles = new float[] { 20f, 60f, 90f, 120f, 160f };
    }

    public override void CollectObservations()
    {
        AddVectorObs((float)GetStepCount() / (float)agentParameters.maxStep);
        AddVectorObs(rayPerception.Perceive(4f, rayAngles, detectableObjects, 0f, 0f));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-1f/ agentParameters.maxStep);
        if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {
            Vector3 dirToGo = Vector3.zero;
            Vector3 rotateDir = Vector3.zero;

            int action = Mathf.FloorToInt(vectorAction[0]);

            // 1 - Up, 2 - Down, 3 - Right, 4 - Left
            if (action == 3) rotateDir = transform.up * -1f;
            else if (action == 2) rotateDir = transform.up * 1f;
            else if (action == 1) dirToGo = transform.forward * -1f;
            else if (action == 0) dirToGo = transform.forward *  1f;

            transform.Rotate(rotateDir, Time.deltaTime * academy.agentRotateSpeed);
            agentRB.AddForce(dirToGo * academy.agentRunSpeed, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Finish"))
        {
            SetReward(1f);
            Done();
            Debug.LogFormat("[AgentStep.Target] world:{0} steps:{1} reward:{2} cummu:{3}",
                transform.parent.name, GetStepCount(), GetReward(), GetCumulativeReward());
        }
        else if(collision.gameObject.CompareTag("Obstacle"))
        {
            SetReward(-1f);
            Done();
            Debug.LogFormat("[AgentStep.Obstacle] world:{0} steps:{1} reward:{2} cummu:{3}",
                transform.parent.name, GetStepCount(), GetReward(), GetCumulativeReward());
        }
    }

    public override void AgentReset()
    {
        //Debug.LogFormat("[AgentStep] world:{0} reset", transform.parent.name);
        transform.localPosition = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        agentRB.velocity *= 0f;
        target.transform.localPosition = new Vector3(Random.Range(minX, maxX), target.transform.position.y, Random.Range(minZ, maxZ));
    }
}
