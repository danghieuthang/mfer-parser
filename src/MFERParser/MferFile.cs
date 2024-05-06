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
}
