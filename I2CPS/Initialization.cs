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
        public Camera CameraEnvspace;
        public Camera CameraWorkspace;
        public Camera CameraRobotView1;
        public Camera CameraRobotView2;
        public Camera CameraWorkspace_support;
        public Camera Haptic_workspace_View;

        public Transform ToolEnd;
        public Vector3 ToolEndPOS;

        // Start is called before the first frame update
        protected void Start()
        {
            print("Welcome to I2CPS");
            // Camera Settings
            CameraEnvspace.targetDisplay = 0;
            ToolEndPOS = ToolEnd.position;
            // Haptic_workspace_View.enabled = true;

            // 1. Set Robot into Home position 
            jointPositions = new List<float> { 0.0f, -0.00045378606f, 1.569958569f, 0.000383972435f, 1.570886f, 0.0f };
            Home.SetJointPositions(jointPositions);

            // 2. Show the Manual


        }

        private void UpdateKeys()
        {

            if (Input.GetKeyDown(KeyCode.H))
            {
                print("Send Robot Back to Home POS");
                jointPositions = new List<float> {0.0f, -0.00045378606f, 1.569958569f, 0.000383972435f, 1.570886f, 0.0f};
                Home.SetJointPositions(jointPositions);
            }

            // Key to change Camera for workspace
            if (Input.GetKeyDown(KeyCode.Space))
            {
                print("Camera Change");
                if (CameraEnvspace.targetDisplay == 0)
                {
                    CameraEnvspace.targetDisplay = 1;
                    CameraRobotView1.targetDisplay = 1;
                    CameraWorkspace.targetDisplay = 0;
                    CameraRobotView2.targetDisplay = 0;
                    Haptic_workspace_View.targetDisplay = 0;
                }
                else if (CameraWorkspace.targetDisplay == 0)
                {
                    CameraWorkspace.targetDisplay = 1;
                    CameraRobotView2.targetDisplay = 1;
                    CameraEnvspace.targetDisplay = 0;
                    CameraRobotView1.targetDisplay = 0;
                    Haptic_workspace_View.targetDisplay = 1;
                }
            }

            // Key to not use haptic

        }

        // Update is called once per frame
        void Update()
        {
            UpdateKeys();

            // Receive joint values for every 0.01s from ROS#
            JointValueSubscriber = (RosSharp.RosBridgeClient.JointValueSubscriber)this.GetComponentInParent(typeof(RosSharp.RosBridgeClient.JointValueSubscriber));
            StartCoroutine(MoveRobot());


            // When reached the Workspace
            ToolEndPOS = ToolEnd.position;
            //print($"ToolEnd POS, {ToolEndPOS.x}, {ToolEndPOS.y}, {ToolEndPOS.z}");
            if (-0.1f < ToolEndPOS.x && ToolEndPOS.x < 0.1f && 0.0f < ToolEndPOS.y && ToolEndPOS.y < 0.1f && 0.3f < ToolEndPOS.z && ToolEndPOS.z < 0.5f)
            {
                print("Reached the Workspace");
                CameraEnvspace.targetDisplay = 1;
                CameraRobotView1.targetDisplay = 1;
                CameraWorkspace.targetDisplay = 0;
                CameraRobotView2.targetDisplay = 0;
                CameraWorkspace_support.targetDisplay = 0;
            }

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
                // Current Home position joint values : -0.000341 -0.000758 1.570947 0.000426 1.570886 0.000134

                yield return new WaitForSeconds(0.01f);
            }
        }

    }
}
