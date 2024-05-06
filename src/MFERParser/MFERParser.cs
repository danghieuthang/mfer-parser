using System.Text;

namespace MFERParser
{
    /// <summary>
    /// Defines the MFER file format parser
    /// </summary>
    public class MferParser
    {
        /// <summary>
        /// Determines the endianess of the data, <c>false</c> for big endian and <c>true</c> for little endian
        /// </summary>
        private bool endian;
        private int frm;
        private byte[] bf;
        private MferFile mferFile;

        public MferParser()
        {
        }

        public MferFile Parse(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                return null;
            }
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            long length = fileStream.Length;
            this.bf = new byte[length];
            int num = fileStream.Read(this.bf, 0, (int)length);
            fileStream.Close();
            if (num <= 0)
                return null;
            this.endian = false;
            int point = 0;
            this.frm = 0;
            this.TLVanz(0, 0, point, (int)length, this.bf);
            return mferFile;
        }

        private MferFile GetMferFileInstance => this.mferFile ??= new();


        private int TLVanz(int mode, int ch, int point, int dtLen, byte[] bf)
        {
            MferFile mferFile = null;
            while (dtLen > point)
            {
                try
                {
                    if (mferFile == null)
                    {
                        mferFile = GetMferFileInstance;
                    }
                    if (bf[point] == MFERdef.MWF_END)
                        return 0;
                    int num1;
                    int len;
                    if (bf[point] == MFERdef.MWF_ZRO)
                    {
                        num1 = 0;
                        string str = "00 ";
                        for (len = 1; bf[point + len] == (byte)0 && dtLen > point + len; ++len)
                        {
                            if (str.Length <= 256)
                                str += "00 ";
                        }
                    }
                    else
                    {
                        int num2 = 1;
                        if ((bf[point] & 32) > 0)
                        {
                            if (bf[point] == MFERdef.MWF_ATT)
                                ++num2;
                            while (((int)bf[point + num2] & 128) > 0)
                                ++num2;
                        }
                        len = (int)bf[point + num2];
                        int num3 = 1;
                        if ((len & 128) != 0)
                        {
                            int num4 = len & (int)sbyte.MaxValue;
                            if (num4 > 4)
                            {
#if DEBUG
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Data length[{num4}] error");
                                Console.ResetColor();
#endif
                                return -1;
                            }
                            len = 0;
                            for (int index = 1; index <= num4; ++index)
                            {
                                len = len * 256 + (int)bf[point + num2 + index];
                                ++num3;
                            }
                        }
                        int num5 = mode;
                        int num6 = Math.Min(num2 + len, 256);
                        num1 = num2 + num3;
                        int dt;
                        switch (bf[point])
                        {
                            // MWF_BLE
                            case MFERdef.MWF_BLE:
                                // Big/Little endian
                                if (len < 2)
                                {
                                    this.GetData(out dt, point, bf);
                                    switch (dt)
                                    {
                                        case 0:
                                            // Big endian
                                            this.endian = false;
                                            break;
                                        case 1:
                                            // Little endian
                                            this.endian = true;
                                            break;
                                        default:
#if DEBUG
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine($"Data error");
                                            Console.ResetColor();
#endif
                                            break;
                                    }
                                }
                                else
                                {
#if DEBUG
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"Data length error");
                                    Console.ResetColor();
#endif
                                    break;
                                }
                                break;
                            case MFERdef.MWF_VER:
                                mferFile.Version = bf[point + num1].ToString() + "." + bf[point + num1 + 1].ToString() + "." + bf[point + num1 + 2].ToString();
                                break;
                            // MWF_BLK
                            case MFERdef.MWF_BLK:
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.Block = dt;
                                break;
                            //MWF_CHN
                            case MFERdef.MWF_CHN:
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.Channel = dt;
                                break;
                            //MWF_SEQ
                            case MFERdef.MWF_SEQ:
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.Sequence = dt;
                                break;
                            case MFERdef.MWF_PNT:
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.DataPointer = dt;
                                break;
                            // MWF_WFM
                            case MFERdef.MWF_WFM:
                                // Waveform identifier
                                this.GetData16(out dt, len, point + num1, bf);
                                mferFile.WaveformIdentifier = dt;
                                string waveformName = len.ToString();
                                int i;
                                for (i = 0; i < MFERdef.WaveformCode.Length; ++i)
                                {
                                    if (MFERdef.WaveformCode[i] == dt)
                                    {
                                        waveformName = MFERdef.WaveformName[i];
                                        break;
                                    }
                                }
                                if (i >= MFERdef.WaveformCode.Length)
                                {
                                    waveformName = "Undefined waveform(" + dt.ToString() + ")";
                                }
                                if (len > 2)
                                {
                                    string str = waveformName + "[";
                                    for (int index2 = 2; index2 < len; ++index2)
                                        str += (char)bf[point + num3 + index2];
                                    waveformName = str + "]";
                                }
                                mferFile.WaveformName = dt;
                                break;
                            // MWF_LDN
                            case MFERdef.MWF_LDN:
                                this.GetData16(out dt, len, point + num1, bf);
                                mferFile.WaveLead = dt;
                                string ecgLeadName = len.ToString();
                                for (i = 0; i < MFERdef.ECGleadCode.Length; ++i)
                                {
                                    if (MFERdef.ECGleadCode[i] == dt)
                                    {
                                        ecgLeadName = "Lead " + MFERdef.ECGleadName[i];
                                        break;
                                    }
                                }
                                if (i >= MFERdef.ECGleadCode.Length)
                                    ecgLeadName = "Undefine code(" + dt.ToString() + ")";
                                if (len > 2)
                                {
                                    string str = ecgLeadName + "[";
                                    for (int index4 = 2; index4 < len; ++index4)
                                        str += (char)bf[point + num3 + index4];
                                    ecgLeadName = str + "]";
                                }
                                mferFile.ECGLeadName = ecgLeadName;
                                break;
                            // MWF_DTP
                            case MFERdef.MWF_DTP:
                                this.GetData16(out dt, len, point + num1, bf);
                                mferFile.DType = dt;
                                //int index3 = 0;
                                //while (index3 < MFERdef.dTypeCode.Length && MFERdef.dTypeCode[index3] != dt)
                                //    ++index3;
                                break;
                            // MWF_IVL
                            case MFERdef.MWF_IVL:
                                if (len <= 6 && len >= 3)
                                {
                                    sbyte y = (sbyte)bf[point + num1 + 1];
                                    this.GetData32(out dt, len - 2, point + num1 + 2, bf);
                                    double num7 = (double)dt * Math.Pow(10.0, (double)y);
                                    double num8;
                                    if (bf[point + num1] == 0)
                                    {
                                        mferFile.Sampling = 1.0 / num7;
                                        mferFile.SamplingFrequency = num7;
                                    }
                                    else if (bf[point + num1] == (byte)1)
                                    {
                                        mferFile.Sampling = 1.0 / num7;
                                        num8 = num7 * 1000.0;
                                        mferFile.SamplingInterval = num8;
                                    }
                                    else if (bf[point + num1] == 2)
                                    {
                                        num8 = num7 * 1000.0;
                                    }
                                    else
                                    {
                                        num8 = 0.0;
#if DEBUG
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($"Definition definition error");
                                        Console.ResetColor();
#endif
                                    }
                                    break;
                                }
                                else
                                {
#if DEBUG
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"Data length  error");
                                    Console.ResetColor();
#endif
                                }
                                break;
                            // MWF_SEN
                            case MFERdef.MWF_SEN:
                                if (len <= 6 && len >= 3)
                                {
                                    sbyte y = (sbyte)bf[point + num1 + 1];
                                    this.GetData32(out dt, len - 2, point + num1 + 2, bf);
                                    double num9 = (double)dt * Math.Pow(10.0, (double)y);
                                    if (bf[point + num1] == 0)
                                    {
                                        num9 *= 1000000.0;
                                    }
                                    if (bf[point + num1] <= 12)
                                    {
                                        mferFile.Resolution = num9;
                                        mferFile.ResolutionType = MFERdef.ResolutionType[bf[point + num1]];
                                    }
                                    else
                                    {
                                        num9 = 0.0;
#if DEBUG
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($"Definition definition error");
                                        Console.ResetColor();
#endif
                                    }
                                }

                                else
                                    break;
                                break;
                            case MFERdef.MWF_OFF:
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.Offset = dt;
                                break;
                            // MWF_CMP
                            case MFERdef.MWF_CMP:
                                this.GetData16(out dt, len, point + num1, bf);
                                switch (dt)
                                {
                                    case 0:
#if DEBUG
                                        Console.WriteLine("No compression");
#endif
                                        break;
                                    case 1:
                                        mferFile.DType = 9;
                                        break;

                                }
                                break;
                            // MWF_FLT
                            case MFERdef.MWF_FLT:
                                // filter
                                string filter = "";
                                for (i = 0; i < len; ++i)
                                    filter += (char)bf[point + num1 + i];
                                mferFile.Filter = filter;
                                break;
                            // MWF_NUL
                            case MFERdef.MWF_NUL:
                                // Null value
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.Nil = dt;
                                break;
                            case MFERdef.MWF_EVT_L1:
                            case MFERdef.MWF_EVT:
                                break;
                            case MFERdef.MWF_NTE:
                                // comment
                                string comment = "";
                                for (i = 0; i < len; ++i)
                                    comment += (char)bf[point + num1 + i];
                                mferFile.Comment = comment;
                                break;
                            case MFERdef.MWF_MAN:
                                // Manufacturer
                                string manufacturer = "";
                                for (i = 0; i < len; ++i)
                                    manufacturer += (char)bf[point + num1 + i];
                                mferFile.Manufacturer = manufacturer;
                                break;
                            case MFERdef.MWF_PNM_L1:
                                // Patient Name(old)
                                string patientName = "";
                                for (i = 0; i < len; ++i)
                                    patientName += (char)bf[point + num1 + i];
                                mferFile.PatientName = patientName;
                                break;
                            // MWF_WAV
                            case MFERdef.MWF_WAV:
                                int num11 = 1;
                                var defInf0 = GetMferFileInstance;
                                if (defInf0.DType != 9)
                                    num11 = 2;
                                int num12 = defInf0.Sequence * defInf0.Block / defInf0.Channel;
                                if (num12 < len / defInf0.Channel / num11)
                                    num12 = len / defInf0.Channel / num11;
                                if (defInf0.Block == 1 && defInf0.Sequence == 1)
                                    defInf0.Sequence = num12;
                                for (int chn = 1; chn <= defInf0.Channel; ++chn)
                                {
                                    var defInfChn = GetMferFileInstance;
                                    if (defInfChn.Block == 1 && defInfChn.Sequence == 1)
                                        defInfChn.Sequence = num12;
                                    defInfChn.SetWave(chn, this.frm, new short[defInfChn.Block * defInfChn.Sequence]);
                                    defInfChn.pData = 0;
                                }
                                int num13 = 0;
                                int sequence = defInf0.Sequence;
                                for (int index4 = 0; index4 < sequence; ++index4)
                                {
                                    int channel = defInf0.Channel;
                                    for (int chn = 1; chn <= channel; ++chn)
                                    {
                                        var defInfChn = this.GetMferFileInstance;

                                        for (int index5 = 0; index5 < defInfChn.Block; ++index5)
                                        {
                                            if (len > num13 + num1 && point + num1 + num13 < dtLen)
                                            {
                                                if (defInfChn.DType == 9)
                                                {
                                                    dt = (int)bf[point + num1 + num13];
                                                    if (dt >= 128)
                                                        dt -= 256;
                                                    defInfChn.pData += dt;
                                                    ++num13;
                                                    defInfChn.GetWave(chn, this.frm)[index4 * defInfChn.Block + index5] = (short)defInfChn.pData;
                                                }
                                                else
                                                {
                                                    this.GetData16(out dt, 2, point + num1 + num13, bf);
                                                    num13 += 2;
                                                    defInfChn.GetWave(chn, this.frm)[index4 * defInfChn.Block + index5] = (short)dt;
                                                }
                                            }
                                            else
                                                goto label_112;
                                        }
                                    }
                                }
                            label_112:
                                if (ch == 0)
                                    ++this.frm;
                                int num14 = len / num11;
                                mferFile = null;
                                break;
                            // MWF_ATT
                            case MFERdef.MWF_ATT:
#if DEBUG
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Channel(" + bf[point + 1].ToString() + ") attribute");
                                Console.ResetColor();
#endif
                                this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num1, point + num1 + len, bf);
                                break;
                            case MFERdef.MWF_PRE:
                                //Preamble
                                string preamble = "";
                                for (i = 0; i < num1 + len; ++i)
                                    preamble += (char)bf[point + i];
                                mferFile.Preamble = preamble;
                                break;
                            // MWF_SET
                            case MFERdef.MWF_SET:
                                // Group
                                this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num1, point + num1 + len, bf);
                                break;
                            // MWF_END
                            case MFERdef.MWF_END:
                                // End of format
                                return 0;
                            case MFERdef.MWF_PNM:
                                //Person Name
                                string personName = "";
                                for (i = 0; i < len; ++i)
                                    personName += (char)bf[point + num1 + i];
                                mferFile.PersonName = personName;
                                break;
                            case MFERdef.MWF_PID:
                                //Person Id
                                string personID = "";
                                for (i = 0; i < len; ++i)
                                    personID += (char)bf[point + num1 + i];
                                mferFile.PersonID = personID;
                                break;

                            case MFERdef.MWF_AGE:
                                //Age
                                StringBuilder age = new StringBuilder();
                                if (len >= 1)
                                    age.Append(bf[point + num1]).Append("(Years)");
                                if (len >= 2)
                                {
                                    this.GetData16(out dt, 2, point + num1 + 1, bf);
                                    age.Append(" ").Append(dt).Append("(Month)");
                                }
                                if (len >= 4)
                                {
                                    this.GetData16(out dt, 2, point + num1 + 3, bf);
                                    age.Append(dt).Append("/");
                                }
                                if (len >= 6)
                                    age.Append(bf[point + num1 + 5]).Append("/");
                                if (len >= 7)
                                    age.Append(bf[point + num1 + 6]);

                                mferFile.Age = age.ToString();
                                break;
                            case MFERdef.MWF_SEX:
                                // Sex
                                string sex = "";
                                switch (bf[point + num1])
                                {
                                    case 0:
                                        sex = "Undefined";
                                        break;
                                    case 1:
                                        sex = "Male";
                                        break;
                                    case 2:
                                        sex = "Female";
                                        break;
                                    case 3:
                                        sex = "Unclassified";
                                        break;
                                }
                                mferFile.Sex = sex;
                                break;
                            // MWF_TIM
                            case MFERdef.MWF_TIM:
                                StringBuilder text69 = new StringBuilder();
                                if (len >= 1)
                                {
                                    this.GetData16(out int dt4, 2, point + num1, bf);
                                    text69.Append(dt4).Append("/");
                                }
                                if (len >= 3)
                                    text69.Append(bf[point + num1 + 2]).Append("/");
                                if (len >= 4)
                                    text69.Append(bf[point + num1 + 3]).Append(" ");
                                if (len >= 5)
                                    text69.Append(bf[point + num1 + 4]).Append(":");
                                if (len >= 6)
                                    text69.Append(bf[point + num1 + 5]).Append(":");
                                if (len >= 7)
                                    text69.Append(bf[point + num1 + 6]).Append(" ");
                                if (len >= 8)
                                    text69.Append(bf[point + num1 + 7]).Append(":");
                                if (len >= 9)
                                    text69.Append(bf[point + num1 + 8]).Append(":");
                                if (len >= 10)
                                    text69.Append(bf[point + num1 + 9]);

                                mferFile.MeasurementTime = text69.ToString();
                                break;
                            case MFERdef.MWF_MSS:
                                // message
                                StringBuilder message = new StringBuilder(len);
                                for (i = 0; i < len; ++i)
                                    message.Append((char)bf[point + num3 + i]);
                                mferFile.Message = message.ToString();
                                break;
                        }
                    }
                    point += num1 + len;
                }
                catch
                {
                    return 0;
                }
            }
            return 0;
        }


        private void ToHex(byte dt, out string sr)
        {
            string[] strArray = new string[16]
            {
                "0",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "A",
                "B",
                "C",
                "D",
                "E",
                "F"
            };
            int index1 = ((int)dt & 240) >> 4;
            string str1 = strArray[index1];
            int index2 = (int)dt & 15;
            string str2 = strArray[index2];
            sr = str1 + str2;
        }

        private int GetData(out int dt, int pt, byte[] bf)
        {
            int num1 = (int)bf[pt + 1] & 128;
            int num2 = 2;
            dt = 0;
            int num3;
            if (num1 > 0)
            {
                int num4 = (int)bf[pt + 1] & (int)sbyte.MaxValue;
                if (num4 > 4)
                    return -1;
                int num5 = num4;
                num3 = 0;
                for (int index = 0; index < num5; ++index)
                {
                    num3 = num3 * 256 + (int)bf[pt + 2 + index];
                    ++num2;
                }
            }
            else
                num3 = (int)bf[pt + 1];
            if (this.endian)
            {
                dt = 0;
                for (int index1 = 0; index1 < num3; ++index1)
                {
                    int num6 = (int)bf[pt + num2 + index1];
                    for (int index2 = 0; index2 < index1; ++index2)
                        num6 *= 256;
                    dt += num6;
                }
            }
            else
            {
                dt = 0;
                for (int index = 0; index < num3; ++index)
                    dt = dt * 256 + (int)bf[pt + num2 + index];
            }
            return 0;
        }

        private int GetData16(out int dt, int len, int pt, byte[] bf)
        {
            int num1 = 2;
            if (num1 > len)
                num1 = len;
            if (this.endian)
            {
                dt = 0;
                for (int index1 = 0; index1 < num1; ++index1)
                {
                    int num2 = (int)bf[pt + index1];
                    for (int index2 = 0; index2 < index1; ++index2)
                        num2 *= 256;
                    dt += num2;
                }
            }
            else
            {
                dt = 0;
                for (int index = 0; index < num1; ++index)
                    dt = dt * 256 + (int)bf[pt + index];
            }
            return len > 2 ? -1 : 0;
        }

        private int GetData32(out int dt, int len, int pt, byte[] bf)
        {
            int num1 = 4;
            if (num1 > len)
                num1 = len;
            if (this.endian)
            {
                dt = 0;
                for (int index1 = 0; index1 < num1; ++index1)
                {
                    int num2 = (int)bf[pt + index1];
                    for (int index2 = 0; index2 < index1; ++index2)
                        num2 *= 256;
                    dt += num2;
                }
            }
            else
            {
                dt = 0;
                for (int index = 0; index < num1; ++index)
                    dt = dt * 256 + (int)bf[pt + index];
            }
            return len > 4 ? -1 : 0;
        }
    }
}
