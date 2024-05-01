namespace MFERParser
{
    public class MferFile
    {
        public string MeasurementTime { get; set; }
        /// <summary>
        /// Data format version
        /// </summary>
        public string Version { get; internal set; }
        public int DataPointer { get; internal set; }
        public int WaveformIdentifier { get; internal set; }
        public int WaveformName { get; internal set; }
        public string ECGLeadName { get; internal set; }
        public string Filter { get; internal set; }
        public string Comment { get; internal set; }
        public string Manufacturer { get; internal set; }
        public string PatientName { get; internal set; }
        public string Preamble { get; internal set; }
        public string PersonName { get; internal set; }
        public string PersonID { get; internal set; }
        public string Age { get; internal set; }
        public string Sex { get; internal set; }
        public string Message { get; internal set; }

        /// <summary>
        /// Channel number
        /// </summary>
        public int Channel;

        public int WaveLead;

        /// <summary>
        /// Data block length
        /// </summary>
        public int Block;

        /// <summary>
        /// Sequence number
        /// </summary>
        public int Sequence;

        /// <summary>
        /// The sampling
        /// </summary>
        public double Sampling;
        /// <summary>
        /// Sampling frequency in Hz
        /// </summary>
        public double SamplingFrequency;
        /// <summary>
        /// Sampling interval in ms
        /// </summary>
        public double SamplingInterval;

        /// <summary>
        /// The resolution of the data
        /// </summary>
        public double Resolution;

        /// <summary>
        /// The resolution type
        /// </summary>
        public string ResolutionType;
        public int DType;
        public int Offset;
        public int Nil;
        public int pID;
        public int pData;
        public Dictionary<(int, int), short[]> Waves;
        public MferFile()
        {
            Waves = new Dictionary<(int, int), short[]>();
        }

        /// <summary>
        /// Get the wave data for a specific channel and frame
        /// </summary>
        /// <param name="chn">The channel</param>
        /// <param name="frm">The frame</param>
        /// <returns></returns>
        public short[] GetWave(int chn, int frm)
        {
            return Waves[(chn, frm)];
        }

        public void SetWave(int chn, int frm, short[] wave)
        {
            Waves[(chn, frm)] = wave;
        }
    }



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
                            case 1:
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
                            case 2:
                                mferFile.Version = bf[point + num1].ToString() + "." + bf[point + num1 + 1].ToString() + "." + bf[point + num1 + 2].ToString();
                                break;
                            // MWF_BLK
                            case 4:
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.Block = dt;
                                break;
                            //MWF_CHN
                            case 5:
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.Channel = dt;
                                break;
                            //MWF_SEQ
                            case 6:
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.Sequence = dt;
                                break;
                            case 7:
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.DataPointer = dt;
                                break;
                            // MWF_WFM
                            case 8:
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
                            case 9:
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
                            case 10:
                                this.GetData16(out dt, len, point + num1, bf);
                                mferFile.DType = dt;
                                //int index3 = 0;
                                //while (index3 < MFERdef.dTypeCode.Length && MFERdef.dTypeCode[index3] != dt)
                                //    ++index3;
                                break;
                            // MWF_IVL
                            case 11:
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
                            case 12:
                                if (len <= 6 && len >= 3)
                                {
                                    sbyte y = (sbyte)bf[point + num1 + 1];
                                    this.GetData32(out dt, len - 2, point + num1 + 2, bf);
                                    double num9 = (double)dt * Math.Pow(10.0, (double)y);
                                    switch (bf[point + num1])
                                    {
                                        case 0:
                                            num9 *= 1000000.0;
                                            goto case 1;
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 5:
                                        case 6:
                                        case 7:
                                        case 8:
                                        case 9:
                                        case 10:
                                        case 11:
                                        case 12:
                                            mferFile.Resolution = num9;
                                            mferFile.ResolutionType = MFERdef.ResolutionType[bf[point + num1]];
                                            break;
                                        default:
                                            num9 = 0.0;
#if DEBUG
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine($"Definition definition error");
                                            Console.ResetColor();
#endif
                                            break;
                                    }
                                }
                                else
                                    break;
                                break;
                            case 13:
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.Offset = dt;
                                break;
                            // MWF_CMP
                            case 14:
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
                            case 17:
                                // filter
                                string filter = "";
                                for (i = 0; i < len; ++i)
                                    filter += (char)bf[point + num1 + i];
                                mferFile.Filter = filter;
                                break;
                            // MWF_NUL
                            case 18:
                                // Null value
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.Nil = dt;
                                break;
                            case 19:
                            case 65:
                                break;
                            case 22:
                                // comment
                                string comment = "";
                                for (i = 0; i < len; ++i)
                                    comment += (char)bf[point + num1 + i];
                                mferFile.Comment = comment;
                                break;
                            case 23:
                                // Manufacturer
                                string manufacturer = "";
                                for (i = 0; i < len; ++i)
                                    manufacturer += (char)bf[point + num1 + i];
                                mferFile.Manufacturer = manufacturer;
                                break;
                            case 26:
                                // Patient Name(old)
                                string patientName = "";
                                for (i = 0; i < len; ++i)
                                    patientName += (char)bf[point + num1 + i];
                                mferFile.PatientName = patientName;
                                break;
                            // MWF_WAV
                            case 30:
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
                            case 63:
#if DEBUG
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Channel(" + bf[point + 1].ToString() + ") attribute");
                                Console.ResetColor();
#endif
                                this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num1, point + num1 + len, bf);
                                break;
                            case 64:
                                //Preamble
                                string preamble = "";
                                for (i = 0; i < num1 + len; ++i)
                                    preamble += (char)bf[point + i];
                                mferFile.Preamble = preamble;
                                break;
                            // MWF_SET
                            case 103:
                                // Group
                                this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num1, point + num1 + len, bf);
                                break;
                            // MWF_END
                            case 128:
                                // End of format
                                return 0;
                            case 129:
                                //Person Name
                                string personName = "";
                                for (i = 0; i < len; ++i)
                                    personName += (char)bf[point + num1 + i];
                                mferFile.PersonName = personName;
                                break;
                            case 130:
                                //Person Id
                                string personID = "";
                                for (i = 0; i < len; ++i)
                                    personID += (char)bf[point + num1 + i];
                                mferFile.PersonID = personID;
                                break;

                            case 131:
                                //Age
                                string age = "";
                                if (len >= 1)
                                    age = age + bf[point + num1].ToString() + "(Years)";
                                if (len >= 2)
                                {
                                    this.GetData16(out dt, 2, point + num1 + 1, bf);
                                    age = age + " " + dt.ToString() + "(Month)";
                                }
                                if (len >= 4)
                                {
                                    this.GetData16(out dt, 2, point + num1 + 3, bf);
                                    age = age + dt.ToString() + "/";
                                }
                                if (len >= 6)
                                    age = age + bf[point + num1 + 5].ToString() + "/";
                                if (len >= 7)
                                    age += bf[point + num1 + 6].ToString();

                                mferFile.Age = age;
                                break;
                            case 132:
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
                            case 133:
                                string text69 = "";
                                if (len >= 1)
                                {
                                    this.GetData16(out int dt4, 2, point + num1, bf);
                                    text69 = text69 + dt4.ToString() + "/";
                                }
                                if (len >= 3)
                                    text69 = text69 + bf[point + num1 + 2].ToString() + "/";
                                if (len >= 4)
                                    text69 = text69 + bf[point + num1 + 3].ToString() + " ";
                                if (len >= 5)
                                    text69 = text69 + bf[point + num1 + 4].ToString() + ":";
                                if (len >= 6)
                                    text69 = text69 + bf[point + num1 + 5].ToString() + ":";
                                if (len >= 7)
                                    text69 = text69 + bf[point + num1 + 6].ToString() + " ";
                                if (len >= 8)
                                    text69 = text69 + bf[point + num1 + 7].ToString() + ":";
                                if (len >= 9)
                                    text69 = text69 + bf[point + num1 + 8].ToString() + ":";
                                if (len >= 10)
                                    text69 += bf[point + num1 + 9].ToString();
                                mferFile.MeasurementTime = text69;
                                break;
                            case 134:
                                // message
                                string message = "";
                                for (i = 0; i < len; ++i)
                                    message += (char)bf[point + num3 + i];
                                mferFile.Message = message;
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

        public int GetData16(out int dt, int len, int pt, byte[] bf)
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

        public int GetData32(out int dt, int len, int pt, byte[] bf)
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
