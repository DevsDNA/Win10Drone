namespace DevsDNA.Drone.Commons
{
    /// <summary>
    /// CommonsKeys
    /// </summary>
    public static class CommonsKeys
    {
        public static readonly string I2C_DEVICE_ID = "\\\\?\\ACPI#MSFT8000#1#{a11ee3c6-8421-4202-a3e7-b91ff90188e4}\\I2C1";

        public static double SENSORS_GRAVITY_EARTH = (9.80665F);
        public static byte SENSORS_GAUSS_TO_MICROTESLA = (100);

        public static readonly int PCA9685_DEFAULTADDRESS = (0x40);

        public static readonly int ACCEL_DEFAULTADDRESS = (0x32 >> 1);
        public static readonly int MAG_DEFAULTADDRESS = (0x3C >> 1);

        public static readonly int GYRO_DEFAULTADDRESS = (0x6B);
        public static readonly int BAR_DEFAULTADDRESS = (0x77);
        
    }
}
