namespace DevsDNA.Drone.Devices.BMP085
{
    /// <summary>
    /// Calibration data
    /// https://github.com/adafruit/Adafruit_BMP085_Unified/blob/master/Adafruit_BMP085_U.h
    /// </summary>
    public class Bmp085CalibrationData
    {
       

        public short ac1;
        public short ac2;
        public short ac3;
        public ushort ac4;
        public ushort ac5;
        public ushort ac6;
        public short b1;
        public short b2;
        public short mb;
        public short mc;
        public short md;
    }
}
