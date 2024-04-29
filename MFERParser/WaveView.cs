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
    }
}
