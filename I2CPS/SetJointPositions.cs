using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetJointPositions : MonoBehaviour
{
    public ArticulationBody joint1;
    public ArticulationBody endEffector;
    public List<float> jointPositions = new List<float>();
    // Start is called before the first frame update
    void Start()
    {
        jointPositions = new List<float>
        {
            (float)0, (float)0,(float)0, (float)0, (float)0, (float)0
        };
    }

    // Update is called once per frame
    void Update()
    {
        endEffector.SetJointPositions(jointPositions);
    }
}
