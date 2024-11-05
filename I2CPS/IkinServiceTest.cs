using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient.MessageTypes;
using RosSharp.RosBridgeClient.MessageTypes.CollisionTestM1509;
using System.IO;
using System;
using System.Linq;

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
        public List<float> joint_data = new List<float> { 0, 0, 0, 0, 0, 0 };
        public bool ReadDone;

        void Start()
        {
            RosConnector = GetComponent<RosConnector>();

            filename_sol = Application.dataPath + $"/Scripts/Solution/joint_solution.csv";
            TextWriter tw = new StreamWriter(filename_sol, false);
            tw.WriteLine("X, Y, Z, ORI.x, ORI.y, ORI.z, ORI.w");
            tw.Close();

        }

        public void getRobotSolution()
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

                for (int i = 0; i < 6; i++)
                {
                    operator_data_read[i] = double.Parse(Data[i]);
                }
                sbyte solSpace = 1;
                sbyte refVal = 0;
                print($"{operator_data_read[0]},{operator_data_read[1]},{operator_data_read[2]},{operator_data_read[3]},{operator_data_read[4]},{operator_data_read[5]}");

                CallIkinService(operator_data_read, solSpace, refVal);
            }
        }


        public void CallIkinService(double[] pos, sbyte sol_space, sbyte @ref)
        {
            IkinRequest request = new IkinRequest
            {
                pos = pos,
                sol_space = sol_space,
                @ref = @ref
            };

            // Debug.Log("Request Pos: " + string.Join(", ", pos));
            // Debug.Log("Sol Space: " + sol_space);
            // Debug.Log("Ref Val: " + @ref);

            RosConnector.RosSocket.CallService<IkinRequest, IkinResponse>(
                ikin, ServiceRequestHandler, request);
        }

        private void ServiceRequestHandler(IkinResponse response)
        {
            // Debug.Log("ServiceResponseHandler called");
            if (response.success)
            {
                Debug.Log("Received joint positions: " + string.Join(", ", response.conv_posj));
                //joints = response;
                StreamWriter tw;
                tw = File.AppendText(filename_sol);
                for (int i = 0; i < 6; i++)
                {
                    //print($"See operator data, {i}, {operator_data[i]}");
                    tw.Write(joint_data[i]);
                    tw.Write(",");
                }
                tw.WriteLine();
                tw.Close();
            }
            else
            {
                Debug.LogError("Failed to get joint position. Response details: " + FormatResponse(response));
            }
        }
        private string FormatResponse(IkinResponse response)
        {
            // Assuming IkinResponse has properties 'success' and 'conv_posj'
            return $"success: {response.success}, conv_posj: [{(response.conv_posj != null ? string.Join(", ", response.conv_posj) : "null")}]";
        }
    }
}
