using RosSharp.RosBridgeClient.MessageTypes.Moveit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

namespace RosSharp.RosBridgeClient
{
    public class Initialization : MonoBehaviour
    {
        public float starttime;
        public float endtime;
        public GetHapticPOS GetHapticPOS;
        public ArticulationBody joint1;
        public ArticulationBody Home;
        RosSharp.RosBridgeClient.JointValueSubscriber JointValueSubscriber;
        RosSharp.RosBridgeClient.ForceFeedbackSubscriber ForceFeedbackSubscriber;
        RosSharp.RosBridgeClient.OperatorTrajSubscriber OperatorTrajSubscriber;
        private List<float> jointPositions;
        public Camera CameraEnvspace;
        public Camera CameraWorkspace;
        public Camera CameraRobotView1;
        public Camera CameraRobotView2;
        public Camera CameraWorkspace_support;
        public Camera Haptic_workspace_View;
        public Canvas Canvas1;
        public Canvas Canvas2;

        public Transform ToolEnd;
        public Vector3 ToolEndPOS;

        string filename = "";
        string filename_opt = "";
        public int count;
        public int recording_operator;
        private List<float> operator_data;
        public List<float> operator_data_saved = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public List<float> operator_data_new = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public bool ReadDone;


        // Start is called before the first frame update
        protected void Start()
        {
            print("Welcome to I2CPS");
            // Camera Settings
            CameraEnvspace.targetDisplay = 0;
            ToolEndPOS = ToolEnd.position;
            // Operator Settings
            recording_operator = 0;
            count = 0;
            // 1. Set Robot into Home position 
            jointPositions = new List<float> { 0.0f, -0.00045378606f, 1.569958569f, 0.000383972435f, 1.570886f, 0.0f };
            Home.SetJointPositions(jointPositions);

            // 2. Show the Manual

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
            if (-0.15f < ToolEndPOS.x && ToolEndPOS.x < 0.15f && 0.0f < ToolEndPOS.y && ToolEndPOS.y < 0.15f && 0.25f < ToolEndPOS.z && ToolEndPOS.z < 0.55f)
            {
                print("Reached the Workspace");
            }

            if (recording_operator == 1)
            {
                record_operator_data();
            }
        }

        public void record_operator_data()
        {
            float time = (DateTime.Now.Millisecond);
            OperatorTrajSubscriber = (RosSharp.RosBridgeClient.OperatorTrajSubscriber)this.GetComponentInParent(typeof(RosSharp.RosBridgeClient.OperatorTrajSubscriber));
            ForceFeedbackSubscriber = (RosSharp.RosBridgeClient.ForceFeedbackSubscriber)this.GetComponentInParent(typeof(RosSharp.RosBridgeClient.ForceFeedbackSubscriber));
            operator_data = new List<float>
            {
                (float)OperatorTrajSubscriber.x,
                (float)OperatorTrajSubscriber.y,
                (float)OperatorTrajSubscriber.z,
                (float)OperatorTrajSubscriber.ori_x,
                (float)OperatorTrajSubscriber.ori_y,
                (float)OperatorTrajSubscriber.ori_z,
                (float)JointValueSubscriber.J1,
                (float)JointValueSubscriber.J2,
                (float)JointValueSubscriber.J3,
                (float)JointValueSubscriber.J4,
                (float)JointValueSubscriber.J5,
                (float)JointValueSubscriber.J6,
                (float)ForceFeedbackSubscriber.Fx,
                (float)ForceFeedbackSubscriber.Fy,
                (float)ForceFeedbackSubscriber.Fz,
                (float)ForceFeedbackSubscriber.Mx,
                (float)ForceFeedbackSubscriber.My,
                (float)ForceFeedbackSubscriber.Mz,
                time
            };

            StreamWriter tw;
            tw = File.AppendText(filename);
            for (int i = 0; i < 19; i++)
            {
                //print($"See operator data, {i}, {operator_data[i]}");
                tw.Write(operator_data[i]);
                tw.Write(",");
            }
            tw.WriteLine();
            tw.Close();
            // print($"See operator data, {operator_data[0]}, {operator_data[1]}, {operator_data[2]}, {operator_data[3]}, {operator_data[4]}, {operator_data[5]}, {operator_data[6]}");
        }

        public void Operator_data()
        {
            TextWriter tw = new StreamWriter(filename, false);          
            tw.WriteLine("X, Y, Z, ORI.x, ORI.y, ORI.z, J1, J2, J3, J4, J5, J6, Fx, Fy, Fz, Mx, My, Mz, time");
            tw.Close();
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

        public void Trajectory_Optimzer()
        {
            filename = Application.dataPath + $"/Scripts/operator_trajectory_{count}.csv";
            filename_opt = Application.dataPath + $"/Scripts/operator_trajectory_opt_{count}.csv";
            ReadDone = false;
            TextWriter tws = new StreamWriter(filename_opt, false);
            tws.WriteLine("X, Y, Z, ORI.x, ORI.y, ORI.z, J1, J2, J3, J4, J5, J6, Fx, Fy, Fz, Mx, My, Mz");
            tws.Close();

            StreamReader reader = new StreamReader(filename);
            while (ReadDone == false)
            {
                string ReadData = reader.ReadLine();
                // print(ReadData);
                if (ReadData == null)
                {
                    ReadDone=true;
                    break;
                }
                string[] Data = ReadData.Split(',');
                // print($"{Data[0]},{Data[1]},{Data[2]},{Data[3]},{Data[4]},{Data[5]}");
                if (Data[0] == "X")
                {
                    continue;
                }

                for (int i = 0; i < 18; i++)
                {
                    operator_data_new[i] = float.Parse(Data[i]);
                    //print($"{i}, {operator_data_new[i]}");
                }
                // print($"New,{operator_data_new[0]}, {operator_data_new[1]}, {operator_data_new[2]}, {operator_data_new[3]}, {operator_data_new[4]}, {operator_data_new[5]}");

                double distance_threshold = Math.Sqrt(Math.Pow(operator_data_saved[0] - operator_data_new[0], 2) + Math.Pow(operator_data_saved[1] - operator_data_new[1], 2) + Math.Pow(operator_data_saved[2] - operator_data_new[2], 2));
                double orientational_threshold = Math.Sqrt(Math.Pow(operator_data_saved[3] - operator_data_new[3], 2) + Math.Pow(operator_data_saved[4] - operator_data_new[4], 2) + Math.Pow(operator_data_saved[5] - operator_data_new[5], 2));
                double force_threshold = Math.Sqrt(Math.Pow(operator_data_saved[12] - operator_data_new[12], 2) + Math.Pow(operator_data_saved[13] - operator_data_new[13], 2) + Math.Pow(operator_data_saved[14] - operator_data_new[14], 2));

                // print($" {operator_data_saved[0]}, {operator_data_new[0]}, {Math.Pow(operator_data_saved[0] - operator_data_new[0], 2)}");
                //print($"Threshold values, {distance_threshold}, {orientational_threshold}, {force_threshold}");
                if ((distance_threshold > 0.01 || force_threshold > 0.1) || (orientational_threshold > 0.01 || force_threshold > 0.1))
                {
                    // print($"Threshold exist, {distance_threshold}");
                    StreamWriter tww;
                    tww = File.AppendText(filename_opt);
                    for (int i = 0; i < 18; i++)
                    {
                        tww.Write(operator_data_new[i]);
                        tww.Write(",");
                    }
                    tww.Write(",");
                    tww.WriteLine();
                    tww.Close();
                    operator_data_saved = operator_data_new.ToList();
                }
                else
                {
                    operator_data_saved = operator_data_saved;
                    // print(operator_data_saved[0]);
                }
                // print($"Saved,{operator_data_saved[0]}, {operator_data_saved[1]}, {operator_data_saved[2]}, {operator_data_saved[3]}, {operator_data_saved[4]}, {operator_data_saved[5]}");

            }
            print("Trajectory Optimized");

        }
        public void recording_start()
        {
            print("Record Trajectory");
            count ++;
            print(count);
            recording_operator = 1;
            filename = Application.dataPath + $"/Scripts/operator_trajectory_{count}.csv";
            Operator_data();
        }
        public void recording_end()
        {
            print("End of Recording Trajectory (Please Optimize the Recorded Trajectory)");
            recording_operator = 0;
        }

        private void UpdateKeys()
        {
            //Make Button UI for Recording Operator's data 

            //if (Input.GetKeyDown(KeyCode.H))
            //{
            //    print("Send Robot Back to Home POS");
            //    jointPositions = new List<float> { 0.0f, -0.00045378606f, 1.569958569f, 0.000383972435f, 1.570886f, 0.0f };
            //    Home.SetJointPositions(jointPositions);
            //}

            // Key to change Camera for workspace
            if (Input.GetKeyDown(KeyCode.Space))
            {
                print("Display Change");
                if (CameraEnvspace.targetDisplay == 0)
                {
                    CameraEnvspace.targetDisplay = 1;
                    CameraRobotView1.targetDisplay = 1;
                    Canvas1.targetDisplay = 1;
                    CameraWorkspace.targetDisplay = 0;
                    CameraRobotView2.targetDisplay = 0;
                    Haptic_workspace_View.targetDisplay = 0;
                    Canvas2.targetDisplay = 0;
                }
                else if (CameraWorkspace.targetDisplay == 0)
                {
                    Canvas2.targetDisplay = 1;
                    CameraWorkspace.targetDisplay = 1;
                    CameraRobotView2.targetDisplay = 1;
                    Haptic_workspace_View.targetDisplay = 1;
                    CameraEnvspace.targetDisplay = 0;
                    CameraRobotView1.targetDisplay = 0;
                    Canvas1.targetDisplay = 0;
                }
            }
            // Key to not use haptic

        }

    }
}
