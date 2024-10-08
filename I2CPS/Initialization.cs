using RosSharp.RosBridgeClient.MessageTypes.Moveit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class Initialization : MonoBehaviour
    {

        public GetHapticPOS GetHapticPOS;
        public ArticulationBody endEffector;
        public ArticulationBody Home;

        public float j1;
        public float j2;
        public float j3;
        public float j4;
        public float j5;
        public float j6;
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
            jointPositions = new List<float> { j1 * 3.141592f / 180, j2 * 3.141592f / 180, j3 * 3.141592f / 180, j4 * 3.141592f / 180, j5 * 3.141592f / 180, j6 * 3.141592f / 180 };
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
                jointPositions = new List<float> { j1 * 3.141592f / 180, j2 * 3.141592f / 180, j3 * 3.141592f / 180, j4 * 3.141592f / 180, j5 * 3.141592f / 180, j6 * 3.141592f / 180 };
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
            endEffector.SetJointPositions(jointPositions);

            // Switch and send Haptic end-effector 'POS and Pose' to ROS
            // ROS Ik Solver send joint value for every 0.1s
            // Change Joint angles according to


        }

    }
}