using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class HapticOriPublisher : UnityPublisher<MessageTypes.Geometry.Pose>
    {
        public Transform PublishedTransform;
       
        private MessageTypes.Geometry.Pose message;
        public int Pose_mode;
        public int Orientation_mode;
        public Vector3 Saved_Ori_i;
        public Vector3 Saved_Ori;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
            Orientation_mode = 0;
            Pose_mode = 0;
        }

        private void FixedUpdate()
        {
            if (Orientation_mode == 1)
            {
                UpdateMessage();
            }
        }

        public void orientation_change()
        {
            Pose_mode = 1;
            Saving_Initial_Ori();
            Orientation_mode = 1;
            print("Robot's Orientation Modification with Haptic X,Z Rotation");
        }
        public void orientation_fix()
        {
            Orientation_mode = 0;
            Pose_mode = 0;
            print("Fix Robot's Orientation");
        }
        public void orientation_home()
        {
            Pose_mode = 1;
            Orientation_mode = 2;
            print("Return to Robot's Home Orientation");
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.Pose();

        }

        private void Saving_Initial_Ori()
        {
            Quaternion rotation = PublishedTransform.rotation;
            Saved_Ori_i = rotation.eulerAngles;

            if (Saved_Ori_i.x >= 180)
                Saved_Ori.x = Saved_Ori_i.x - 360;
            else
                Saved_Ori.x = Saved_Ori_i.x;
            if (Saved_Ori_i.y >= 180)
                Saved_Ori.y = Saved_Ori_i.y - 360;
            else
                Saved_Ori.y = Saved_Ori_i.y;
            if (Saved_Ori_i.z >= 180)
                Saved_Ori.z = Saved_Ori_i.z - 360;
            else
                Saved_Ori.z = Saved_Ori_i.z;
            print($"Saving Current Haptic Orientation, {Saved_Ori}");
        }

        private void UpdateMessage()
        {
            // Get position and rotation matrices from the Unity Transform
            //Matrix4x4 localToWorldMatrix = PublishedTransform.localToWorldMatrix;
            // Extract rotation from the matrices
            //Quaternion rotation = Quaternion.LookRotation(localToWorldMatrix.GetColumn(2), localToWorldMatrix.GetColumn(1));

            Quaternion rotation = PublishedTransform.rotation;

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
            if (euler.x >= 180)
                geometryQuaternion.x = euler.x - 360 - Saved_Ori.x;
            else
                geometryQuaternion.x = euler.x - Saved_Ori.x;
            if (euler.y >= 180)
                geometryQuaternion.y = euler.y - 360 - Saved_Ori.y;
            else
                geometryQuaternion.y = euler.y - Saved_Ori.y;
            if (euler.z >= 180)
                geometryQuaternion.z = euler.z - 360 - Saved_Ori.z;
            else
                geometryQuaternion.z = euler.z - Saved_Ori.z;
            geometryQuaternion.w = 0.0f;
            // print($"See Haptic Orientation, X: {euler.x}, {Saved_Ori.x}, Y: {euler.y}, {Saved_Ori.y}, Z: {euler.z}, {Saved_Ori.z}");
            // print($"See Haptic Orientation, {geometryQuaternion.x}, {geometryQuaternion.y}, {geometryQuaternion.z}");        
        }
    }
}
