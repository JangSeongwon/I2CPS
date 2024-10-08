using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class CollideDetector_2 : MonoBehaviour
    {
        public Material collisionMaterial;
        public ArticulationBody link1;

        private Material defaultMaterial_0;
        private Material defaultMaterial_1;
        private MeshRenderer Renderer_2_0;
        private MeshRenderer Renderer_2_1;
        private MeshRenderer Renderer_2_2;
        private Collider Collider_2_0;
        private Collider Collider_2_1;
        private Collider Collider_2_2;

        private void Start()
        {
            Renderer_2_0 = transform.Find("Visuals/MF1509_2_0/MF1509_2_0/MF1509_2_0").GetComponent<MeshRenderer>();
            Renderer_2_1 = transform.Find("Visuals/MF1509_2_1/MF1509_2_1/MF1509_2_1").GetComponent<MeshRenderer>();
            Renderer_2_2 = transform.Find("Visuals/MF1509_2_2/MF1509_2_2/MF1509_2_2").GetComponent<MeshRenderer>();
            Collider_2_0 = transform.Find("Collisions/MF1509_2_0/MF1509_2_0/MF1509_2_0").GetComponent<Collider>();
            Collider_2_1 = transform.Find("Collisions/MF1509_2_1/MF1509_2_1/MF1509_2_1").GetComponent<Collider>();
            Collider_2_2 = transform.Find("Collisions/MF1509_2_2/MF1509_2_2/MF1509_2_2").GetComponent<Collider>();
            defaultMaterial_0 = Renderer_2_0.material;
            defaultMaterial_1 = Renderer_2_1.material;
        }
        private void OnCollisionEnter(Collision collision)
        {
            string commandName = CommandList.currentCommandName;
            string commandDesiredPosition = CommandList.currentCommandDesiredPosition;
            Debug.Log("Collision Detected. Command Name = " + commandName + ", Desired Position = " + commandDesiredPosition);
        }
       

        private void OnCollisionStay(Collision collision)
        {
            List<float> JointPositions = new List<float>();
            link1.GetJointPositions(JointPositions);
            for (int i = 0; i < JointPositions.Count; i++)
            {
                JointPositions[i] *= 180 / 3.141592f;
            }
            string jointPositionsStr = string.Join(", ", JointPositions);
            bool is2_0 = false;
            bool is2_1 = false;
            bool is2_2 = false;
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.thisCollider == Collider_2_0)
                {
                    is2_0 = true;
                } else if (contact.thisCollider == Collider_2_1)
                {
                    is2_1 = true;
                } else if (contact.thisCollider == Collider_2_2)
                {
                    is2_2 = true;
                }
            }
            if (is2_0)
            {
                Renderer_2_0.material = collisionMaterial;
            } 

            if (is2_1)
            {
                Renderer_2_1.material = collisionMaterial;
            } 

            if (is2_2)
            {
                Renderer_2_2.material = collisionMaterial;
            } 
        }

        private void OnCollisionExit(Collision collision)
        {
            Renderer_2_0.material = defaultMaterial_0;
            Renderer_2_1.material = defaultMaterial_1;
            Renderer_2_2.material = defaultMaterial_0;
        }
    }


}