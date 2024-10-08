using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class CollideDetector_4 : MonoBehaviour
    {
        public ArticulationBody link1;
        public Material collisionMaterial;
        private Material defaultMaterial;

        private MeshRenderer Renderer_4_0;
        private Collider Collider_4_0;
        private MeshRenderer Renderer_4_1;
        private Collider Collider_4_1;

        private void Start()
        {
            Renderer_4_0 = transform.Find("Visuals/MF1509_4_0/MF1509_4_0/MF1509_4_0").GetComponent<MeshRenderer>();
            Renderer_4_1 = transform.Find("Visuals/MF1509_4_1/MF1509_4_1/MF1509_4_1").GetComponent<MeshRenderer>();
            
            Collider_4_0 = transform.Find("Collisions/MF1509_4_0/MF1509_4_0/MF1509_4_0").GetComponent<Collider>();
            Collider_4_1 = transform.Find("Collisions/MF1509_4_1/MF1509_4_1/MF1509_4_1").GetComponent<Collider>();
            defaultMaterial = Renderer_4_0.material;
        }
        private void OnCollisionEnter(Collision collision)
        {
            string commandName = CommandList.currentCommandName;
            string commandDesiredPosition = CommandList.currentCommandDesiredPosition;
            Debug.Log("Collision Detected. Command Name = " + commandName + ", Desired Position = " + commandDesiredPosition);
        }

        private void OnCollisionStay(Collision collision)
        {
            List<float> jointPositions = new List<float>();
            link1.GetJointPositions(jointPositions);
            for (int i = 0; i < jointPositions.Count; i++)
            {
                jointPositions[i] *= 180 / 3.141592f;
            }
            string jointPositionsStr = string.Join(", ", jointPositions);
            bool is4_0 = false;
            bool is4_1 = false;
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.thisCollider == Collider_4_0)
                {
                    is4_0 = true;
                }
                else if (contact.thisCollider == Collider_4_1)
                {
                    is4_1 = true;
                }

            }
            if (is4_0)
            {
                Renderer_4_0.material = collisionMaterial;
                // Debug.Log("Link4_0 is Collided. Current Joint Positions : " + jointPositionsStr);
            }
            if (is4_1)
            {
                Renderer_4_1.material = collisionMaterial;
                // Debug.Log("Link4_1 is Collided. Current Joint Positions : " + jointPositionsStr);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            Renderer_4_0.material = defaultMaterial;
            Renderer_4_1.material = defaultMaterial;
        }
    }


}