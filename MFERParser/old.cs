//using MFERParser;

//private int TLVanz(int mode, int ch, int point, int dtLen, byte[] bf)
//{
//    int num1 = 0;
//    Waveview waveview = (Waveview)null;
//    while (dtLen > point)
//    {
//        try
//        {
//            if (waveview == null)
//                waveview = this.GetDefInf(ch, this.frm);
//            ListViewItem listViewItem = new ListViewItem();
//            bool flag = true;
//            listViewItem.Text = mode != 0 ? mode.ToString() : " ";
//            ++num1;
//            listViewItem.SubItems.Add(num1.ToString());
//            string sr1 = "";
//            int num2 = 0;
//            byte dt1 = (byte)(point >> 24 & (int)byte.MaxValue);
//            if (dt1 != (byte)0)
//            {
//                this.ToHex(dt1, out sr1);
//                num2 = 1;
//            }
//            byte dt2 = (byte)(point >> 16 & (int)byte.MaxValue);
//            string sr2;
//            if (num2 != 0 || dt2 != (byte)0)
//            {
//                this.ToHex(dt2, out sr2);
//                sr1 += sr2;
//                num2 = 1;
//            }
//            byte dt3 = (byte)(point >> 8 & (int)byte.MaxValue);
//            if (num2 != 0 || dt3 != (byte)0)
//            {
//                this.ToHex(dt3, out sr2);
//                sr1 += sr2;
//            }
//            this.ToHex((byte)(point & (int)byte.MaxValue), out sr2);
//            string text1 = sr1 + sr2;
//            listViewItem.SubItems.Add(text1);
//            if (bf[point] == (byte)128)
//            {
//                listViewItem.SubItems.Add("80");
//                listViewItem.SubItems.Add("80 End of format");
//                this.MFERFormat.Items.AddRange(new ListViewItem[1]
//                {
//              listViewItem
//                });
//                return 0;
//            }
//            int num3;
//            int len;
//            if (bf[point] == (byte)0)
//            {
//                num3 = 0;
//                string text2 = "00 ";
//                for (len = 1; bf[point + len] == (byte)0 && dtLen > point + len; ++len)
//                {
//                    if (text2.Length <= 256)
//                        text2 += "00 ";
//                }
//                listViewItem.SubItems.Add(text2);
//                listViewItem.SubItems.Add("00 Zero");
//                listViewItem.SubItems.Add(len.ToString());
//                string text3 = "Zero(" + len.ToString() + ")";
//                listViewItem.SubItems.Add(text3);
//            }
//            else
//            {
//                int num4 = 1;
//                if (((int)bf[point] & 32) > 0)
//                {
//                    if (bf[point] == (byte)63)
//                        ++num4;
//                    while (((int)bf[point + num4] & 128) > 0)
//                        ++num4;
//                }
//                len = (int)bf[point + num4];
//                int num5 = 1;
//                if ((len & 128) != 0)
//                {
//                    int num6 = len & (int)sbyte.MaxValue;
//                    if (num6 > 4)
//                    {
//                        listViewItem.SubItems.Add("Data length[" + num6.ToString() + "] error");
//                        this.MFERFormat.Items.AddRange(new ListViewItem[1]
//                        {
//                  listViewItem
//                        });
//                        return -1;
//                    }
//                    len = 0;
//                    for (int index = 1; index <= num6; ++index)
//                    {
//                        len = len * 256 + (int)bf[point + num4 + index];
//                        ++num5;
//                    }
//                }
//                string sr3 = "";
//                for (int index = 0; index < mode; ++index)
//                    sr3 += "   ";
//                for (int index = 0; index <= num4 + len; ++index)
//                {
//                    this.ToHex(bf[point + index], out sr2);
//                    sr3 = sr3 + sr2 + " ";
//                    if (sr3.Length >= 256)
//                        break;
//                }
//                num3 = num4 + num5;
//                listViewItem.SubItems.Add(sr3);
//                this.ToHex(bf[point], out sr3);
//                string text4 = sr3 + " ";
//                int dt4;
//                int num7;
//                switch (bf[point])
//                {
//                    case 0:
//                        goto label_246;
//                    case 1:
//                        string text5 = text4 + "Big/Little Endian";
//                        listViewItem.SubItems.Add(text5);
//                        string text6 = len.ToString();
//                        listViewItem.SubItems.Add(text6);
//                        if (len >= 2)
//                        {
//                            listViewItem.SubItems.Add("Data Length error");
//                            goto label_246;
//                        }
//                        else
//                        {
//                            num7 = this.GetData(out dt4, point, bf);
//                            switch (dt4)
//                            {
//                                case 0:
//                                    listViewItem.SubItems.Add("Big endian");
//                                    this.endian = false;
//                                    goto label_246;
//                                case 1:
//                                    listViewItem.SubItems.Add("Little endian");
//                                    this.endian = true;
//                                    goto label_246;
//                                default:
//                                    listViewItem.SubItems.Add("Data error");
//                                    goto label_246;
//                            }
//                        }
//                    case 2:
//                        string text7 = text4 + "Version";
//                        listViewItem.SubItems.Add(text7);
//                        string text8 = len.ToString();
//                        listViewItem.SubItems.Add(text8);
//                        string text9 = "Version " + bf[point + num3].ToString() + "." + bf[point + num3 + 1].ToString() + "." + bf[point + num3 + 2].ToString();
//                        listViewItem.SubItems.Add(text9);
//                        goto label_246;
//                    case 4:
//                        string text10 = text4 + "Data block length";
//                        listViewItem.SubItems.Add(text10);
//                        string text11 = len.ToString();
//                        listViewItem.SubItems.Add(text11);
//                        num7 = this.GetData32(out dt4, len, point + num3, bf);
//                        listViewItem.SubItems.Add("Block length=" + dt4.ToString());
//                        waveview.block = dt4;
//                        if (ch == 0)
//                        {
//                            for (int chn = 1; chn <= waveview.channel; ++chn)
//                                this.GetDefInf(chn, this.frm).block = waveview.block;
//                            goto label_246;
//                        }
//                        else
//                            goto label_246;
//                    case 5:
//                        string text12 = text4 + "Channel number";
//                        listViewItem.SubItems.Add(text12);
//                        string text13 = len.ToString();
//                        listViewItem.SubItems.Add(text13);
//                        num7 = this.GetData32(out dt4, len, point + num3, bf);
//                        listViewItem.SubItems.Add("Channel=" + dt4.ToString());
//                        waveview.channel = dt4;
//                        this.SetChannel(dt4);
//                        goto label_246;
//                    case 6:
//                        string text14 = text4 + "Sequence number";
//                        listViewItem.SubItems.Add(text14);
//                        string text15 = len.ToString();
//                        listViewItem.SubItems.Add(text15);
//                        num7 = this.GetData32(out dt4, len, point + num3, bf);
//                        listViewItem.SubItems.Add("Sequence=" + dt4.ToString());
//                        waveview.sequence = dt4;
//                        if (ch == 0)
//                        {
//                            for (int chn = 1; chn <= waveview.channel; ++chn)
//                                this.GetDefInf(chn, this.frm).sequence = waveview.sequence;
//                            goto label_246;
//                        }
//                        else
//                            goto label_246;
//                    case 7:
//                        string text16 = text4 + "Data pointer";
//                        listViewItem.SubItems.Add(text16);
//                        string text17 = len.ToString();
//                        listViewItem.SubItems.Add(text17);
//                        num7 = this.GetData32(out dt4, len, point + num3, bf);
//                        listViewItem.SubItems.Add("Point=" + dt4.ToString());
//                        goto label_246;
//                    case 8:
//                        string text18 = text4 + "Waveform identifier";
//                        listViewItem.SubItems.Add(text18);
//                        string text19 = len.ToString();
//                        listViewItem.SubItems.Add(text19);
//                        num7 = this.GetData16(out dt4, len, point + num3, bf);
//                        int index1;
//                        for (index1 = 0; index1 < MFERdef.WaveformCode.Length; ++index1)
//                        {
//                            if (MFERdef.WaveformCode[index1] == dt4)
//                            {
//                                text19 = MFERdef.WaveformName[index1];
//                                break;
//                            }
//                        }
//                        if (index1 >= MFERdef.WaveformCode.Length)
//                            text19 = "Undefined waveform(" + dt4.ToString() + ")";
//                        if (len > 2)
//                        {
//                            string str = text19 + "[";
//                            for (int index2 = 2; index2 < len; ++index2)
//                                str += (string)(object)(char)bf[point + num3 + index2];
//                            text19 = str + "]";
//                        }
//                        listViewItem.SubItems.Add(text19);
//                        goto label_246;
//                    case 9:
//                        string text20 = text4 + "Waveform or lead name";
//                        listViewItem.SubItems.Add(text20);
//                        string text21 = len.ToString();
//                        listViewItem.SubItems.Add(text21);
//                        num7 = this.GetData16(out dt4, len, point + num3, bf);
//                        int index3;
//                        for (index3 = 0; index3 < MFERdef.ECGleadCode.Length; ++index3)
//                        {
//                            if (MFERdef.ECGleadCode[index3] == dt4)
//                            {
//                                text21 = "Lead " + MFERdef.ECGleadName[index3];
//                                break;
//                            }
//                        }
//                        if (index3 >= MFERdef.ECGleadCode.Length)
//                            text21 = "Undefine code(" + dt4.ToString() + ")";
//                        waveview.wave_lead = dt4;
//                        if (len > 2)
//                        {
//                            string str = text21 + "[";
//                            for (int index4 = 2; index4 < len; ++index4)
//                                str += (string)(object)(char)bf[point + num3 + index4];
//                            text21 = str + "]";
//                        }
//                        listViewItem.SubItems.Add(text21);
//                        goto label_246;
//                    case 10:
//                        string text22 = text4 + "Data type";
//                        listViewItem.SubItems.Add(text22);
//                        string text23 = len.ToString();
//                        listViewItem.SubItems.Add(text23);
//                        num7 = this.GetData16(out dt4, len, point + num3, bf);
//                        waveview.dType = dt4;
//                        if (ch == 0)
//                        {
//                            for (int chn = 1; chn <= waveview.channel; ++chn)
//                                this.GetDefInf(chn, this.frm).sequence = dt4;
//                        }
//                        int index5;
//                        for (index5 = 0; index5 < MFERdef.dTypeCode.Length; ++index5)
//                        {
//                            if (MFERdef.dTypeCode[index5] == dt4)
//                            {
//                                listViewItem.SubItems.Add(MFERdef.dTypeName[index5]);
//                                break;
//                            }
//                        }
//                        if (index5 >= MFERdef.dTypeCode.Length)
//                        {
//                            listViewItem.SubItems.Add("Undefine data type[" + dt4.ToString() + "]");
//                            goto label_246;
//                        }
//                        else
//                            goto label_246;
//                    case 11:
//                        string text24 = text4 + "Sampling frequency";
//                        listViewItem.SubItems.Add(text24);
//                        string text25 = len.ToString();
//                        listViewItem.SubItems.Add(text25);
//                        if (len <= 6 && len >= 3)
//                        {
//                            sbyte y = (sbyte)bf[point + num3 + 1];
//                            num7 = this.GetData32(out dt4, len - 2, point + num3 + 2, bf);
//                            double num8 = (double)dt4 * Math.Pow(10.0, (double)y);
//                            if (bf[point + num3] == (byte)0)
//                            {
//                                listViewItem.SubItems.Add("Sampling frquency=" + num8.ToString() + "Hz");
//                                waveview.sampling = 1.0 / num8;
//                            }
//                            else if (bf[point + num3] == (byte)1)
//                            {
//                                waveview.sampling = num8;
//                                num8 *= 1000.0;
//                                listViewItem.SubItems.Add("Sampling interval=" + num8.ToString() + "ms");
//                            }
//                            else if (bf[point + num3] == (byte)2)
//                            {
//                                num8 *= 1000.0;
//                                listViewItem.SubItems.Add("Distance=" + num8.ToString() + "mm");
//                            }
//                            else
//                            {
//                                num8 = 0.0;
//                                listViewItem.SubItems.Add("Definition definition error");
//                            }
//                            if (ch == 0)
//                            {
//                                for (int chn = 1; chn <= waveview.channel; ++chn)
//                                    this.GetDefInf(chn, this.frm).sampling = waveview.sampling;
//                                goto label_246;
//                            }
//                            else
//                                goto label_246;
//                        }
//                        else
//                        {
//                            listViewItem.SubItems.Add("Definition length error");
//                            goto label_246;
//                        }
//                    case 12:
//                        string text26 = text4 + "Sampling resolution";
//                        listViewItem.SubItems.Add(text26);
//                        string text27 = len.ToString();
//                        listViewItem.SubItems.Add(text27);
//                        if (len <= 6 && len >= 3)
//                        {
//                            sbyte y = (sbyte)bf[point + num3 + 1];
//                            num7 = this.GetData32(out dt4, len - 2, point + num3 + 2, bf);
//                            double num9 = (double)dt4 * Math.Pow(10.0, (double)y);
//                            switch (bf[point + num3])
//                            {
//                                case 0:
//                                    num9 *= 1000000.0;
//                                    listViewItem.SubItems.Add("Resolution(volt)=" + num9.ToString() + "μV");
//                                    break;
//                                case 1:
//                                    listViewItem.SubItems.Add("Resolution(Press)=" + num9.ToString() + "mmHg");
//                                    break;
//                                case 2:
//                                    listViewItem.SubItems.Add("Resolution(Press)=" + num9.ToString() + "Pa");
//                                    break;
//                                case 3:
//                                    listViewItem.SubItems.Add("Resolution(Press)=" + num9.ToString() + "cmH2O");
//                                    break;
//                                case 4:
//                                    listViewItem.SubItems.Add("Resolution=" + num9.ToString() + "mmHg/s");
//                                    break;
//                                case 5:
//                                    listViewItem.SubItems.Add("Resolution=" + num9.ToString() + "dyne");
//                                    break;
//                                case 6:
//                                    listViewItem.SubItems.Add("Resolution=" + num9.ToString() + "N");
//                                    break;
//                                case 7:
//                                    listViewItem.SubItems.Add("Resolution=" + num9.ToString() + "%");
//                                    break;
//                                case 8:
//                                    listViewItem.SubItems.Add("Resolution(Temp)=" + num9.ToString() + "C");
//                                    break;
//                                case 9:
//                                    listViewItem.SubItems.Add("Resolution=" + num9.ToString() + "/m");
//                                    break;
//                                case 10:
//                                    listViewItem.SubItems.Add("Resolution=" + num9.ToString() + "/s");
//                                    break;
//                                case 11:
//                                    listViewItem.SubItems.Add("Resolution=" + num9.ToString() + "ohm");
//                                    break;
//                                case 12:
//                                    listViewItem.SubItems.Add("Resolution=" + num9.ToString() + "A");
//                                    break;
//                                default:
//                                    num9 = 0.0;
//                                    listViewItem.SubItems.Add("Definition definition error");
//                                    break;
//                            }
//                            waveview.resolution = num9;
//                            if (ch == 0)
//                            {
//                                for (int chn = 1; chn <= waveview.channel; ++chn)
//                                    this.GetDefInf(chn, this.frm).resolution = waveview.resolution;
//                                goto label_246;
//                            }
//                            else
//                                goto label_246;
//                        }
//                        else
//                        {
//                            listViewItem.SubItems.Add("Definition length error");
//                            goto label_246;
//                        }
//                    case 13:
//                        string text28 = text4 + "Offset";
//                        listViewItem.SubItems.Add(text28);
//                        string text29 = len.ToString();
//                        listViewItem.SubItems.Add(text29);
//                        num7 = this.GetData32(out dt4, len, point + num3, bf);
//                        listViewItem.SubItems.Add("Offset=" + dt4.ToString());
//                        goto label_246;
//                    case 14:
//                        string text30 = text4 + "Compression";
//                        listViewItem.SubItems.Add(text30);
//                        string text31 = len.ToString();
//                        listViewItem.SubItems.Add(text31);
//                        num7 = this.GetData16(out dt4, len, point + num3, bf);
//                        switch (dt4)
//                        {
//                            case 0:
//                                listViewItem.SubItems.Add("No-compression");
//                                goto label_246;
//                            case 1:
//                                waveview.dType = 9;
//                                if (ch == 0)
//                                {
//                                    for (int chn = 1; chn <= waveview.channel; ++chn)
//                                        this.GetDefInf(chn, this.frm).dType = 9;
//                                    goto label_246;
//                                }
//                                else
//                                    goto label_246;
//                            default:
//                                goto label_246;
//                        }
//                    case 17:
//                        string text32 = text4 + "Filter";
//                        listViewItem.SubItems.Add(text32);
//                        string text33 = len.ToString();
//                        listViewItem.SubItems.Add(text33);
//                        string text34 = "";
//                        for (int index6 = 0; index6 < len; ++index6)
//                            text34 += (string)(object)(char)bf[point + num3 + index6];
//                        listViewItem.SubItems.Add(text34);
//                        goto label_246;
//                    case 18:
//                        string text35 = text4 + "Null value";
//                        listViewItem.SubItems.Add(text35);
//                        string text36 = len.ToString();
//                        listViewItem.SubItems.Add(text36);
//                        string str1 = "Null data=0x";
//                        int data32 = this.GetData32(out dt4, len, point + num3, bf);
//                        byte dt5 = (byte)(dt4 >> 8 & (int)byte.MaxValue);
//                        if (data32 != 0 || dt5 != (byte)0)
//                        {
//                            this.ToHex(dt5, out sr2);
//                            str1 += sr2;
//                        }
//                        this.ToHex((byte)(dt4 & (int)byte.MaxValue), out sr2);
//                        text4 = str1 + sr2;
//                        listViewItem.SubItems.Add(text4);
//                        waveview.nil = dt4;
//                        break;
//                    case 19:
//                    case 65:
//                        if (this.eventCounter <= this.eventNumber)
//                        {
//                            if (this.eventNumber == this.eventCounter && this.EnterEventNo() < 0)
//                                return -1;
//                            string text37 = text4 + "Event";
//                            listViewItem.SubItems.Add(text37);
//                            string text38 = len.ToString();
//                            listViewItem.SubItems.Add(text38);
//                            ++this.eventCounter;
//                            goto label_246;
//                        }
//                        else
//                            goto label_248;
//                    case 22:
//                        string text39 = text4 + "Comment";
//                        listViewItem.SubItems.Add(text39);
//                        string text40 = len.ToString();
//                        listViewItem.SubItems.Add(text40);
//                        string text41 = "";
//                        for (int index7 = 0; index7 < len; ++index7)
//                            text41 += (string)(object)(char)bf[point + num3 + index7];
//                        listViewItem.SubItems.Add(text41);
//                        goto label_246;
//                    case 23:
//                        string text42 = text4 + "Manufacturer";
//                        listViewItem.SubItems.Add(text42);
//                        string text43 = len.ToString();
//                        listViewItem.SubItems.Add(text43);
//                        string text44 = "";
//                        for (int index8 = 0; index8 < len; ++index8)
//                            text44 += (string)(object)(char)bf[point + num3 + index8];
//                        listViewItem.SubItems.Add(text44);
//                        goto label_246;
//                    case 26:
//                        string text45 = text4 + "Patient Name(old)";
//                        listViewItem.SubItems.Add(text45);
//                        string text46 = len.ToString();
//                        listViewItem.SubItems.Add(text46);
//                        string text47 = "";
//                        for (int index9 = 0; index9 < len; ++index9)
//                            text47 += (string)(object)(char)bf[point + num3 + index9];
//                        listViewItem.SubItems.Add(text47);
//                        goto label_246;
//                    case 30:
//                        string text48 = text4 + "Waveform";
//                        listViewItem.SubItems.Add(text48);
//                        string text49 = len.ToString();
//                        listViewItem.SubItems.Add(text49);
//                        int num10 = 1;
//                        if (this.GetDefInf(0, this.frm).dType != 9)
//                            num10 = 2;
//                        int num11 = this.GetDefInf(0, this.frm).sequence * this.GetDefInf(0, this.frm).block / this.GetDefInf(0, this.frm).channel;
//                        if (num11 < len / this.GetDefInf(0, this.frm).channel / num10)
//                            num11 = len / this.GetDefInf(0, this.frm).channel / num10;
//                        if (this.GetDefInf(0, this.frm).block == 1 && this.GetDefInf(0, this.frm).sequence == 1)
//                            this.GetDefInf(0, this.frm).sequence = num11;
//                        for (int chn = 1; chn <= this.GetDefInf(0, this.frm).channel; ++chn)
//                        {
//                            Waveview defInf = this.GetDefInf(chn, this.frm);
//                            if (defInf.block == 1 && defInf.sequence == 1)
//                                defInf.sequence = num11;
//                            defInf.wave = new short[defInf.block * defInf.sequence];
//                            defInf.pData = 0;
//                        }
//                        int num12 = 0;
//                        int sequence = this.GetDefInf(0, this.frm).sequence;
//                        for (int index10 = 0; index10 < sequence; ++index10)
//                        {
//                            int channel = this.GetDefInf(0, this.frm).channel;
//                            for (int chn = 1; chn <= channel; ++chn)
//                            {
//                                Waveview defInf = this.GetDefInf(chn, this.frm);
//                                string str2;
//                                for (int index11 = 0; index11 < defInf.block; ++index11)
//                                {
//                                    if (len <= num12)
//                                    {
//                                        str2 = "Unmatched Waveform length(-" + (len - num12).ToString() + " bytes)";
//                                        goto label_180;
//                                    }
//                                    else if (point + num3 + num12 >= dtLen)
//                                    {
//                                        str2 = "Unmatched file size(-" + (len - num12).ToString() + " bytes)";
//                                        goto label_180;
//                                    }
//                                    else if (defInf.dType == 9)
//                                    {
//                                        dt4 = (int)bf[point + num3 + num12];
//                                        if (dt4 >= 128)
//                                            dt4 -= 256;
//                                        defInf.pData += dt4;
//                                        ++num12;
//                                        defInf.wave[index10 * defInf.block + index11] = (short)defInf.pData;
//                                    }
//                                    else
//                                    {
//                                        this.GetData16(out dt4, 2, point + num3 + num12, bf);
//                                        num12 += 2;
//                                        defInf.wave[index10 * defInf.block + index11] = (short)dt4;
//                                    }
//                                }
//                            }
//                        }
//                    label_180:
//                        if (ch == 0)
//                            ++this.frm;
//                        int num13 = len / num10;
//                        listViewItem.SubItems.Add("Waveform data=" + num13.ToString());
//                        waveview = (Waveview)null;
//                        goto label_246;
//                    case 63:
//                        string text50 = text4 + "Channel(" + bf[point + 1].ToString() + ") attribute";
//                        listViewItem.SubItems.Add(text50);
//                        string text51 = len.ToString();
//                        listViewItem.SubItems.Add(text51);
//                        this.MFERFormat.Items.AddRange(new ListViewItem[1]
//                        {
//                  listViewItem
//                        });
//                        this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num3, point + num3 + len, bf);
//                        flag = false;
//                        goto label_246;
//                    case 64:
//                        string text52 = text4 + "Preamble";
//                        listViewItem.SubItems.Add(text52);
//                        string text53 = len.ToString();
//                        listViewItem.SubItems.Add(text53);
//                        string text54 = "";
//                        for (int index12 = 0; index12 < num3 + len; ++index12)
//                            text54 += (string)(object)(char)bf[point + index12];
//                        listViewItem.SubItems.Add(text54);
//                        goto label_246;
//                    case 103:
//                        string text55 = text4 + "Group";
//                        listViewItem.SubItems.Add(text55);
//                        this.MFERFormat.Items.AddRange(new ListViewItem[1]
//                        {
//                  listViewItem
//                        });
//                        this.TLVanz(mode + 1, (int)bf[point + 1] + 1, point + num3, point + num3 + len, bf);
//                        flag = false;
//                        goto label_246;
//                    case 128:
//                        listViewItem.SubItems.Add("End of format");
//                        this.MFERFormat.Items.AddRange(new ListViewItem[1]
//                        {
//                  listViewItem
//                        });
//                        return 0;
//                    case 129:
//                        string text56 = text4 + "Person Name";
//                        listViewItem.SubItems.Add(text56);
//                        string text57 = len.ToString();
//                        listViewItem.SubItems.Add(text57);
//                        string text58 = "";
//                        for (int index13 = 0; index13 < len; ++index13)
//                            text58 += (string)(object)(char)bf[point + num3 + index13];
//                        listViewItem.SubItems.Add(text58);
//                        goto label_246;
//                    case 130:
//                        string text59 = text4 + "Person ID";
//                        listViewItem.SubItems.Add(text59);
//                        string text60 = len.ToString();
//                        listViewItem.SubItems.Add(text60);
//                        string text61 = "";
//                        for (int index14 = 0; index14 < len; ++index14)
//                            text61 += (string)(object)(char)bf[point + num3 + index14];
//                        listViewItem.SubItems.Add(text61);
//                        goto label_246;
//                    case 131:
//                        string text62 = text4 + "Age";
//                        listViewItem.SubItems.Add(text62);
//                        string text63 = len.ToString();
//                        listViewItem.SubItems.Add(text63);
//                        string text64 = "";
//                        if (len >= 1)
//                            text64 = text64 + bf[point + num3].ToString() + "(Years)";
//                        if (len >= 2)
//                        {
//                            this.GetData16(out dt4, 2, point + num3 + 1, bf);
//                            text64 = text64 + " " + dt4.ToString() + "(Month)";
//                        }
//                        if (len >= 4)
//                        {
//                            this.GetData16(out dt4, 2, point + num3 + 3, bf);
//                            text64 = text64 + dt4.ToString() + "/";
//                        }
//                        if (len >= 6)
//                            text64 = text64 + bf[point + num3 + 5].ToString() + "/";
//                        if (len >= 7)
//                            text64 += bf[point + num3 + 6].ToString();
//                        listViewItem.SubItems.Add(text64);
//                        goto label_246;
//                    case 132:
//                        string text65 = text4 + "Sex";
//                        listViewItem.SubItems.Add(text65);
//                        string text66 = len.ToString();
//                        listViewItem.SubItems.Add(text66);
//                        if (bf[point + num3] == (byte)0)
//                            text66 = "Undefined";
//                        if (bf[point + num3] == (byte)1)
//                            text66 = "Male";
//                        if (bf[point + num3] == (byte)2)
//                            text66 = "Female";
//                        if (bf[point + num3] == (byte)3)
//                            text66 = "Unclassified";
//                        listViewItem.SubItems.Add(text66);
//                        goto label_246;
//                    case 133:
//                        string text67 = text4 + "Measurement time";
//                        listViewItem.SubItems.Add(text67);
//                        string text68 = len.ToString();
//                        listViewItem.SubItems.Add(text68);
//                        string text69 = "";
//                        if (len >= 1)
//                        {
//                            this.GetData16(out dt4, 2, point + num3, bf);
//                            text69 = text69 + dt4.ToString() + "/";
//                        }
//                        if (len >= 3)
//                            text69 = text69 + bf[point + num3 + 2].ToString() + "/";
//                        if (len >= 4)
//                            text69 = text69 + bf[point + num3 + 3].ToString() + " ";
//                        if (len >= 5)
//                            text69 = text69 + bf[point + num3 + 4].ToString() + ":";
//                        if (len >= 6)
//                            text69 = text69 + bf[point + num3 + 5].ToString() + ":";
//                        if (len >= 7)
//                            text69 = text69 + bf[point + num3 + 6].ToString() + " ";
//                        if (len >= 8)
//                            text69 = text69 + bf[point + num3 + 7].ToString() + ":";
//                        if (len >= 9)
//                            text69 = text69 + bf[point + num3 + 8].ToString() + ":";
//                        if (len >= 10)
//                            text69 += bf[point + num3 + 9].ToString();
//                        listViewItem.SubItems.Add(text69);
//                        goto label_246;
//                    case 134:
//                        string text70 = text4 + "Message";
//                        listViewItem.SubItems.Add(text70);
//                        string text71 = len.ToString();
//                        listViewItem.SubItems.Add(text71);
//                        string text72 = "";
//                        for (int index15 = 0; index15 < len; ++index15)
//                            text72 += (string)(object)(char)bf[point + num3 + index15];
//                        listViewItem.SubItems.Add(text72);
//                        goto label_246;
//                }
//                string text73 = text4 + "Unclassfied by this tool";
//                listViewItem.SubItems.Add(text73);
//                string text74 = len.ToString();
//                listViewItem.SubItems.Add(text74);
//            }
//        label_246:
//            if (flag)
//                this.MFERFormat.Items.AddRange(new ListViewItem[1]
//                {
//              listViewItem
//                });
//            label_248:
//            point += num3 + len;
//        }
//        catch
//        {
//            this.MFERFormat.Items.AddRange(new ListViewItem[1]
//            {
//            new ListViewItem() { SubItems = { "", "", "Format error" } }
//            });
//            return 0;
//        }
//    }
//    return 0;
//}
