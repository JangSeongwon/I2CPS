
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class OperatorTrajSubscriber : UnitySubscriber<MessageTypes.Geometry.PoseStamped>
    {
        private Vector3 position;
        private Quaternion rotation;

        public float x;
        public float y;
        public float z;
        public float ori_x;
        public float ori_y;
        public float ori_z;

        protected override void Start()
        {
			base.Start();
        }
		
        protected override void ReceiveMessage(MessageTypes.Geometry.PoseStamped message)
        {
            position = GetPosition(message).Ros2Unity();
            rotation = GetRotation(message).Ros2Unity();
            x = (float)position.x;
            y = (float)position.y;
            z = (float)position.z;
            ori_x = (float)rotation.x;
            ori_y = (float)rotation.y;
            ori_z = (float)rotation.z;
            //print("Message Received");
        }

        private Vector3 GetPosition(MessageTypes.Geometry.PoseStamped message)
        {
            return new Vector3(
                (float)message.pose.position.x,
                (float)message.pose.position.y,
                (float)message.pose.position.z
                );
        }

        private Quaternion GetRotation(MessageTypes.Geometry.PoseStamped message)
        {
            return new Quaternion(
                (float)message.pose.orientation.x,
                (float)message.pose.orientation.y,
                (float)message.pose.orientation.z,
                (float)message.pose.orientation.w);
        }
    }
}
