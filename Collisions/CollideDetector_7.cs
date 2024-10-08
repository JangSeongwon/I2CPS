using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class CollideDetector_7 : MonoBehaviour
    {
        public ArticulationBody link1;
        public Material collisionMaterial;
        private Material defaultMaterial;

        private MeshRenderer Renderer_7_0;
        private Collider Collider_7_0;

        private void Start()
        {
            Renderer_7_0 = transform.Find("Visuals/6ftsensor_cy/6ftsensor_cy/6ftsensor_cy").GetComponent<MeshRenderer>();
            Collider_7_0 = transform.Find("Collisions/6ftsensor_cy/6ftsensor_cy/6ftsensor_cy").GetComponent<Collider>();
            defaultMaterial = Renderer_7_0.material;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            string commandName = CommandList.currentCommandName;
            string commandDesiredPosition = CommandList.currentCommandDesiredPosition;
            Debug.Log("Collision Detected. Command Name = " + commandName + ", Desired Position = " + commandDesiredPosition);
        }
        private void OnTriggerStay(Collider other)
        {
            Collider[] colliders = other.gameObject.GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collider == Collider_7_0)
                {
                    Debug.Log("Change Color");
                    Renderer_7_0.material = collisionMaterial;
                }
            }
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
                if (contact.thisCollider == Collider_7_0)
                {
                    List<float> jointPositions = new List<float>();
                    link1.GetJointPositions(jointPositions);
                    for (int i = 0; i < jointPositions.Count; i++)
                    {
                        jointPositions[i] *= 180 / 3.141592f;
                    }
                    string jointPositionsStr = string.Join(", ", jointPositions);
                    Renderer_7_0.material = collisionMaterial;
                    // Debug.Log("Link6 is Collided. Current Joint Value: " + jointPositionsStr);
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            Renderer_7_0.material = defaultMaterial;
        }
    }


}