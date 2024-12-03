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
using Unity.VisualScripting;

public class Initialization_Haptic : MonoBehaviour
{
    public HapticPlugin HPlugin = null;
    public GameObject DeviceInfo;
    public GameObject Device1;

    private List<float> operator_data;
    private List<float> force_data;
    public int count;
    public int recording_operator;
    string filename = "";
    string filename_force_data = "";
    string filename_tuning = "";
    public List<float> operator_data_saved = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public List<float> operator_data_new = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public bool ReadDone;

    public float coord_sync;
    public float coord_offset_z;
    public float coord_offset_x;
    public float tool_offset;
    public float offset;

    public Transform HapticTransform;

    private void Start()
    {
        // Operator Settings
        recording_operator = 0;
        count = 0;
        coord_offset_z = 530.0f; // 400 Haptic, 130 Robot
        coord_offset_x = 4000.0f;
        tool_offset = 164.0f;
        coord_sync = -1.0f;
        offset = 205.0f;

        //TextMeshPro TMesh = DeviceInfo.GetComponent<TextMeshPro>();
        //TMesh.text = HPlugin.DeviceIdentifier + "\n" + HPlugin.ModelType + "\n" + HPlugin.SerialNumber + "\n" + HPlugin.MaxForce.ToString("F") + " N";
        Device1.SetActive(true);
        Device1.GetComponent<VirtualHaptic>().ShowGizmo = true;
        Device1.GetComponent<VirtualHaptic>().ShowLabels = true;

        filename_force_data = Application.dataPath + $"/Voxel_space/Data/Haptic_force_data.csv";
        TextWriter tw = new StreamWriter(filename_force_data, false);
        tw.WriteLine("Fx, Fy, Fz, F.Mag");
        tw.Close();
    }


    // Update is called once per frame
    private void Update()
    {
        UpdateKeys();
        if (recording_operator == 1)
        {
            record_operator_data();
        }

        record_force_data();

    }
    private void UpdateKeys()
    {
        if (HPlugin != null)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (HPlugin.isNavRotation())
                {
                    HPlugin.DisableNavRotation();
                    print("Translation only");
                }
                else
                {
                    HPlugin.EnableNavRotation();
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
        }
    }

    public void recording_start()
    {
        count++;
        print($"Recording Operator's Trajectory. Record Number: {count}");
        filename = Application.dataPath + $"/Scripts/operator_trajectory_{count}.csv";
        Operator_data();
        recording_operator = 1;
    }
    public void recording_end()
    {
        recording_operator = 0;
        Trajectory_Tuning();
        operator_data_saved = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        operator_data_new = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        print($"Finished Tuning Record number: {count}");
    }

    public void recording_delete()
    {
        filename = Application.dataPath + $"/Scripts/operator_trajectory_{count}.csv";
        filename_tuning = Application.dataPath + $"/Scripts/operator_trajectory_tuning_{count}.csv";
        if (System.IO.File.Exists(filename))
        {
            try
            {
                System.IO.File.Delete(filename);
            }
            catch (System.IO.IOException e)
            {
                print("Error occured. Try Again");
            }
        }
        if (System.IO.File.Exists(filename_tuning))
        {
            try
            {
                System.IO.File.Delete(filename_tuning);
                count--;
            }
            catch (System.IO.IOException e)
            {
                print("Error occured. Try Again");
            }
        }

    }

    public void Operator_data()
    {
        TextWriter tw = new StreamWriter(filename, false);
        tw.WriteLine("X, Y, Z, ORI.x, ORI.y, ORI.z, Fx, Fy, Fz, F.Mag, time");
        tw.Close();
    }

    public void record_force_data()
    {
        Vector3 force = HPlugin.CurrentForce;
        float force_mag = HPlugin.MagForce;
        StreamWriter tw;
        tw = File.AppendText(filename_force_data);
        force_data = new List<float>
        {
            (float)force.x,
            (float)force.y,
            (float)force.z,
            force_mag
        };
        for (int i = 0; i < 4; i++)
        {
            tw.Write(force_data[i]);
            tw.Write(",");
        }
        tw.WriteLine();
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

        rotation = new Quaternion { w = -rotation.w, x = rotation.z, y = -rotation.x, z = rotation.y};
        //print($"See Quaternion {rotation.w}, {rotation.x}, {rotation.y}, {rotation.z}");
        var (ori_x, ori_y, ori_z) = QuaternionToEulerZYZ(rotation);
        ori_x = ori_x * 180 / Mathf.PI;
        ori_y = ori_y * 180 / Mathf.PI;
        ori_z = ori_z * 180 / Mathf.PI;
        ori_z = 180 - ori_z;
        ori_x = 180 - ori_x;
        ori_y += 90;

        if (ori_y > 180)
        {
            ori_y = ori_y - 360;
        }
        //print($"See ZYZ Euler {ori_x}, {ori_y}, {ori_z}");

        //print(position);
        operator_data = new List<float>
    {
        (float)position.z * 1000 + coord_offset_z,
        ((float)position.x * 1000 - coord_offset_x)*(coord_sync),
        (float)position.y * 1000 - tool_offset + offset,
        ori_x,
        ori_y,
        ori_z,
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

    public static (float, float, float) QuaternionToEulerZYZ(Quaternion rotation)
    {
        float qw = rotation.w;
        float qx = rotation.x;
        float qy = rotation.y;
        float qz = rotation.z;

        //float ori_x = (float)Math.Atan2(qz, qw) - (float)Math.Atan2(-qx, qy);
        //float ori_y = (float)Math.Acos(2.0f * (qw * qw + qz * qz) - 1);
        //float ori_z = (float)Math.Atan2(qz, qw) + (float)Math.Atan2(-qx, qy);

        float ori_x = Mathf.Atan2(2 * qy * qz + 2 * qw * qx, qz * qz - qy * qy - qx * qx + qw * qw);
        float ori_y = -Mathf.Asin(2 * qw * qy - 2 * qx * qz);
        float ori_z = Mathf.Atan2(2 * qx * qy + 2 * qw * qz, qx * qx + qw * qw - qz * qz - qy * qy);

        //float ori_x = Mathf.Atan2(2 * qy * qz - 2 * qw * qx, 2 * qw * qy + 2 * qx * qz);
        //float ori_y = Mathf.Atan2((float)Math.Sqrt((2 * qy * qz - 2 * qw * qx) * (2 * qy * qz - 2 * qw * qx) + (2 * qw * qy + 2 * qx * qz) * (2 * qw * qy + 2 * qx * qz)), qw * qw - qx * qx - qy * qy + qz * qz);
        //float ori_z = Mathf.Atan2(2 * qw * qx + 2 * qy * qz, -2 * qx * qz + 2 * qw * qy);

        return (ori_x, ori_y, ori_z);
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

            // Folloiwng 50Hz from Haptic
            if (distance_threshold == 0.0 || force_threshold == 0.0 || orientational_threshold == 0.0)
            {
                continue;
            }

            // print($" {operator_data_saved[0]}, {operator_data_new[0]}, {Math.Pow(operator_data_saved[0] - operator_data_new[0], 2)}");
            // print($"Threshold values, {distance_threshold}, {orientational_threshold}, {force_threshold}");
            if ((distance_threshold > 0.0001) || (orientational_threshold > 0.0001) || (force_threshold > 0.0001))
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
            //else
            //{
            //    operator_data_saved = operator_data_saved;
            //    // print(operator_data_saved[0]);
            //}
            // print($"Saved,{operator_data_saved[0]}, {operator_data_saved[1]}, {operator_data_saved[2]}, {operator_data_saved[3]}, {operator_data_saved[4]}, {operator_data_saved[5]}");

        }

    }


}

