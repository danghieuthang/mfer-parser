//using MFERParser;

//private int TLVanz(int mode, int ch, int point, int dtLen, byte[] bf)
//{
//    Waveview waveview = (Waveview)null;
//    while (dtLen > point)
//    {
//        try
//        {
//            if (waveview == null)
//            {
//                waveview = this.GetDefInf(ch, this.frm);
//                if (this.frm > 0)
//                {
//                    for (int chn = 0; chn <= waveview.channel; ++chn)
//                    {
//                        Waveview defInf1 = this.GetDefInf(chn, this.frm - 1);
//                        Waveview defInf2 = this.GetDefInf(chn, this.frm);
//                        defInf2.chn = chn;
//                        defInf2.frm = this.frm;
//                        defInf2.block = defInf1.block;
//                        defInf2.channel = defInf1.channel;
//                        defInf2.resolution = defInf1.resolution;
//                        defInf2.sampling = defInf1.sampling;
//                        defInf2.sequence = defInf1.sequence;
//                        defInf2.wave = (short[])null;
//                    }
//                }
//            }
//            bool flag = true;
//            if (bf[point] == (byte)128)
//                return 0;
//            int num1;
//            int len;
//            if (bf[point] == (byte)0)
//            {
//                num1 = 0;
//                string str = "00 ";
//                for (len = 1; bf[point + len] == (byte)0 && dtLen > point + len; ++len)
//                {
//                    if (str.Length <= 256)
//                        str += "00 ";
//                }
//            }
//            else
//            {
//                int num2 = 1;
//                if (((int)bf[point] & 32) > 0)
//                {
//                    if (bf[point] == (byte)63)
//                        ++num2;
//                    while (((int)bf[point + num2] & 128) > 0)
//                        ++num2;
//                }
//                len = (int)bf[point + num2];
//                int num3 = 1;
//                if ((len & 128) != 0)
//                {
//                    int num4 = len & (int)sbyte.MaxValue;
//                    if (num4 > 4)
//                        return -1;
//                    len = 0;
//                    for (int index = 1; index <= num4; ++index)
//                    {
//                        len = len * 256 + (int)bf[point + num2 + index];
//                        ++num3;
//                    }
//                }
//                string str = "";
//                int num5 = 0;
//                while (num5 < mode)
//                    ++num5;
//                int num6 = 0;
//                while (num6 <= num2 + len && str.Length < 256)
//                    ++num6;
//                num1 = num2 + num3;
//                int dt;
//                switch (bf[point])
//                {
//                    case 133:
//                        Console.WriteLine("133");
//                        break;
//                    case 1:
//                        if (len < 2)
//                        {
//                            this.GetData(out dt, point, bf);
//                            switch (dt)
//                            {
//                                case 0:
//                                    this.endian = false;
//                                    break;
//                                case 1:
//                                    this.endian = true;
//                                    break;
//                            }
//                        }
//                        else
//                            break;
//                        break;
//                    case 4:
//                        this.GetData32(out dt, len, point + num1, bf);
//                        waveview.block = dt;
//                        if (ch == 0)
//                        {
//                            for (int chn = 1; chn <= waveview.channel; ++chn)
//                                this.GetDefInf(chn, this.frm).block = waveview.block;
//                            break;
//                        }
//                        break;
//                    case 5:
//                        this.GetData32(out dt, len, point + num1, bf);
//                        waveview.channel = dt;
//                        this.SetChannel(dt);
//                        break;
//                    case 6:
//                        this.GetData32(out dt, len, point + num1, bf);
//                        waveview.sequence = dt;
//                        if (ch == 0)
//                        {
//                            for (int chn = 1; chn <= waveview.channel; ++chn)
//                                this.GetDefInf(chn, this.frm).sequence = waveview.sequence;
//                            break;
//                        }
//                        break;
//                    case 8:
//                        this.GetData16(out dt, len, point + num1, bf);
//                        int index1 = 0;
//                        while (index1 < MFERdef.WaveformCode.Length && MFERdef.WaveformCode[index1] != dt)
//                            ++index1;
//                        break;
//                    case 9:
//                        this.GetData16(out dt, len, point + num1, bf);
//                        int index2 = 0;
//                        while (index2 < MFERdef.ECGleadCode.Length && MFERdef.ECGleadCode[index2] != dt)
//                            ++index2;
//                        waveview.wave_lead = dt;
//                        break;
//                    case 10:
//                        this.GetData16(out dt, len, point + num1, bf);
//                        waveview.dType = dt;
//                        if (ch == 0)
//                        {
//                            for (int chn = 1; chn <= waveview.channel; ++chn)
//                                this.GetDefInf(chn, this.frm).sequence = dt;
//                        }
//                        int index3 = 0;
//                        while (index3 < MFERdef.dTypeCode.Length && MFERdef.dTypeCode[index3] != dt)
//                            ++index3;
//                        break;
//                    case 11:
//                        if (len <= 6 && len >= 3)
//                        {
//                            sbyte y = (sbyte)bf[point + num1 + 1];
//                            this.GetData32(out dt, len - 2, point + num1 + 2, bf);
//                            double num7 = (double)dt * Math.Pow(10.0, (double)y);
//                            double num8;
//                            if (bf[point + num1] == (byte)0)
//                                waveview.sampling = 1.0 / num7;
//                            else if (bf[point + num1] == (byte)1)
//                            {
//                                waveview.sampling = num7;
//                                num8 = num7 * 1000.0;
//                            }
//                            else
//                                num8 = bf[point + num1] != (byte)2 ? 0.0 : num7 * 1000.0;
//                            if (ch == 0)
//                            {
//                                for (int chn = 1; chn <= waveview.channel; ++chn)
//                                    this.GetDefInf(chn, this.frm).sampling = waveview.sampling;
//                                break;
//                            }
//                            break;
//                        }
//                        break;
//                    case 12:
//                        if (len <= 6 && len >= 3)
//                        {
//                            sbyte y = (sbyte)bf[point + num1 + 1];
//                            this.GetData32(out dt, len - 2, point + num1 + 2, bf);
//                            double num9 = (double)dt * Math.Pow(10.0, (double)y);
//                            switch (bf[point + num1])
//                            {
//                                case 0:
//                                    num9 *= 1000000.0;
//                                    goto case 1;
//                                case 1:
//                                case 2:
//                                case 3:
//                                case 4:
//                                case 5:
//                                case 6:
//                                case 7:
//                                case 8:
//                                case 9:
//                                case 10:
//                                case 11:
//                                case 12:
//                                    waveview.resolution = num9;
//                                    if (ch == 0)
//                                    {
//                                        for (int chn = 1; chn <= waveview.channel; ++chn)
//                                            this.GetDefInf(chn, this.frm).resolution = waveview.resolution;
//                                        break;
//                                    }
//                                    break;
//                                default:
//                                    num9 = 0.0;
//                                    goto case 1;
//                            }
//                        }
//                        else
//                            break;
//                        break;
//                    case 14:
//                        this.GetData16(out dt, len, point + num1, bf);
//                        if (dt != 0 && dt == 1)
//                        {
//                            waveview.dType = 9;
//                            if (ch == 0)
//                            {
//                                for (int chn = 1; chn <= waveview.channel; ++chn)
//                                    this.GetDefInf(chn, this.frm).dType = 9;
//                                break;
//                            }
//                            break;
//                        }
//                        break;
//                    case 17:
//                        int num10 = 0;
//                        while (num10 < len)
//                            ++num10;
//                        break;
//                    case 18:
//                        this.GetData32(out dt, len, point + num1, bf);
//                        waveview.nil = dt;
//                        break;
//                    case 19:
//                    case 65:
//                        ++this.eventCounter;
//                        break;
//                    case 30:
//                        int num11 = 1;
//                        if (this.GetDefInf(0, this.frm).dType != 9)
//                            num11 = 2;
//                        int num12 = this.GetDefInf(0, this.frm).sequence * this.GetDefInf(0, this.frm).block / this.GetDefInf(0, this.frm).channel;
//                        if (num12 < len / this.GetDefInf(0, this.frm).channel / num11)
//                            num12 = len / this.GetDefInf(0, this.frm).channel / num11;
//                        if (this.GetDefInf(0, this.frm).block == 1 && this.GetDefInf(0, this.frm).sequence == 1)
//                            this.GetDefInf(0, this.frm).sequence = num12;
//                        for (int chn = 1; chn <= this.GetDefInf(0, this.frm).channel; ++chn)
//                        {
//                            Waveview defInf = this.GetDefInf(chn, this.frm);
//                            if (defInf.block == 1 && defInf.sequence == 1)
//                                defInf.sequence = num12;
//                            defInf.wave = new short[defInf.block * defInf.sequence];
//                            defInf.pData = 0;
//                        }
//                        int num13 = 0;
//                        int sequence = this.GetDefInf(0, this.frm).sequence;
//                        for (int index4 = 0; index4 < sequence; ++index4)
//                        {
//                            int channel = this.GetDefInf(0, this.frm).channel;
//                            for (int chn = 1; chn <= channel; ++chn)
//                            {
//                                Waveview defInf = this.GetDefInf(chn, this.frm);
//                                for (int index5 = 0; index5 < defInf.block; ++index5)
//                                {
//                                    if (len > num13 + num1 && point + num1 + num13 < dtLen)
//                                    {
//                                        if (defInf.dType == 9)
//                                        {
//                                            dt = (int)bf[point + num1 + num13];
//                                            if (dt >= 128)
//                                                dt -= 256;
//                                            defInf.pData += dt;
//                                            ++num13;
//                                            defInf.wave[index4 * defInf.block + index5] = (short)defInf.pData;
//                                        }
//                                        else
//                                        {
//                                            this.GetData16(out dt, 2, point + num1 + num13, bf);
//                                            num13 += 2;
//                                            defInf.wave[index4 * defInf.block + index5] = (short)dt;
//                                        }
//                                    }
//                                    else
//                                        goto label_112;
//                                }
//                            }
//                        }
//                    label_112:
//                        if (ch == 0)
//                            ++this.frm;
//                        int num14 = len / num11;
//                        waveview = (Waveview)null;
//                        break;
//                    case 63:
//                        this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num1, point + num1 + len, bf);
//                        flag = false;
//                        break;

//                    case 103:
//                        this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num1, point + num1 + len, bf);
//                        flag = false;
//                        break;
//                    case 128:
//                        return 0;

//                }
//            }
//            int num15 = flag ? 1 : 0;
//            point += num1 + len;
//        }
//        catch
//        {
//            return 0;
//        }
//    }
//    return 0;
//}
