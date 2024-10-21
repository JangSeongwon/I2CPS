using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class HapticOriPublisher : UnityPublisher<MessageTypes.Geometry.Pose>
    {
        public Transform PublishedTransform;
       
        private MessageTypes.Geometry.Pose message;
        public int Pose_mode;
        public Vector3 Saved_Ori;

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
                Saving_Initial_Ori();
            else if (Pose_mode == 2)
                UpdateMessage();
            else if (Pose_mode == 0)
                return;
        }

        private void UpdateKeys()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Pose_mode == 0)
                {
                    Pose_mode = 1;
                    print("Saving current Orientation");
                }
                else if (Pose_mode == 1)
                {
                    Pose_mode = 2;
                    print("Robot's Orientation Modification");
                }
                else if (Pose_mode == 2)
                    Pose_mode = 0;
            }
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.Pose();
           
        }

        private void Saving_Initial_Ori()
        {
            Matrix4x4 localToWorldMatrix = PublishedTransform.localToWorldMatrix;
            Quaternion rotation = Quaternion.LookRotation(localToWorldMatrix.GetColumn(2), localToWorldMatrix.GetColumn(1));
            Saved_Ori = rotation.eulerAngles;
            //print($"Saving current Orientation, {Saved_Ori}");

        }

        private void UpdateMessage()
        {
           
            // Get position and rotation matrices from the Unity Transform
            Matrix4x4 localToWorldMatrix = PublishedTransform.localToWorldMatrix;

            // Extract rotation from the matrices
            Quaternion rotation = Quaternion.LookRotation(localToWorldMatrix.GetColumn(2), localToWorldMatrix.GetColumn(1));

            // Convert Quaternion to Euler angles
            Vector3 euler = rotation.eulerAngles;
            // Set the PoseStamped message
            GetGeometryEuler(euler, message.orientation, Saved_Ori);
            //print($"Saved Orientation, {Saved_Ori}");

            Publish(message);
        }

        private static void GetGeometryEuler(Vector3 euler, MessageTypes.Geometry.Quaternion geometryQuaternion,Vector3 Saved_Ori)
        {
            
            // Set the Quaternion in the message
            geometryQuaternion.x = euler.x - Saved_Ori.x;
            geometryQuaternion.y = euler.y - Saved_Ori.y;
            geometryQuaternion.z = euler.z - Saved_Ori.z;
            geometryQuaternion.w = 0.0f;
            print($"See Haptic Orientation, {geometryQuaternion.x}, {geometryQuaternion.y}, {geometryQuaternion.z}");        
        }
    }
}
