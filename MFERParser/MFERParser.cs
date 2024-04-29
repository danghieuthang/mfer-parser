namespace MFERParser
{
    public class MferFile
    {
        public short[] Wave { get; set; }
        public string MeasureTime { get; set; }
    }

    public class MferParser
    {
        private bool endian;
        private Waveview wQueue;
        private int pID;
        private int frm;
        private List<string> MFERFormat;
        private byte[] bf;
        private Waveview cForm;

        public MferParser()
        {
            this.wQueue = null;
            this.pID = 0;
            this.MFERFormat = new List<string>();
        }
        public MferFile Parse(string filePath)
        {
            var mferFile = new MferFile();

            // Open the file and read its contents
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                long length = fileStream.Length;
                this.bf = new byte[length];
                int num = fileStream.Read(this.bf, 0, (int)length);
                fileStream.Close();
                if (num <= 0)
                    return null;
                this.endian = false;
                int point = 0;
                this.frm = 0;
                this.wQueue = null;
                this.cForm = null;
                this.TLVanz(0, 0, point, (int)length, this.bf);
                if (this.frm > 1)
                {
                    Console.WriteLine("FRM:1");
                    for (int index = 1; index <= this.frm; ++index)
                        Console.WriteLine("FRM:" + index.ToString());
                }
                this.cForm = this.GetDefInf(0, 0);
                if (this.cForm.channel >= 1)
                {
                    Console.WriteLine("CH:1");
                    for (int index = 1; index <= this.cForm.channel; ++index)
                        Console.WriteLine("CN:" + index.ToString());
                }
                var dispCHN = 2;
                var dispFRM = 0;
                this.WaveInit(dispCHN, dispFRM);
                mferFile.Wave = this.cForm.wave;

            }

            return mferFile;
        }

        private Waveview GetDefInf(int chn, int frm)
        {
            Waveview waveview = null;
            for (Waveview defInf = this.wQueue; defInf != null; defInf = defInf.wLink)
            {
                if (defInf.chn == chn && defInf.frm == frm)
                    return defInf;
                waveview = defInf;
            }
            Waveview defInf1 = new Waveview(chn)
            {
                pID = ++this.pID
            };
            if (this.wQueue == null)
                this.wQueue = defInf1;
            if (waveview != null)
                waveview.wLink = defInf1;
            defInf1.frm = frm;
            return defInf1;
        }


        private int TLVanz(int mode, int ch, int point, int dtLen, byte[] bf)
        {
            Waveview waveview = null;
            while (dtLen > point)
            {
                try
                {
                    if (waveview == null)
                    {
                        waveview = this.GetDefInf(ch, this.frm);
                        if (this.frm > 0)
                        {
                            for (int chn = 0; chn <= waveview.channel; ++chn)
                            {
                                Waveview defInf1 = this.GetDefInf(chn, this.frm - 1);
                                Waveview defInf2 = this.GetDefInf(chn, this.frm);
                                defInf2.chn = chn;
                                defInf2.frm = this.frm;
                                defInf2.block = defInf1.block;
                                defInf2.channel = defInf1.channel;
                                defInf2.resolution = defInf1.resolution;
                                defInf2.sampling = defInf1.sampling;
                                defInf2.sequence = defInf1.sequence;
                                defInf2.wave = null;
                            }
                        }
                    }
                    bool flag = true;
                    if (bf[point] == (byte)128)
                        return 0;
                    int num1;
                    int len;
                    if (bf[point] == (byte)0)
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
                        if (((int)bf[point] & 32) > 0)
                        {
                            if (bf[point] == (byte)63)
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
                            case 4:
                                this.GetData32(out dt, len, point + num1, bf);
                                waveview.block = dt;
                                if (ch == 0)
                                {
                                    for (int chn = 1; chn <= waveview.channel; ++chn)
                                        this.GetDefInf(chn, this.frm).block = waveview.block;
                                    break;
                                }
                                break;
                            case 5:
                                this.GetData32(out dt, len, point + num1, bf);
                                waveview.channel = dt;
                                this.SetChannel(dt);
                                break;
                            case 6:
                                this.GetData32(out dt, len, point + num1, bf);
                                waveview.sequence = dt;
                                if (ch == 0)
                                {
                                    for (int chn = 1; chn <= waveview.channel; ++chn)
                                        this.GetDefInf(chn, this.frm).sequence = waveview.sequence;
                                    break;
                                }
                                break;
                            case 8:
                                this.GetData16(out dt, len, point + num1, bf);
                                int index1 = 0;
                                while (index1 < MFERdef.WaveformCode.Length && MFERdef.WaveformCode[index1] != dt)
                                    ++index1;
                                break;
                            case 9:
                                this.GetData16(out dt, len, point + num1, bf);
                                int index2 = 0;
                                while (index2 < MFERdef.ECGleadCode.Length && MFERdef.ECGleadCode[index2] != dt)
                                    ++index2;
                                waveview.wave_lead = dt;
                                break;
                            case 10:
                                this.GetData16(out dt, len, point + num1, bf);
                                waveview.dType = dt;
                                if (ch == 0)
                                {
                                    for (int chn = 1; chn <= waveview.channel; ++chn)
                                        this.GetDefInf(chn, this.frm).sequence = dt;
                                }
                                int index3 = 0;
                                while (index3 < MFERdef.dTypeCode.Length && MFERdef.dTypeCode[index3] != dt)
                                    ++index3;
                                break;
                            case 11:
                                if (len <= 6 && len >= 3)
                                {
                                    sbyte y = (sbyte)bf[point + num1 + 1];
                                    this.GetData32(out dt, len - 2, point + num1 + 2, bf);
                                    double num7 = (double)dt * Math.Pow(10.0, (double)y);
                                    double num8;
                                    if (bf[point + num1] == (byte)0)
                                        waveview.sampling = 1.0 / num7;
                                    else if (bf[point + num1] == (byte)1)
                                    {
                                        waveview.sampling = num7;
                                        num8 = num7 * 1000.0;
                                    }
                                    else
                                        num8 = bf[point + num1] != (byte)2 ? 0.0 : num7 * 1000.0;
                                    if (ch == 0)
                                    {
                                        for (int chn = 1; chn <= waveview.channel; ++chn)
                                            this.GetDefInf(chn, this.frm).sampling = waveview.sampling;
                                        break;
                                    }
                                    break;
                                }
                                break;
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
                                            waveview.resolution = num9;
                                            if (ch == 0)
                                            {
                                                for (int chn = 1; chn <= waveview.channel; ++chn)
                                                    this.GetDefInf(chn, this.frm).resolution = waveview.resolution;
                                                break;
                                            }
                                            break;
                                        default:
                                            num9 = 0.0;
                                            goto case 1;
                                    }
                                }
                                else
                                    break;
                                break;
                            case 14:
                                this.GetData16(out dt, len, point + num1, bf);
                                if (dt != 0 && dt == 1)
                                {
                                    waveview.dType = 9;
                                    if (ch == 0)
                                    {
                                        for (int chn = 1; chn <= waveview.channel; ++chn)
                                            this.GetDefInf(chn, this.frm).dType = 9;
                                        break;
                                    }
                                    break;
                                }
                                break;
                            case 17:
                                int num10 = len;
                                break;
                            case 18:
                                this.GetData32(out dt, len, point + num1, bf);
                                waveview.nil = dt;
                                break;
                            case 19:
                            case 65:
                                break;
                            case 30:
                                int num11 = 1;
                                var defInf0 = this.GetDefInf(0, this.frm);
                                if (defInf0.dType != 9)
                                    num11 = 2;
                                int num12 = defInf0.sequence * defInf0.block / defInf0.channel;
                                if (num12 < len / defInf0.channel / num11)
                                    num12 = len / defInf0.channel / num11;
                                if (defInf0.block == 1 && defInf0.sequence == 1)
                                    defInf0.sequence = num12;
                                for (int chn = 1; chn <= defInf0.channel; ++chn)
                                {
                                    Waveview defInfChn = this.GetDefInf(chn, this.frm);
                                    if (defInfChn.block == 1 && defInfChn.sequence == 1)
                                        defInfChn.sequence = num12;
                                    defInfChn.wave = new short[defInfChn.block * defInfChn.sequence];
                                    defInfChn.pData = 0;
                                }
                                int num13 = 0;
                                int sequence = defInf0.sequence;
                                for (int index4 = 0; index4 < sequence; ++index4)
                                {
                                    int channel = defInf0.channel;
                                    for (int chn = 1; chn <= channel; ++chn)
                                    {
                                        Waveview defInfChn = this.GetDefInf(chn, this.frm);
                                        for (int index5 = 0; index5 < defInfChn.block; ++index5)
                                        {
                                            if (len > num13 + num1 && point + num1 + num13 < dtLen)
                                            {
                                                if (defInfChn.dType == 9)
                                                {
                                                    dt = (int)bf[point + num1 + num13];
                                                    if (dt >= 128)
                                                        dt -= 256;
                                                    defInfChn.pData += dt;
                                                    ++num13;
                                                    defInfChn.wave[index4 * defInfChn.block + index5] = (short)defInfChn.pData;
                                                }
                                                else
                                                {
                                                    this.GetData16(out dt, 2, point + num1 + num13, bf);
                                                    num13 += 2;
                                                    defInfChn.wave[index4 * defInfChn.block + index5] = (short)dt;
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
                                waveview = null;
                                break;
                            case 63:
                                this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num1, point + num1 + len, bf);
                                flag = false;
                                break;

                            case 103:
                                this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num1, point + num1 + len, bf);
                                flag = false;
                                break;
                            case 128:
                                return 0;
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
                                Console.WriteLine(text69);
                                break;
                        }
                    }
                    int num15 = flag ? 1 : 0;
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

        public void SetChannel(int chn)
        {
            Waveview defInf1 = this.GetDefInf(0, this.frm);
            for (int chn1 = 1; chn1 <= chn; ++chn1)
            {
                Waveview defInf2 = this.GetDefInf(chn1, this.frm);
                defInf2.block = defInf1.block;
                defInf2.channel = defInf1.channel;
                defInf2.resolution = defInf1.resolution;
                defInf2.sampling = defInf1.sampling;
                defInf2.sequence = defInf1.sequence;
            }
        }

        public void WaveInit(int chn, int frm)
        {
            this.cForm = this.GetDefInf(chn, frm);
            this.cForm.ViewInit(0);
        }
    }
}
