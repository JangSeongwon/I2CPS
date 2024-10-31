using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class HomeOriPublisher : UnityPublisher<MessageTypes.Geometry.Pose>
    {
        private MessageTypes.Geometry.Pose message_home;
        public HapticOriPublisher HapticOriPublisher;
        public int Home_mode;

        protected override void Start()
        {
            base.Start();
            HapticOriPublisher = (RosSharp.RosBridgeClient.HapticOriPublisher)this.GetComponentInParent(typeof(RosSharp.RosBridgeClient.HapticOriPublisher));
            Home_mode = HapticOriPublisher.Orientation_mode;
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            Home_mode = HapticOriPublisher.Orientation_mode;
            if (Home_mode == 2)
            {
                SendMessageHome();
            }
        }

        private void InitializeMessage()
        {
            message_home = new MessageTypes.Geometry.Pose();
        }

        private void SendMessageHome()
        {
            Vector3 euler = new Vector3 ( 0, 0, 0);
            GetGeometryEuler(euler, message_home.orientation);
            Publish(message_home);
        }

        private static void GetGeometryEuler(Vector3 euler, MessageTypes.Geometry.Quaternion geometryQuaternion)
        {
            if (euler.x >= 180)
                geometryQuaternion.x = euler.x - 360;
            else
                geometryQuaternion.x = euler.x;
            if (euler.y >= 180)
                geometryQuaternion.y = euler.y - 360;
            else
                geometryQuaternion.y = euler.y;
            if (euler.z >= 180)
                geometryQuaternion.z = euler.z - 360;
            else
                geometryQuaternion.z = euler.z;
            geometryQuaternion.w = 0.0f;
            // print($"See Haptic Orientation, X: {euler.x}, Y: {euler.y}, Z: {euler.z}");
            // print($"See Haptic Orientation, {geometryQuaternion.x}, {geometryQuaternion.y}, {geometryQuaternion.z}");        

        }
    }
}
