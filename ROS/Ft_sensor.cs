
namespace RosSharp.RosBridgeClient.MessageTypes.robotoq_ft_sensor
{
    public class ft_sensor : Message
    {
        public const string RosMessageName = "robotiq_ft_sensor/ft_sensor";

        public float Fx { get; set; }
        public float Fy { get; set; }
        public float Fz { get; set; }
        public float Mx { get; set; }
        public float My { get; set; }
        public float Mz { get; set; }

        public ft_sensor()
        {
            this.Fx = 0.0f;
            this.Fy = 0.0f;
            this.Fz = 0.0f;
            this.Mx = 0.0f;
            this.My = 0.0f;
            this.Mz = 0.0f;
        }

        public ft_sensor(float Fx, float Fy, float Fz, float Mx, float My, float Mz)
        {
            this.Fx = Fx;
            this.Fy = Fy;
            this.Fz = Fz;
            this.Mx = Mx;
            this.My = My;
            this.Mz = Mz;
        }
    }
}
