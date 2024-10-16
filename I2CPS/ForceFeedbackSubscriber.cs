using JetBrains.Annotations;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class ForceFeedbackSubscriber : UnitySubscriber<MessageTypes.robotoq_ft_sensor.ft_sensor>
    {
        public bool isMessageReceived;
        public double Fx;
        public double Fy;
        public double Fz;
        public double Mx;
        public double My;
        public double Mz;
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

        protected override void ReceiveMessage(MessageTypes.robotoq_ft_sensor.ft_sensor message)
        {
            //print($"msg FT_sensor,{message.Fx}");
            Fx = (double)message.Fx;
            Fy = (double)message.Fy;
            Fz = (double)message.Fz;
            Mx = (double)message.Mx;
            My = (double)message.My;
            Mz = (double)message.Mz;

            isMessageReceived = true;
        }

        private void ProcessMessage()
        {
            isMessageReceived = false;
        }
    }


}
