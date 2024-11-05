using RosSharp.RosBridgeClient.MessageTypes.Moveit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RosSharp.RosBridgeClient
{
    public class Initialization_Robot : MonoBehaviour
    {
        public GetHapticPOS GetHapticPOS;
        public ArticulationBody joint1;
        public ArticulationBody Home;
        RosSharp.RosBridgeClient.JointValueSubscriber JointValueSubscriber;
        RosSharp.RosBridgeClient.ForceFeedbackSubscriber ForceFeedbackSubscriber;
        RosSharp.RosBridgeClient.OperatorTrajSubscriber OperatorTrajSubscriber;
        RosSharp.RosBridgeClient.IkinServiceTest IkinServiceTest;
        private List<float> jointPositions;

        public Camera HapticWorkspaceView;
        public Camera HapticEnvironmentView;
        public Camera HapticEnvironmentViewSupport;
        public Camera HapticDeviceView;
        public Camera RobotWorkspaceView;
        public Camera RobotActiveView;
       
        public Canvas Canvas1;
        public Canvas Canvas2;

        public Transform ToolEnd;
        public Vector3 ToolEndPOS;
        public bool ReadDone;

        public bool ReadDonecheck;
        string combining_filename_tuned = "";
        string combined_filename = "";
        string input_filename_execute = "";
        public float[] joint_data_read = new float[6];
        public int moving_count;
        public double[] read_operator_data = new double[6];

        // Start is called before the first frame update
        protected void Start()
        {
            print("Welcome to Digital-Twin");
            // Camera Settings
            HapticWorkspaceView.targetDisplay = 0;

            // 1. Set Robot into Home position 
            jointPositions = new List<float> { 0.0f, -0.00045378606f, 1.569958569f, 0.000383972435f, 1.570886f, 0.0f };
            Home.SetJointPositions(jointPositions);
            //2. Robot Moving Settings
            moving_count = 0;

            // 3. Show the Manual

        }

        // Update is called once per frame
        void Update()
        {
            UpdateKeys();
            // Receive joint values for every 0.01s from ROS#
            JointValueSubscriber = (RosSharp.RosBridgeClient.JointValueSubscriber)this.GetComponentInParent(typeof(RosSharp.RosBridgeClient.JointValueSubscriber));
            // StartCoroutine(MoveRobot());

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
                if (HapticWorkspaceView.targetDisplay == 0)
                {
                    HapticWorkspaceView.targetDisplay = 1;
                    HapticEnvironmentView.targetDisplay = 1;
                    HapticEnvironmentViewSupport.targetDisplay = 1;
                    Canvas1.targetDisplay = 1;

                    RobotWorkspaceView.targetDisplay = 0;
                    RobotActiveView.targetDisplay = 0;
                    Canvas2.targetDisplay = 0;
                }
                else if (RobotWorkspaceView.targetDisplay == 0)
                {
                    HapticWorkspaceView.targetDisplay = 0;
                    HapticEnvironmentView.targetDisplay = 0;
                    HapticEnvironmentViewSupport.targetDisplay = 0;
                    Canvas1.targetDisplay = 0;

                    RobotWorkspaceView.targetDisplay = 1;
                    RobotActiveView.targetDisplay = 1;
                    Canvas2.targetDisplay = 1;
                }
            }
            // Key to not use haptic

        }

        public void getRobotSolution()
        {
            combined_filename = Application.dataPath + $"/Scripts/Optimal Trajectory/operator_trajectory_final.csv";
            TextWriter tw = new StreamWriter(combined_filename, false);
            tw.WriteLine("X, Y, Z, ORI.x, ORI.y, ORI.z");
            tw.Close();

            for (int record_num = 1; record_num < 100; record_num++)
            {
                ReadDonecheck = false;
                //print($"Record Num: {record_num}");
                combining_filename_tuned = Application.dataPath + $"/Scripts/operator_trajectory_tuning_{record_num}.csv";
                try
                {
                    StreamReader reader = new StreamReader(combining_filename_tuned);
                    while (ReadDonecheck == false)
                    {
                        string ReadData = reader.ReadLine();
                        // print(ReadData);
                        if (ReadData == null)
                        {
                            ReadDonecheck = true;
                            break;
                        }
                        string[] Data = ReadData.Split(',');
                        // print($"{Data[0]},{Data[1]},{Data[2]},{Data[3]},{Data[4]},{Data[5]}");
                        if (Data[0] == "X")
                        {
                            continue;
                        }

                        StreamWriter tww;
                        tww = File.AppendText(combined_filename);

                        for (int i = 0; i < 6; i++)
                        {
                            read_operator_data[i] = float.Parse(Data[i]);
                            tww.Write(read_operator_data[i]);
                            tww.Write(",");
                        }
                        tww.WriteLine();
                        tww.Close();
                    }
                }
                catch (FileNotFoundException) 
                {
                    print($"Combined {record_num-1} Records");
                    break;
                }  
            }

            IkinServiceTest = (RosSharp.RosBridgeClient.IkinServiceTest)this.GetComponentInParent(typeof(RosSharp.RosBridgeClient.IkinServiceTest));
            IkinServiceTest.getrobotsolution();
            print("Received Solution");
        }

        public void executeRobot()
        {
            ReadDone = false;
            input_filename_execute = Application.dataPath + $"/Scripts/Solution/joint_solution_checking.csv";
            StreamReader reader = new StreamReader(input_filename_execute);

            while (ReadDone == false)
            {
                string ReadData = reader.ReadLine();
                print(ReadData);
                if (ReadData == null)
                {
                    ReadDone = true;
                    break;
                }
                string[] Data = ReadData.Split(',');
                print($"{Data[0]},{Data[1]},{Data[2]},{Data[3]},{Data[4]},{Data[5]}");
                if (Data[0] == "J1")
                {
                    continue;
                }
                moving_count++;
                List<float> joint_data_read = new List<float>
                {
                    float.Parse(Data[0]),
                    float.Parse(Data[1]),
                    float.Parse(Data[2]),
                    float.Parse(Data[3]),
                    float.Parse(Data[4]),
                    float.Parse(Data[5])
                };

                joint1.SetJointPositions(joint_data_read);
                print($"Moved {moving_count} times");
                Task.Delay(100);

            }
        }

            //public void record_operator_data()
            //{
            //    float time = (DateTime.Now.Millisecond);
            //    OperatorTrajSubscriber = (RosSharp.RosBridgeClient.OperatorTrajSubscriber)this.GetComponentInParent(typeof(RosSharp.RosBridgeClient.OperatorTrajSubscriber));
            //    ForceFeedbackSubscriber = (RosSharp.RosBridgeClient.ForceFeedbackSubscriber)this.GetComponentInParent(typeof(RosSharp.RosBridgeClient.ForceFeedbackSubscriber));
            //    operator_data = new List<float>
            //    {
            //        (float)OperatorTrajSubscriber.x,
            //        (float)OperatorTrajSubscriber.y,
            //        (float)OperatorTrajSubscriber.z,
            //        (float)OperatorTrajSubscriber.ori_x,
            //        (float)OperatorTrajSubscriber.ori_y,
            //        (float)OperatorTrajSubscriber.ori_z,
            //        (float)JointValueSubscriber.J1,
            //        (float)JointValueSubscriber.J2,
            //        (float)JointValueSubscriber.J3,
            //        (float)JointValueSubscriber.J4,
            //        (float)JointValueSubscriber.J5,
            //        (float)JointValueSubscriber.J6,
            //        (float)ForceFeedbackSubscriber.Fx,
            //        (float)ForceFeedbackSubscriber.Fy,
            //        (float)ForceFeedbackSubscriber.Fz,
            //        (float)ForceFeedbackSubscriber.Mx,
            //        (float)ForceFeedbackSubscriber.My,
            //        (float)ForceFeedbackSubscriber.Mz,
            //        time
            //    };

            //    StreamWriter tw;
            //    tw = File.AppendText(filename);
            //    for (int i = 0; i < 19; i++)
            //    {
            //        //print($"See operator data, {i}, {operator_data[i]}");
            //        tw.Write(operator_data[i]);
            //        tw.Write(",");
            //    }
            //    tw.WriteLine();
            //    tw.Close();
            //    // print($"See operator data, {operator_data[0]}, {operator_data[1]}, {operator_data[2]}, {operator_data[3]}, {operator_data[4]}, {operator_data[5]}, {operator_data[6]}");
            //}

        
    }
}
