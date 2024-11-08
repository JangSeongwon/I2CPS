using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{

    public class CoordinateInterface : MonoBehaviour
    {
        public GameObject HapticCoord;
        public Transform CoordinateTransform;
        public Vector3 Coordinate;
        public float Sensitivity_x;
        public float Sensitivity_y;
        public float Sensitivity_z;

        void Start()
        {
        }

        void Update()
        {
            HapticCoord = GameObject.Find("HapticEEF");
            Quaternion newRotation = HapticCoord.transform.rotation;
            //Vector3 euler = newRotation.eulerAngles;
            //euler.x = euler.x * (Sensitivity_x);
            //euler.y = euler.y * (Sensitivity_y);
            //euler.z = euler.z * (Sensitivity_z);
            //newRotation = Quaternion.Euler(euler);
            //Debug.Log($"See Orientation, {newRotation}");
            CoordinateTransform.rotation = newRotation;
        }
    }
}
