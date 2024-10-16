using JetBrains.Annotations;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class ForceFeedbackSubscriber : UnitySubscriber<MessageTypes.RobotiqFtSensor.Ft_sensor>
    {
        public bool isMessageReceived;
        public float Fx;
        public float Fy;
        public float Fz;
        public float Mx;
        public float My;
        public float Mz;
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

        protected override void ReceiveMessage(MessageTypes.RobotiqFtSensor.Ft_sensor message)
        {
            print($"msg FT_sensor,{message}");
            //Fx = (double)message[0];
            //Fy = (double)message[1];
            //Fz = (double)message[2];
            //Mx = (double)message[3];
            //My = (double)message[4];
            //Mz = (double)message[5];

            isMessageReceived = true;
        }

        private void ProcessMessage()
        {
            isMessageReceived = false;
        }
    }


}
