using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class GetHapticPOS : MonoBehaviour
    {
        public Vector3 POS;
        public Quaternion POSE;
        public Vector3 POSE2;


        void Start()
        {

        }


        void Update()
        {
            POS = transform.position;
            POSE = transform.rotation;
            POSE2 = transform.eulerAngles;
            // Debug.Log($"See Haptic POS, POSE, {POS.x}, {POS}, {POSE}, {POSE2}");

        }

        public Vector3 GetPOS()
        {
            
            return POS;

        }

    }
}
