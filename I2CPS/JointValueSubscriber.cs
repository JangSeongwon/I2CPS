using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class JointValueSubscriber : UnitySubscriber<MessageTypes.Std.Float32MultiArray>
    {
        public bool isMessageReceived;
        public float J1;
        public float J2;
        public float J3;
        public float J4;
        public float J5;
        public float J6;

        protected override void Start()
        {
            base.Start();
            isMessageReceived = false;
        }

        private void Update()
        {
            if (isMessageReceived)
                ProcessMessage();
        }

        protected override void ReceiveMessage(MessageTypes.Std.Float32MultiArray message)
        {
            float[] data = message.data;

            J1 = data[0];
            J2 = data[1];
            J3 = data[2];
            J4 = data[3];
            J5 = data[4];
            J6 = data[5];

            isMessageReceived = true;
        }

        private void ProcessMessage()
        {
            isMessageReceived = false;
        }
    }


}