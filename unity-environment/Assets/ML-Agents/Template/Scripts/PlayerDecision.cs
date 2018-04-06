using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDecision : MonoBehaviour, Decision
{
    public float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        throw new NotImplementedException();
    }

    public List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        throw new NotImplementedException();
    }
}
