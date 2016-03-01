namespace DevsDNA.Drone.Devices.BMP085
{
    /// <summary>
    /// Bmp085CalibrationDataConst
    /// </summary>
    public static class Bmp085CalibrationDataConst
    {
        public static readonly byte BMP085_REGISTER_CAL_AC1 = 0xAA;  // R   Calibration data (16 bits)
        public static readonly byte BMP085_REGISTER_CAL_AC2 = 0xAC;  // R   Calibration data (16 bits)
        public static readonly byte BMP085_REGISTER_CAL_AC3 = 0xAE;  // R   Calibration data (16 bits)
        public static readonly byte BMP085_REGISTER_CAL_AC4 = 0xB0;  // R   Calibration data (16 bits)
        public static readonly byte BMP085_REGISTER_CAL_AC5 = 0xB2;  // R   Calibration data (16 bits)
        public static readonly byte BMP085_REGISTER_CAL_AC6 = 0xB4;  // R   Calibration data (16 bits)
        public static readonly byte BMP085_REGISTER_CAL_B1 = 0xB6;   // R   Calibration data (16 bits)
        public static readonly byte BMP085_REGISTER_CAL_B2 = 0xB8;   // R   Calibration data (16 bits)
        public static readonly byte BMP085_REGISTER_CAL_MB = 0xBA;   // R   Calibration data (16 bits)
        public static readonly byte BMP085_REGISTER_CAL_MC = 0xBC;   // R   Calibration data (16 bits)
        public static readonly byte BMP085_REGISTER_CAL_MD = 0xBE;   // R   Calibration data (16 bits)
        public static readonly byte BMP085_REGISTER_CHIPID = 0xD0;
        public static readonly byte BMP085_REGISTER_VERSION = 0xD1;
        public static readonly byte BMP085_REGISTER_SOFTRESET = 0xE0;
        public static readonly byte BMP085_REGISTER_CONTROL = 0xF4;
        public static readonly byte BMP085_REGISTER_TEMPDATA = 0xF6;
        public static readonly byte BMP085_REGISTER_PRESSUREDATA = 0xF6;
        public static readonly byte BMP085_REGISTER_READTEMPCMD = 0x2E;
        public static readonly byte BMP085_REGISTER_READPRESSURECMD = 0x34;
    }
}
