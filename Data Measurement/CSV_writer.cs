using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using RosSharp.RosBridgeClient;


public class CSV_writer : MonoBehaviour
{
    string filename = "";
    public Transform HapticEef;
    public Vector3 HapticEefPOS;
    public List<Vector3> pos = new List<Vector3>();

    void Start()
    {
        filename = Application.dataPath + "/Scripts/Haptic_plots.csv";
    }
    private void UpdateKeys()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print("Get 3D Plots");
            Write_Csv();
        }
    }

        // Update is called once per frame
        void Update()
    {
        UpdateKeys();
        HapticEefPOS = HapticEef.position;
        Record_plots();
    }

    public void Record_plots()
    {
        pos.Add(HapticEefPOS);
    }

    public void Write_Csv()
    {
        TextWriter tw = new StreamWriter(filename, false);
        tw.WriteLine("X, Y, Z");
        tw.Close();
        
        tw = new StreamWriter(filename, true);
        for (int i = 0; i <= 1000000; i++)
        {
            print($"See plots {i}, {pos[i]}");
            tw.WriteLine(pos[i]);
        }
        tw.Close();

    }
}

