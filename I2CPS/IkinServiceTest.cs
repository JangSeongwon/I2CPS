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
        public int get_response;
        public int count;
        public bool isServiceCompleted;

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

                for (int i = 0; i < 6; i++)
                {
                    operator_data_read[i] = double.Parse(Data[i]);
                }
                sbyte solSpace = 1;
                sbyte refVal = 0;
                //print($"{operator_data_read[0]},{operator_data_read[1]},{operator_data_read[2]},{operator_data_read[3]},{operator_data_read[4]},{operator_data_read[5]}");
                //print("Send Service");
                count++;
                get_response = 1;
                CallIkinService(operator_data_read, solSpace, refVal);
            }
        }

        public IEnumerator CallIkinService(double[] pos, sbyte sol_space, sbyte @ref)
        {
            isServiceCompleted = false;
            IkinRequest request = new IkinRequest
            {
                pos = pos,
                sol_space = sol_space,
                @ref = @ref
            };

            Debug.Log("Request Pos: " + string.Join(", ", pos));
            // Debug.Log("Sol Space: " + sol_space);
            // Debug.Log("Ref Val: " + @ref);

            RosConnector.RosSocket.CallService<IkinRequest, IkinResponse>(
                ikin, ServiceRequestHandler, request);

            while (!isServiceCompleted)
            {
                yield return null;
            }
        }

        private void ServiceRequestHandler(IkinResponse response)
        {
            // Debug.Log("ServiceResponseHandler called");
            if (response.success)
            {
                Debug.Log("Received joint positions: " + string.Join(", ", response.conv_posj));
                
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
            }
            else
            {
                isServiceCompleted = true;
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
