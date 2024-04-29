namespace MFERParser
{
    public class MferFile
    {
        public string MeasurementTime { get; set; }
        public int Channel;
        public int wave_lead;
        public int Block;
        public int Sequence;
        public double Sampling;
        public double Resolution;
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
                                return -1;
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
                                if (len < 2)
                                {
                                    this.GetData(out dt, point, bf);
                                    switch (dt)
                                    {
                                        case 0:
                                            this.endian = false;
                                            break;
                                        case 1:
                                            this.endian = true;
                                            break;
                                    }
                                }
                                else
                                    break;
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
                            // MWF_WFM
                            case 8:
                                //this.GetData16(out dt, len, point + num1, bf);
                                //int index1 = 0;
                                //while (index1 < MFERdef.WaveformCode.Length && MFERdef.WaveformCode[index1] != dt)
                                //    ++index1;
                                break;
                            // MWF_LDN
                            case 9:
                                this.GetData16(out dt, len, point + num1, bf);
                                //int index2 = 0;
                                //while (index2 < MFERdef.ECGleadCode.Length && MFERdef.ECGleadCode[index2] != dt)
                                //    ++index2;
                                mferFile.wave_lead = dt;
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
                                    if (bf[point + num1] == (byte)0)
                                        mferFile.Sampling = 1.0 / num7;
                                    else if (bf[point + num1] == (byte)1)
                                    {
                                        mferFile.Sampling = 1.0 / num7;
                                        num8 = num7 * 1000.0;
                                    }
                                    else
                                        num8 = bf[point + num1] != (byte)2 ? 0.0 : num7 * 1000.0;
                                    break;
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
                                            break;
                                        default:
                                            num9 = 0.0;
                                            goto case 1;
                                    }
                                }
                                else
                                    break;
                                break;
                            // MWF_CMP
                            case 14:
                                this.GetData16(out dt, len, point + num1, bf);
                                if (dt != 0 && dt == 1)
                                {
                                    mferFile.DType = 9;
                                    break;
                                }
                                break;
                            // MWF_FLT
                            case 17:
                                int num10 = len;
                                break;
                            // MWF_NUL
                            case 18:
                                this.GetData32(out dt, len, point + num1, bf);
                                mferFile.Nil = dt;
                                break;
                            case 19:
                            case 65:
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
                                this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num1, point + num1 + len, bf);
                                break;
                            // MWF_SET
                            case 103:
                                this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num1, point + num1 + len, bf);
                                break;
                            // MWF_END
                            case 128:
                                return 0;
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
