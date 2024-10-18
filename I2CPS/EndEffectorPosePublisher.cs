using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class EndEffectorPosePublisher : UnityPublisher<MessageTypes.Geometry.Pose>
    {
        public Transform PublishedTransform;
       
        private MessageTypes.Geometry.Pose message;
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
            if (Pose_mode == 1)
                UpdateMessage();
            else
                return;
        }

        private void UpdateKeys()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                print("Determining Robot's Orientation");
                if (Pose_mode == 0)
                    Pose_mode = 1;
                else
                    Pose_mode = 0;
            }
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.Pose();
           
        }

        private void UpdateMessage()
        {
           
            // Get position and rotation matrices from the Unity Transform
            Matrix4x4 localToWorldMatrix = PublishedTransform.localToWorldMatrix;

            // Extract position and rotation from the matrices
            Vector3 position = localToWorldMatrix.GetColumn(3);
            Quaternion rotation = Quaternion.LookRotation(localToWorldMatrix.GetColumn(2), localToWorldMatrix.GetColumn(1));

            // Convert Quaternion to Euler angles
            Vector3 euler = rotation.eulerAngles;

            // Set the PoseStamped message
            GetGeometryEuler(euler, message.orientation);

            Publish(message);
        }

        private static void GetGeometryEuler(Vector3 euler, MessageTypes.Geometry.Quaternion geometryQuaternion)
        {
            
            // Set the Quaternion in the message
            geometryQuaternion.x = euler.x;
            geometryQuaternion.y = euler.y;
            geometryQuaternion.z = euler.z;
            geometryQuaternion.w = 0.0f;
            print($"See EEF Orientation, {euler.x}, {euler.y}, {euler.z}");        
        }
    }
}
