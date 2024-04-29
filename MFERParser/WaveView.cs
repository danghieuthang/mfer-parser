namespace MFERParser
{
    public class Waveview
    {
        public int channel;
        public int wave_lead;
        public int block;
        public int sequence;
        public double sampling;
        public double resolution;
        public int dType;
        public int offset;
        public int nil;
        public short[] wave;
        public int chn;
        public int frm;
        public int pID;
        public Waveview wLink;
        public int pData;
        private int pPoint;

        public Waveview(int ch)
        {
            this.chn = ch;
            this.channel = 1;
            this.block = 1;
            this.sequence = 1;
            this.sampling = 0.001;
            this.resolution = 1.0;
            this.wLink = null;
            this.wave = null;
        }


        public void ViewInit(int point)
        {
            this.pData = 0;
            int num1 = this.sequence * this.block;
            int dt;
            if (this.GetWaveData(out dt, 0))
            {
                int num2 = dt;
                int num3 = num2;
                for (int pt = 1; pt < num1 && this.GetWaveData(out dt, pt); ++pt)
                {
                    if (dt > num2)
                        num2 = dt;
                    else if (dt < num3)
                        num3 = dt;
                }
            }
            this.GetWaveData(out this.pData, 0);
        }

        private bool GetWaveData(out int dt, int pt)
        {
            this.pPoint = pt;
            dt = (int)this.wave[pt];
            return true;
        }
    }
}
