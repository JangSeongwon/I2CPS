using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RosSharp.RosBridgeClient
{
    public class CollideDetector_1 : MonoBehaviour
    {
        public Material collisionMaterial;
        private Material defaultMaterial;
        public ArticulationBody link1;
        private MeshRenderer Renderer_1_0;
        private Collider Collider_1_0;


        private void Start()
        {
            Renderer_1_0 = transform.Find("Visuals/MF1509_1_0/MF1509_1_0/MF1509_1_0").GetComponent<MeshRenderer>();
            Collider_1_0 = transform.Find("Collisions/MF1509_1_0/MF1509_1_0/MF1509_1_0").GetComponent<Collider>();
            defaultMaterial = Renderer_1_0.material;
        }
        private void OnCollisionEnter(Collision collision)
        {
            string commandName = CommandList.currentCommandName;
            string commandDesiredPosition = CommandList.currentCommandDesiredPosition;
            Debug.Log("Collision Detected. Command Name = "+commandName+", Desired Position = "+commandDesiredPosition);
        }

        private void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.thisCollider == Collider_1_0)
                {
                    Renderer_1_0.material = collisionMaterial;
                }
            }
            List<float> JointPositions = new List<float>();
            link1.GetJointPositions(JointPositions);
            for (int i = 0; i < JointPositions.Count; i++)
            {
                JointPositions[i] *= 180 / 3.141592f;
            }
            string jointPositionsStr = string.Join(", ", JointPositions);
            // Debug.Log("Link1 is Collided. Current Joint Positions : " + jointPositionsStr);
        }

        private void OnCollisionExit(Collision collision)
        {
            Renderer_1_0.material = defaultMaterial;
        }
    }


}