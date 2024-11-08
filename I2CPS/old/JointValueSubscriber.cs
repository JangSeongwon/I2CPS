using JetBrains.Annotations;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class JointValueSubscriber : UnitySubscriber<MessageTypes.Sensor.JointState>
    {
        public bool isMessageReceived;
        public float J1;
        public float J2;
        public float J3;
        public float J4;
        public float J5;
        public float J6;
        public double[] position;

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

        protected override void ReceiveMessage(MessageTypes.Sensor.JointState message)
        {
            position = message.position;
            // print("getting joint as topic");

            J1 = (float)position[0];
            J2 = (float)position[1];
            J3 = (float)position[2];
            J4 = (float)position[3];
            J5 = (float)position[4];
            J6 = (float)position[5];

            isMessageReceived = true;
        }

        private void ProcessMessage()
        {
            isMessageReceived = false;
        }
    }


}
