using JetBrains.Annotations;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class ForceFeedbackSubscriber : UnitySubscriber<MessageTypes.robotiq_ft_sensor.ft_sensor>
    {
        public double Fx;
        public double Fy;
        public double Fz;
        public double Mx;
        public double My;
        public double Mz;

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(MessageTypes.robotiq_ft_sensor.ft_sensor message)
        {
            //print($"msg FT_sensor,{message.Fx}");
            Fx = (double)message.Fx;
            Fy = (double)message.Fy;
            Fz = (double)message.Fz;
            Mx = (double)message.Mx;
            My = (double)message.My;
            Mz = (double)message.Mz;
        }
    }


}
