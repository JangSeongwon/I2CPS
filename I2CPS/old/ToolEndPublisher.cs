using UnityEngine;
using System;
using System.Threading;

namespace RosSharp.RosBridgeClient
{
    public class ToolEndPublisher : UnityPublisher<MessageTypes.Geometry.PoseStamped>
    {
        public Transform ToolTransform;
        public string FrameId = "Unity";

        private MessageTypes.Geometry.PoseStamped message_tool;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message_tool = new MessageTypes.Geometry.PoseStamped
            {
                header = new MessageTypes.Std.Header()
                {
                    frame_id = FrameId
                }
            };
        }

        private void UpdateMessage()
        {
            message_tool.header.Update();

            //Get global position of the ToolEnd 
            Vector3 position = ToolTransform.position;

            // Get rotation matrices from the Unity Transform
            Matrix4x4 localToWorldMatrix = ToolTransform.localToWorldMatrix;
            // Extract rotation from the matrices
            Quaternion rotation = Quaternion.LookRotation(localToWorldMatrix.GetColumn(2), localToWorldMatrix.GetColumn(1));
            // Convert Quaternion to Euler angles
            Vector3 euler = rotation.eulerAngles;

            // Set the PoseStamped message
            GetGeometryPoint(position, message_tool.pose.position);
            GetGeometryEuler(euler, message_tool.pose.orientation);

            Publish(message_tool);
        }

        private static void GetGeometryPoint(Vector3 position, MessageTypes.Geometry.Point geometryPoint)
        {
            geometryPoint.x = position.x;
            geometryPoint.y = position.y;
            geometryPoint.z = position.z;
            //print($"See ToolEnd Position, {geometryPoint.x}, {geometryPoint.y}, {geometryPoint.z}");

        }

        private static void GetGeometryEuler(Vector3 euler, MessageTypes.Geometry.Quaternion geometryQuaternion)
        {
            // Convert Euler angles to Quaternion
            //Quaternion quaternion = Quaternion.Euler(euler);

            // Set the Quaternion in the message
            geometryQuaternion.x = euler.x;
            geometryQuaternion.y = euler.y;
            geometryQuaternion.z = euler.z;
            geometryQuaternion.w = 0.0f;
            //print($"See ToolEnd Orientation in Euler, {euler.x}, {euler.y}, {euler.z}");
            //print($"See ToolEnd Orientation in Quaternion, {geometryQuaternion.x}, {geometryQuaternion.y}, {geometryQuaternion.z}");
        }

    }
}
