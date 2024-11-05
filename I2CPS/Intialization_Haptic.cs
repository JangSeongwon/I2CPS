// This code contains 3D SYSTEMS Confidential Information and is disclosed to you
// under a form of 3D SYSTEMS software license agreement provided separately to you.
//
// Notice
// 3D SYSTEMS and its licensors retain all intellectual property and
// proprietary rights in and to this software and related documentation and
// any modifications thereto. Any use, reproduction, disclosure, or
// distribution of this software and related documentation without an express
// license agreement from 3D SYSTEMS is strictly prohibited.
//
// ALL 3D SYSTEMS DESIGN SPECIFICATIONS, CODE ARE PROVIDED "AS IS.". 3D SYSTEMS MAKES
// NO WARRANTIES, EXPRESSED, IMPLIED, STATUTORY, OR OTHERWISE WITH RESPECT TO
// THE MATERIALS, AND EXPRESSLY DISCLAIMS ALL IMPLIED WARRANTIES OF NONINFRINGEMENT,
// MERCHANTABILITY, AND FITNESS FOR A PARTICULAR PURPOSE.
//
// Information and code furnished is believed to be accurate and reliable.
// However, 3D SYSTEMS assumes no responsibility for the consequences of use of such
// information or for any infringement of patents or other rights of third parties that may
// result from its use. No license is granted by implication or otherwise under any patent
// or patent rights of 3D SYSTEMS. Details are subject to change without notice.
// This code supersedes and replaces all information previously supplied.
// 3D SYSTEMS products are not authorized for use as critical
// components in life support devices or systems without express written approval of
// 3D SYSTEMS.
//
// Copyright (c) 2021 3D SYSTEMS. All rights reserved.

using RosSharp.RosBridgeClient.MessageTypes.Moveit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HapticGUI;
using System.IO;
using System;
using System.Linq;
using RosSharp.RosBridgeClient;

public class Initialization_Haptic : MonoBehaviour
{
    public HapticPlugin HPlugin = null;
    public GameObject DeviceInfo;
    public GameObject Device1;
    public string deviceName;

    private List<float> operator_data;
    public int count;
    public int recording_operator;
    string filename = "";
    string filename_tuning = "";
    public List<float> operator_data_saved = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public List<float> operator_data_new = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public bool ReadDone;

    public float coord_offset;
    public float tool_offset;

    public Transform HapticTransform;

    private void Start()
    {
        // Operator Settings
        recording_operator = 0;
        count = 0;
        tool_offset = 0.164f;
        coord_offset = -1.0f;

        //TextMeshPro TMesh = DeviceInfo.GetComponent<TextMeshPro>();
        //TMesh.text = HPlugin.DeviceIdentifier + "\n" + HPlugin.ModelType + "\n" + HPlugin.SerialNumber + "\n" + HPlugin.MaxForce.ToString("F") + " N";
        Device1.SetActive(true);
        Device1.GetComponent<VirtualHaptic>().ShowGizmo = true;
        Device1.GetComponent<VirtualHaptic>().ShowLabels = true;

    }


    // Update is called once per frame
    private void Update()
    {
        UpdateKeys();
        if (recording_operator == 1)
        {
            record_operator_data();
        }
    }
    private void UpdateKeys()
    {
        if (HPlugin != null)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (!HPlugin.isNavTranslation())
                {
                    HPlugin.EnableNavTranslation();
                    HPlugin.DisableNavRotation();
                    print("Rotation->Translation");
                }
                else
                {
                    HPlugin.DisableNavTranslation();
                    print("No Translation");
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!HPlugin.isNavRotation())
                {
                    HPlugin.EnableNavRotation();
                    HPlugin.DisableNavTranslation();
                }
                else
                {
                    HPlugin.DisableNavRotation();
                }
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                HPlugin.EnableVibration();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                HPlugin.DisableVibration();
            }
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
    }

    public void recording_start()
    {
        count++;
        print($"Recording Operator's Trajectory {count}");
        filename = Application.dataPath + $"/Scripts/operator_trajectory_{count}.csv";
        Operator_data();
        recording_operator = 1;
    }
    public void recording_end()
    {
        recording_operator = 0;
        print("End of Recording. Start Tuning");
        Trajectory_Tuning();
        operator_data_saved = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        operator_data_new = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        print($"Finished Tuning: {count}");
    }

    public void Operator_data()
    {
        TextWriter tw = new StreamWriter(filename, false);
        tw.WriteLine("X, Y, Z, ORI.x, ORI.y, ORI.z, Fx, Fy, Fz, F.Mag, time");
        tw.Close();
    }

    public void record_operator_data()
    {
        float time = (DateTime.Now.Millisecond);
        Vector3 position = HapticTransform.position;
        Quaternion rotation = HapticTransform.rotation;
        Vector3 force = HPlugin.CurrentForce;
        float force_mag = HPlugin.MagForce;
        //print($"Haptic POS, ORI: {position}, {rotation}, {force}, {force_mag}");
        operator_data = new List<float>
        {
            (float)position.x * 1000 - 4000,
            (float)position.y * 1000 + tool_offset,
            (float)position.z * 1000 + 400,
            (float)rotation.x,
            (float)rotation.y,
            (float)rotation.z * coord_offset,
            (float)force.x,
            (float)force.y,
            (float)force.z,
            force_mag,
            time
        };

        StreamWriter tw;
        tw = File.AppendText(filename);
        for (int i = 0; i < 11; i++)
        {
            //print($"See operator data, {i}, {operator_data[i]}");
            tw.Write(operator_data[i]);
            tw.Write(",");
        }
        tw.WriteLine();
        tw.Close();
        // print($"See operator data, {operator_data[0]}, {operator_data[1]}, {operator_data[2]}, {operator_data[3]}, {operator_data[4]}, {operator_data[5]}, {operator_data[6]}");
    }

    public void Trajectory_Tuning()
    {
        filename = Application.dataPath + $"/Scripts/operator_trajectory_{count}.csv";
        filename_tuning = Application.dataPath + $"/Scripts/operator_trajectory_tuning_{count}.csv";
        ReadDone = false;
        TextWriter tws = new StreamWriter(filename_tuning, false);
        tws.WriteLine("X, Y, Z, ORI.x, ORI.y, ORI.z, Fx, Fy, Fz, F.Mag, time");
        tws.Close();

        StreamReader reader = new StreamReader(filename);
        while (ReadDone == false)
        {
            string ReadData = reader.ReadLine();
            // print(ReadData);
            if (ReadData == null)
            {
                ReadDone = true;
                break;
            }
            string[] Data = ReadData.Split(',');
            // print($"{Data[0]},{Data[1]},{Data[2]},{Data[3]},{Data[4]},{Data[5]}");
            if (Data[0] == "X")
            {
                continue;
            }

            for (int i = 0; i < 11; i++)
            {
                operator_data_new[i] = float.Parse(Data[i]);
                //print($"{i}, {operator_data_new[i]}");
            }
            // print($"New,{operator_data_new[0]}, {operator_data_new[1]}, {operator_data_new[2]}, {operator_data_new[3]}, {operator_data_new[4]}, {operator_data_new[5]}");

            double distance_threshold = Math.Sqrt(Math.Pow(operator_data_saved[0] - operator_data_new[0], 2) + Math.Pow(operator_data_saved[1] - operator_data_new[1], 2) + Math.Pow(operator_data_saved[2] - operator_data_new[2], 2));
            double orientational_threshold = Math.Sqrt(Math.Pow(operator_data_saved[3] - operator_data_new[3], 2) + Math.Pow(operator_data_saved[4] - operator_data_new[4], 2) + Math.Pow(operator_data_saved[5] - operator_data_new[5], 2));
            double force_threshold = Math.Sqrt(Math.Pow(operator_data_saved[6] - operator_data_new[6], 2) + Math.Pow(operator_data_saved[7] - operator_data_new[7], 2) + Math.Pow(operator_data_saved[8] - operator_data_new[8], 2));

            if ((distance_threshold < 0.001 && force_threshold < 0.0001) || (orientational_threshold < 0.001 && force_threshold < 0.0001))
            {
                continue;
            }

            // print($" {operator_data_saved[0]}, {operator_data_new[0]}, {Math.Pow(operator_data_saved[0] - operator_data_new[0], 2)}");
            //print($"Threshold values, {distance_threshold}, {orientational_threshold}, {force_threshold}");
            if ((distance_threshold > 0.001) || (orientational_threshold > 0.001) || (force_threshold > 0.0001))
            {
                //print($"Distance Threshold, {distance_threshold}");
                //print($"Orientation Threshold, {orientational_threshold}");
                //print($"Force Threshold, {force_threshold}");
                StreamWriter tww;
                tww = File.AppendText(filename_tuning);
                for (int i = 0; i < 11; i++)
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
                print(operator_data_saved[0]);
            }
            // print($"Saved,{operator_data_saved[0]}, {operator_data_saved[1]}, {operator_data_saved[2]}, {operator_data_saved[3]}, {operator_data_saved[4]}, {operator_data_saved[5]}");

        }

    }


}
