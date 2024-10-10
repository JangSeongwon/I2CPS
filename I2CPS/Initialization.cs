using RosSharp.RosBridgeClient.MessageTypes.Moveit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class Initialization : MonoBehaviour
    {

        public GetHapticPOS GetHapticPOS;
        public ArticulationBody joint1;
        public ArticulationBody Home;
        RosSharp.RosBridgeClient.JointValueSubscriber JointValueSubscriber;
        private List<float> jointPositions;
        public Camera CameraWorkspace;
        public Camera CameraTarget;
        public Camera CameraSideView;
        
        // Start is called before the first frame update
        protected void Start()
        {
            print("Welcome to I2CPS");
            // Camera Settings
            CameraWorkspace.enabled = true;
            CameraTarget.enabled = false;
            CameraSideView.enabled = false;

            // 1. Set Robot into Home position 
            jointPositions = new List<float> { 0.005f * 3.141592f / 180, -0.017f * 3.141592f / 180, 90.063f * 3.141592f / 180, 0.005f * 3.141592f / 180, 90.013f * 3.141592f / 180, 0.008f * 3.141592f / 180 };
            Home.SetJointPositions(jointPositions);

            // 2. Show the Manual


        }

        private void UpdateKeys()
        {
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    print("Start Task");
            //}

            if (Input.GetKeyDown(KeyCode.H))
            {
                print("Send Robot Back to Home POS");
                jointPositions = new List<float> { 0.005f * 3.141592f / 180, -0.017f * 3.141592f / 180, 90.063f * 3.141592f / 180, 0.005f * 3.141592f / 180, 90.013f * 3.141592f / 180, 0.008f * 3.141592f / 180 };
                Home.SetJointPositions(jointPositions);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (CameraWorkspace.enabled)
                {
                    CameraTarget.enabled = true; 
                    CameraWorkspace.enabled = false;
                                     
                }
                else if (CameraTarget.enabled)
                {
                    CameraSideView.enabled = true; 
                    CameraTarget.enabled = false;
                    
                }
                else if (CameraSideView.enabled)
                {
                    CameraWorkspace.enabled = true;
                    CameraSideView.enabled = false;

                }
            }

            // Key to change workspace
            // Key to change velocity
            // Key to not use haptic

        }

        // Update is called once per frame
        void Update()
        {
            UpdateKeys();

            // Receive joint values for every 0.1s from ROS Ik Solver
            JointValueSubscriber = (RosSharp.RosBridgeClient.JointValueSubscriber)this.GetComponentInParent(typeof(RosSharp.RosBridgeClient.JointValueSubscriber));
            // Change Joint angles according to
            StartCoroutine(MoveRobot());
                       
        }

        private IEnumerator MoveRobot()
        {
            while (true)
            {

                List<float> jointPositions = new List<float>
                {
                    JointValueSubscriber.J1,
                    JointValueSubscriber.J2,
                    JointValueSubscriber.J3,
                    JointValueSubscriber.J4,
                    JointValueSubscriber.J5,
                    JointValueSubscriber.J6
                };

                joint1.SetJointPositions(jointPositions);
                // print($"Joints Value, {jointPositions[0]},{jointPositions[1]},{jointPositions[2]},{jointPositions[3]}");

                yield return new WaitForSeconds(0.1f);
            }
        }

    }
}