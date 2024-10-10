using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class CollideDetector_3 : MonoBehaviour
    {
        public ArticulationBody link1;
        public Material collisionMaterial;
        private Material defaultMaterial;

        private MeshRenderer Renderer_3_0;
        private Collider Collider_3_0;

        private void Start()
        {
            Renderer_3_0 = transform.Find("Visuals/MF1509_3_0/MF1509_3_0/MF1509_3_0").GetComponent<MeshRenderer>();
            Collider_3_0 = transform.Find("Collisions/MF1509_3_0/MF1509_3_0/MF1509_3_0").GetComponent<Collider>();
            defaultMaterial = Renderer_3_0.material;

        }
        private void OnCollisionEnter(Collision collision)
        {
            string commandName = CommandList.currentCommandName;
            string commandDesiredPosition = CommandList.currentCommandDesiredPosition;
            Debug.Log("Collision Detected. Command Name = " + commandName + ", Desired Position = " + commandDesiredPosition);
        }

        private void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.thisCollider == Collider_3_0)
                {
                    List<float> jointPositions = new List<float>();
                    link1.GetJointPositions(jointPositions);
                    for (int i = 0; i < jointPositions.Count; i++)
                    {
                        jointPositions[i] *= 180 / 3.141592f;
                    }
                    string jointPositionsStr = string.Join(", ", jointPositions);
                    Renderer_3_0.material = collisionMaterial;
                    // Debug.Log("Link3 is Collided. Current Joint Value: " + jointPositionsStr);
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            Renderer_3_0.material = defaultMaterial;
        }
    }


}