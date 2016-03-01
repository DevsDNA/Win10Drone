namespace DevsDNA.Drone.Interfaces
{
    public interface IDifferential
    {
        void SetControl(double thrust, double pitch, double roll, double yaw);
    }
}
