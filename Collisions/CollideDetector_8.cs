using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class CollideDetector_8 : MonoBehaviour
    {
        public ArticulationBody link1;
        public Material collisionMaterial;
        private Material defaultMaterial;

        private MeshRenderer Renderer_8_0;
        private Collider Collider_8_0;

        private void Start()
        {
            Renderer_8_0 = transform.Find("Visuals/drillbit/drillbit/Drill bit").GetComponent<MeshRenderer>();
            Collider_8_0 = transform.Find("Collisions/drillbit/drillbit/drillbit").GetComponent<Collider>();
            defaultMaterial = Renderer_8_0.material;
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
                if (collider == Collider_8_0)
                {
                    Debug.Log("Change Color");
                    Renderer_8_0.material = collisionMaterial;
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
                if (contact.thisCollider == Collider_8_0)
                {
                    List<float> jointPositions = new List<float>();
                    link1.GetJointPositions(jointPositions);
                    for (int i = 0; i < jointPositions.Count; i++)
                    {
                        jointPositions[i] *= 180 / 3.141592f;
                    }
                    string jointPositionsStr = string.Join(", ", jointPositions);
                    Renderer_8_0.material = collisionMaterial;
                    // Debug.Log("Link6 is Collided. Current Joint Value: " + jointPositionsStr);
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            Renderer_8_0.material = defaultMaterial;
        }
    }


}