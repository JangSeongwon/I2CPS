using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient.MessageTypes;
using RosSharp.RosBridgeClient.MessageTypes.CollisionTestM1509;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RosSharp.RosBridgeClient
{
    public class IkinServiceTest : MonoBehaviour
    {
        private RosConnector RosConnector;
        private string ikin = "/dsr01m1509/motion/ikin";
        public ArticulationBody endEffector;
        string filename_sol = "";
        string input_filename_opt = "";
        public double[] operator_data_read = new double[6];
        public double[] joint_data = new double[6];
        public bool ReadDone;
        public DateTime time_start;
        public DateTime time_start_as;
        public DateTime time_end;
        public int count;
        public int count_as;

        void Start()
        {
            RosConnector = GetComponent<RosConnector>();

            filename_sol = Application.dataPath + $"/Scripts/Solution/joint_solution.csv";
            TextWriter tw = new StreamWriter(filename_sol, false);
            tw.WriteLine("J1, J2, J3, J4, J5, J6");
            // Starting Joints
            tw.WriteLine("-0.0001011, -0.0009295, 1.5705487, 0.0005570, 1.5708914, -0.0002538");
            //Haptic Starting Joints
            tw.WriteLine("-0.0001011, -0.0009295, 1.5705487, 0.0005570, 1.5708914, -0.0002538");
            tw.Close();
            count = 0;

        }

        public void getrobotsolution()
        {
            ReadDone = false;
            input_filename_opt = Application.dataPath + $"/Scripts/Optimal Trajectory/operator_trajectory_final.csv";
            StreamReader reader = new StreamReader(input_filename_opt);
            while (ReadDone == false)
            {
                string ReadData = reader.ReadLine();
                if (ReadData == null)
                {
                    ReadDone = true;
                    break;
                }
                string[] Data = ReadData.Split(',');
                if (Data[0] == "X")
                {
                    continue;
                }

                count++;
                for (int i = 0; i < 6; i++)
                {
                    operator_data_read[i] = double.Parse(Data[i]);
                }
                sbyte solSpace = 1;
                sbyte refVal = 0;
                //print($"{operator_data_read[0]},{operator_data_read[1]},{operator_data_read[2]},{operator_data_read[3]},{operator_data_read[4]},{operator_data_read[5]}");
                //print("Send Service");
                time_start = DateTime.Now;
                print($"Service Calling time at {count}: {time_start}");
                StartCoroutine(CallIkinService(operator_data_read, solSpace, refVal, count, time_start));
            }
        }

        public IEnumerator CallIkinService(double[] pos, sbyte sol_space, sbyte @ref, int count, DateTime timestart)
        {
            bool isServiceCompleted = false;
            IkinRequest request = new IkinRequest
            {
                pos = pos,
                sol_space = sol_space,
                @ref = @ref
            };

            // Debug.Log("Request Pos: " + string.Join(", ", pos));
            // Debug.Log("Sol Space: " + sol_space);
            // Debug.Log("Ref Val: " + @ref);
            count_as = count;
            time_start_as = timestart;
            RosConnector.RosSocket.CallService<IkinRequest, IkinResponse>(
                ikin, (response) =>
                {
                    // Debug.Log("Received joint positions: " + string.Join(", ", response.conv_posj));

                    StreamWriter tw;
                    tw = File.AppendText(filename_sol);
                    for (int i = 0; i < 6; i++)
                    {
                        joint_data[i] = response.conv_posj[i];
                        //print($"See joint data, {i}, {joint_data[i]}");
                        tw.Write(joint_data[i]);
                        tw.Write(",");
                    }
                    tw.WriteLine();
                    tw.Close();
                    isServiceCompleted = true;
                    time_end = DateTime.Now;
                    print(time_end);
                    print($"Service Returning & Recording time at {count_as}, time end: {time_end}, spent {time_end-time_start_as}");
                }, request);
            yield return isServiceCompleted;
        }

    }
}
