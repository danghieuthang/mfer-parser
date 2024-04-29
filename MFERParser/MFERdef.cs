namespace MFERParser
{
    public class MFERdef
    {
        public const int MAX_WAVE_CHANNEL = 128;
        public const int MAX_EVENT_CNT = 200000;
        public const int MWF_ZRO = 0;
        public const int MWF_BLE = 1;
        public const int MWF_VER = 2;
        public const int MWF_TXC = 3;
        public const int MWF_BLK = 4;
        public const int MWF_CHN = 5;
        public const int MWF_SEQ = 6;
        public const int MWF_PNT = 7;
        public const int MWF_WFM = 8;
        public const int MWF_LDN = 9;
        public const int MWF_DTP = 10;
        public const int MWF_IVL = 11;
        public const int MWF_SEN = 12;
        public const int MWF_OFF = 13;
        public const int MWF_CMP = 14;
        public const int MWF_IPD = 15;
        public const int MWF_SKW_L1 = 16;
        public const int MWF_FLT = 17;
        public const int MWF_NUL = 18;
        public const int MWF_EVT_L1 = 19;
        public const int MWF_VAL_L1 = 20;
        public const int MWF_INF = 21;
        public const int MWF_NTE = 22;
        public const int MWF_MAN = 23;
        public const int MWF_PNM_L1 = 26;
        public const int MWF_PID_L1 = 27;
        public const int MWF_TIM_L1 = 28;
        public const int MWF_MSS_L1 = 29;
        public const int MWF_WAV = 30;
        public const int MWF_ATT = 63;
        public const int MWF_PRE = 64;
        public const int MWF_EVT = 65;
        public const int MWF_VAL = 66;
        public const int MWF_SKW = 67;
        public const int MWF_SET = 103;
        public const int MWF_RPT = 104;
        public const int MWF_SIG = 105;
        public const int MWF_END = 128;
        public const int MWF_PNM = 129;
        public const int MWF_PID = 130;
        public const int MWF_AGE = 131;
        public const int MWF_SEX = 132;
        public const int MWF_TIM = 133;
        public const int MWF_MSS = 134;
        public const int MWF_UID = 135;
        public const int MWF_MAP = 136;
        public const int MWF_STD12 = 1;
        public const int MWF_LONG_TERM = 2;
        public const int MWF_EEG = 40;
        public const int MWF_MONT = 20;
        public const int MWF_PCG = 30;
        public const int MWF_ECG12_1 = 1;
        public const int MWF_ECG12_2 = 2;
        public const int MWF_ECG12_V1 = 3;
        public const int MWF_ECG12_V2 = 4;
        public const int MWF_ECG12_V3 = 5;
        public const int MWF_ECG12_V4 = 6;
        public const int MWF_ECG12_V5 = 7;
        public const int MWF_ECG12_V6 = 8;
        public const int MWF_ECG12_V7 = 9;
        public const int MWF_ECG12_V3R = 11;
        public const int MWF_ECG12_V4R = 12;
        public const int MWF_ECG12_V5R = 13;
        public const int MWF_ECG12_V6R = 14;
        public const int MWF_ECG12_V7R = 15;
        public const int MWF_ECG12_3 = 61;
        public const int MWF_ECG12_aVR = 62;
        public const int MWF_ECG12_aVL = 63;
        public const int MWF_ECG12_aVF = 64;
        public const int MWF_ECG12_V8 = 66;
        public const int MWF_ECG12_V9 = 67;
        public const int MWF_ECG12_V8R = 68;
        public const int MWF_ECG12_V9R = 69;
        public const int MWF_PCG_O = 512;
        public const int MWF_PCG_L = 513;
        public const int MWF_PCG_M = 514;
        public const int MWF_PCG_H = 515;
        public const int MWF_FQ_HZ = 0;
        public const int MWF_FQ_1u = 1;
        public const int MWF_FQ_1m = 2;
        public const int MWF_FQ_1s = 3;
        public const int MWF_SN_nV = 0;
        public const int MWF_SN_uV = 1;
        public const int MWF_SN_mV = 2;
        public const int MWF_SN_V = 3;
        public const int MWF_AHA_DIF = 1;
        public static int[] ECGleadCode = new int[22]
        {
          1,
          2,
          3,
          4,
          5,
          6,
          7,
          8,
          9,
          11,
          12,
          13,
          14,
          15,
          61,
          62,
          63,
          64,
          66,
          67,
          68,
          69
        };
        public static string[] ECGleadName = new string[22]
        {
          "I",
          "II",
          "V1",
          "V2",
          "V3",
          "V4",
          "V5",
          "V6",
          "V7",
          "V3R",
          "V4R",
          "V5R",
          "V6R",
          "V7R",
          "III",
          "aVR",
          "aVL",
          "aVF",
          "V8",
          "V9",
          "V8R",
          "V9R"
        };
        public static int[] WaveformCode = new int[5]
        {
          1,
          2,
          40,
          20,
          30
        };
        public static string[] WaveformName = new string[5]
        {
          "Standard 12 lead ECG",
          "Long term ECG",
          "EEG",
          "Monitor",
          "PCG"
        };
        public static int[] dTypeCode = new int[7]
        {
          0,
          1,
          2,
          3,
          4,
          5,
          6
        };
        public static string[] dTypeName = new string[10]
        {
          "Signed 16 bits integer",
          "Unsigned 16 bits integer",
          "Signed 32 bits integer",
          "Unsigned 8 bits integer",
          "16 bits status",
          "Signed 8 bits integer",
          "Unsigned 32 bits integer",
          "32 bitst single-precision floating(IEEE754)",
          "64 bits double-precision floating(IEEE754)",
          "8 bits AHA differential"
        };
    }
}