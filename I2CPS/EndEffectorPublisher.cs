using UnityEngine;
using System;
using System.Threading;

namespace RosSharp.RosBridgeClient
{
    public class EndEffectorPublisher : UnityPublisher<MessageTypes.Geometry.PoseStamped>
    {
        public Transform PublishedTransform;
        public string FrameId = "Unity";

        private MessageTypes.Geometry.PoseStamped message;
        public int Pose_mode;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
            Pose_mode = 0;
        }

        private void FixedUpdate()
        {
            UpdateKeys();
            if (Pose_mode == 0)
                UpdateMessage();
            else
                return;
        }

        private void UpdateKeys()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                print("Fix the Robot's Position");
                if (Pose_mode == 0)
                    Pose_mode = 1;
                else
                    Pose_mode = 0;
            }
        }

            private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.PoseStamped
            {
                header = new MessageTypes.Std.Header()
                {
                    frame_id = FrameId
                }
            };
        }

        private void UpdateMessage()
        {
            message.header.Update();

            // Get position and rotation matrices from the Unity Transform
            Matrix4x4 localToWorldMatrix = PublishedTransform.localToWorldMatrix;

            // Extract position and rotation from the matrices
            Vector3 position = localToWorldMatrix.GetColumn(3);
            Quaternion rotation = Quaternion.LookRotation(localToWorldMatrix.GetColumn(2), localToWorldMatrix.GetColumn(1));

            // Convert Quaternion to Euler angles
            Vector3 euler = rotation.eulerAngles;

            // Set the PoseStamped message
            GetGeometryPoint(position, message.pose.position);
            GetGeometryEuler(euler, message.pose.orientation);

            Publish(message);
        }

        private static void GetGeometryPoint(Vector3 position, MessageTypes.Geometry.Point geometryPoint)
        {
            geometryPoint.x = position.x - 3;
            geometryPoint.y = position.y + 0.5;
            geometryPoint.z = position.z;
        }

        private static void GetGeometryEuler(Vector3 euler, MessageTypes.Geometry.Quaternion geometryQuaternion)
        {
            // Convert Euler angles to Quaternion
            // Quaternion quaternion = Quaternion.Euler(euler);

            // Set the Quaternion in the message
            geometryQuaternion.x = euler.x;
            geometryQuaternion.y = euler.y;
            geometryQuaternion.z = euler.z;
            geometryQuaternion.w = 0.0f;
            
        }

    }
}
