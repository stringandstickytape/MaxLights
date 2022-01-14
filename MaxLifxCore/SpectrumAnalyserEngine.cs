using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MaxLifxCore;
using NAudio.CoreAudioApi;
using NAudio.Dsp;
using NAudio.Wave;

namespace MaxLifxCore.Controls
{

    public struct BPMGroup
    {
        public int Count;
        public float Tempo;
    }


    public class SpectrumAnalyserEngine
    {
        // Based on the nAudio source code

        // Other inputs are also usable. Just look through the NAudio library.
        private static readonly int FftLength = 1024; // NAudio fft wants powers of two!
        private readonly SampleAggregator _sampleAggregator = new SampleAggregator(FftLength);
        public int Bins = 512; // guess a 1024 size FFT, bins is half FFT size
        public List<Point> LatestPoints;
        public int SelectedBin = 10;
        private IWaveIn _waveIn;
        private AppController _appController;
        public SpectrumAnalyserEngine(ref AppController appController)
        {
            _appController = appController;
            LatestPoints = new List<Point>{new Point(0, 100), new Point(1, 50)};
        }

        public void StartCapture()
        {
            _sampleAggregator.FftCalculated += FftCalculated;
            //var deviceEnum = new MMDeviceEnumerator();
            //var device = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);

            if (_waveIn == null)
            {
                _waveIn = new WasapiLoopbackCapture(); // device);
                _waveIn.DataAvailable += OnDataAvailable;
                _waveIn.RecordingStopped += OnRecordingStopped;
            }
            // Forcibly turn on the microphone (some programs (Skype) turn it off).
            //device.AudioEndpointVolume.Mute = false;


            // disable "Enable Audio Enahncements" if you crash here.
            _waveIn.StartRecording();
        }

        private void FftCalculated(object sender, FftEventArgs e)
        {
            Update(e.Result);
        }

        public void Update(Complex[] fftResults)
        {
            if (fftResults.Length/2 != Bins)
            {
                Bins = fftResults.Length/2;
            }

            var points = new List<Point>();
            for (var n = 0; n < fftResults.Length/2; n ++)
            {
                points.Add(new Point(n, GetYPosLog(fftResults[n])));
            }
            LatestPoints = points;
            _appController.LatestPoints = points;
        }

        private int GetYPosLog(Complex c)
        {
            // not entirely sure whether the multiplier should be 10 or 20 in this case.
            // going with 10 from here http://stackoverflow.com/a/10636698/7532
            var intensityDb = 10*Math.Log10(Math.Sqrt(c.X*c.X + c.Y*c.Y));
            double minDb = -90;
            if (intensityDb < minDb) intensityDb = minDb;
            var percent = intensityDb/minDb;
            // we want 0dB to be at the top (i.e. yPos = 0)
            var yPos = percent*200;
            return (int)yPos;
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show($"A problem was encountered during recording {e.Exception.Message}");
            }
        }

        private byte[] massiveBuffer = new byte[10000000];
        private int byteCtr = 0;


        public BPMGroup[] groups;

        public BPMGroup[] Groups
        {
            get
            {
                return groups;
            }
        }

        private int sampleRate;
        private int channels;

        private struct Peak
        {
            public int Position;
            public float Volume;
        }

        private Peak[] getPeaks(float[] data, int length)
        {
            // What we're going to do here, is to divide up our audio into parts.

            // We will then identify, for each part, what the loudest sample is in that
            // part.

            // It's implied that that sample would represent the most likely 'beat'
            // within that part.

            // Each part is 0.5 seconds long

            // This will give us 60 'beats' - we will only take the loudest half of
            // those.

            // This will allow us to ignore breaks, and allow us to address tracks with
            // a BPM below 120.

            float v;

            int i, j, k;

            int partSize = sampleRate / 4;
            int parts = length / channels / partSize;
            Peak[] peaks = new Peak[parts];

            for (i = 0; i < parts; ++i)
            {
                Peak max = new Peak
                {
                    Position = -1,
                    Volume = 0.0F
                };

                var dataRef = i * channels * partSize;

                for (j = 0; j < partSize; ++j)
                {
                    // think we could get away with just one channel
                    v = data[dataRef + j * channels];
                    if (max.Position == -1 || max.Volume < v)
                    {
                        max.Volume = v;
                        max.Position = i * partSize + j;
                    }

                    /*for (k = 0; k < 1; ++k)
                    {
                        v = data[i * channels * partSize + j * channels + k];

                        if (vol < v) vol = v;
                    } 

                    if (max.Position == -1 || max.Volume < vol)
                    {
                        max.Position = i * partSize + j;
                        max.Volume = vol;
                    }*/
                }
                peaks[i] = max;
            }

            // We then sort the peaks according to volume...

            Array.Sort(peaks, (x, y) => y.Volume.CompareTo(x.Volume));

            // ...take the loundest half of those...

            //Array.Resize(ref peaks, peaks.Length / 2);

            // ...and re-sort it back based on position.

            Array.Sort(peaks, (x, y) => x.Position.CompareTo(y.Position));

            return peaks;
        }

        private List<float> getIntervals(Peak[] peaks)
        {
            // What we now do is get all of our peaks, and then measure the distance to
            // other peaks, to create intervals.  Then based on the distance between
            // those peaks (the distance of the intervals) we can calculate the BPM of
            // that particular interval.

            // The interval that is seen the most should have the BPM that corresponds
            // to the track itself.

            //List<BPMGroup> groups = new List<BPMGroup>();

            List<float> retVal = new List<float>();

            for (int index = 0; index < peaks.Length; ++index)
            {
                Peak peak = peaks[index];
                for (int i = 1; index + i < peaks.Length && i < 10; ++i)
                {
                    float tempo = 30.0F * sampleRate / (peaks[index + i].Position - peak.Position);
                    while (tempo < 60.0F)
                    {
                        tempo *= 2.0F;
                    }
                    while (tempo > 180.0F)
                    {
                        tempo /= 2.0F;
                    }

                    retVal.Add(tempo);
                    /*BPMGroup group = new BPMGroup
                    {
                        Count = 1,
                        Tempo = (float)/*Math.Round*//*(tempo)
                    };
                    int j;
                    for (j = 0; j < groups.Count && groups[j].Tempo != group.Tempo; ++j) { }
                    if (j < groups.Count)
                    {
                        group.Count = groups[j].Count + 1;
                        groups[j] = group;
                    }
                    else
                    {
                        groups.Add(group);
                    }*/
                }
            }
            //return groups.ToArray();
            return retVal;
        }

        private Queue<float[]> sampleSetQueue = new Queue<float[]>(90);

        private List<float[]> sampleSets = new List<float[]>();

        private int sampleCtr;
        private float[] sampleBuf = new float[10000000];
        private float[] sampleBuf2 = new float[10000000];

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            var buffer = e.Buffer;
            var bytesRecorded = e.BytesRecorded;
            var bufferIncrement = _waveIn.WaveFormat.BlockAlign;

            {
                //sampleCtr = 0;
                GenerateBPM1(sender, buffer, bytesRecorded);

                

                //Array.Copy(buffer, 0, byteBuf, byteBufCtr, bytesRecorded);
                //byteBufCtr += bytesRecorded;
                //
                //var b = new BPMDetector(byteBuf, byteBufCtr, _waveIn.WaveFormat.SampleRate);
                //double bpm = b.getBPM();
                //System.Diagnostics.Debug.WriteLine(bpm);
            }

            for (var index = 0; index < bytesRecorded; index += bufferIncrement)
            {
                var sample32 = BitConverter.ToSingle(buffer, index);
                _sampleAggregator.Add(sample32);
            }
        }
        float[] floatArr = new float[10000000];
        private Queue<float> prevResults = new Queue<float>(10);

        public float BPM;

        private void GenerateBPM1(object sender, byte[] buffer, int bytesRecorded)
        {
            sampleRate = ((WasapiLoopbackCapture)sender).WaveFormat.SampleRate;
            channels = ((WasapiLoopbackCapture)sender).WaveFormat.Channels;

            int bytesPerSample = ((WasapiLoopbackCapture)sender).WaveFormat.BitsPerSample / 8;
            if (bytesPerSample == 0)
            {
                bytesPerSample = 2; // assume 16 bit
            }

            int sampleCount = bytesRecorded / bytesPerSample;

            // Read the wave data

            var start = 0;
            start *= channels * sampleRate;
            var length = 0;
            length *= channels * sampleRate;

            if (start >= sampleCount)
            {
                groups = new BPMGroup[0];
            }
            else
            {
                if (length == 0 || start + length >= sampleCount)
                {
                    length = sampleCount - start;
                }

                length = (int)(length / channels) * channels;

                IWaveProvider provider = new RawSourceWaveStream(buffer, 0, bytesRecorded, ((WasapiLoopbackCapture)sender).WaveFormat);
                ISampleProvider sampleReader = provider.ToSampleProvider();
                float[] samples = new float[length];
                sampleReader.Read(samples, start, length);

                // Beats, or kicks, generally occur around the 100 to 150 hz range.
                // Below this is often the bassline.  So let's focus just on that.

                for (int ch = 0; ch < channels; ++ch)
                {
                    // First a lowpass to remove most of the song.
                
                    BiQuadFilter lowpass = BiQuadFilter.LowPassFilter(sampleRate, 140, 1.0F);
                
                    // Now a highpass to remove the bassline.
                
                    BiQuadFilter highpass = BiQuadFilter.HighPassFilter(sampleRate, 100, 1.0F);
                
                    for (int i = ch; i < length; i += channels)
                    {
                        samples[i] = highpass.Transform(lowpass.Transform(samples[i]));
                        //samples[i] = lowpass.Transform(samples[i]);
                    }
                }

                if (sampleCtr + samples.Length > 120000)
                {
                    sampleCtr = 0;
                    //Array.Copy(sampleBuf, samples.Length, sampleBuf2, 0, sampleCtr - samples.Length);
                    //Array.Copy(sampleBuf2, 0, sampleBuf, 0, sampleCtr - samples.Length);
                    //sampleCtr = sampleCtr - samples.Length * 10;
                    //sampleBufSwap = sampleBuf;
                    //sampleBuf = sampleBuf2;
                    //sampleBuf2 = sampleBufSwap;
                }
                
                Array.Copy(samples, 0, sampleBuf, sampleCtr, samples.Length);
                sampleCtr += samples.Length;
                
                var tempos = getIntervals(getPeaks(sampleBuf, sampleCtr));

                if (tempos.Any())
                {
                    var b = tempos.Skip(tempos.Count / 2).Take(1).ToList()[0];
                    //System.Diagnostics.Debug.WriteLine($"{b}");
                    sampleCtr = 0;
                    prevResults.Enqueue(b);
                    if (prevResults.Count > 20) prevResults.Dequeue();
                    if (prevResults.Any())
                    {
                        BPM = prevResults.OrderBy(x => x).Skip(prevResults.Count / 2).Take(1).ToList()[0];
                        //System.Diagnostics.Debug.WriteLine($"{BPM}  {b} {tempos.Average()}");
                    }
                }
                //
                //sampleSets.Add(samples);
                //
                //sampleSetQueue.Enqueue(samples);
                //if (sampleSetQueue.Count > 90) 
                //    sampleSetQueue.Dequeue();
                //
                ////if (sampleSets.Count > 90) 
                ////    sampleSets = sampleSets.Skip(sampleSets.Count - 90).Take(90).ToList();
                //
                //var q = sampleSetQueue.Sum(x => x.Count());
                //
                //var ctr = 0;
                //foreach(var fl in sampleSetQueue)
                //{
                //    Array.Copy(fl, 0, floatArr, ctr, fl.Length);
                //    ctr += fl.Length;
                //}
                //
                //var tempos = getIntervals(getPeaks(floatArr, q)).OrderBy(x => x).ToList();
                //
                //if(tempos.Any())
                //    System.Diagnostics.Debug.WriteLine($"{tempos.Skip(tempos.Count / 2).Take(1).ToList()[0]}");
                //Array.Sort(allGroups, (x, y) => y.Count.CompareTo(x.Count));
                //
                ////if (allGroups.Length > 5)
                ////{
                ////    Array.Resize(ref allGroups, 5);
                ////}
                //
                //this.groups = allGroups;
                //
                //if (allGroups.Length > 0)
                //{
                //   //for (var ii = 0; ii < allGroups.Length; ii++)
                //   //{
                //   //    System.Diagnostics.Debug.Write($"{allGroups[ii].Tempo}x{allGroups[ii].Count};  ");
                //   //}
                //
                //    var thing = new List<float>();
                //    foreach(var g in allGroups)
                //    {
                //        for (var g2 = 0; g2 < g.Count; g2++)
                //        {
                //            thing.Add(g.Tempo);
                //        }
                //    }
                //    thing = thing.OrderBy(x => x).ToList();
                //    System.Diagnostics.Debug.WriteLine($"{thing.Skip(thing.Count / 2).Take(1).ToList()[0]}");
                //    //System.Diagnostics.Debug.WriteLine($"{allGroups.OrderBy(x => x.Tempo).Skip(allGroups.}");
                //}


                /*
                                {
                                    var leftChn1 = new List<short>();

                                    for (var iii = 0; iii < sampleCtr / 2; iii++)
                                    {
                                        leftChn1.Add((short)(sampleBuf[iii * 2] * 32767));
                                    }
                                    var leftChn = leftChn1.ToArray();

                                    var trackLength = (float)leftChn.Length / sampleRate;

                                    // 0.1s window ... 0.1*44100 = 4410 samples, lets adjust this to 3600 
                                    int sampleStep = 3600;

                                    // calculate energy over windows of size sampleSetep
                                    List<double> energies = new List<double>();
                                    for (int i = 0; i < leftChn.Length - sampleStep - 1; i += sampleStep)
                                    {
                                        energies.Add(rangeQuadSum(leftChn, i, i + sampleStep));
                                    }

                                    int beats = 0;
                                    double average = 0;
                                    double sumOfSquaresOfDifferences = 0;
                                    double variance = 0;
                                    double newC = 0;
                                    List<double> variances = new List<double>();

                                    // how many energies before and after index for local energy average
                                    int offset = 10;

                                    for (int i = offset; i <= energies.Count - offset - 1; i++)
                                    {
                                        // calculate local energy average
                                        double currentEnergy = energies[i];
                                        double qwe = rangeSum(energies.ToArray(), i - offset, i - 1) + currentEnergy + rangeSum(energies.ToArray(), i + 1, i + offset);
                                        qwe /= offset * 2 + 1;

                                        // calculate energy variance of nearby energies
                                        List<double> nearbyEnergies = energies.Skip(i - 5).Take(5).Concat(energies.Skip(i + 1).Take(5)).ToList<double>();
                                        average = nearbyEnergies.Average();
                                        sumOfSquaresOfDifferences = nearbyEnergies.Select(val => (val - average) * (val - average)).Sum();
                                        variance = (sumOfSquaresOfDifferences / nearbyEnergies.Count) / Math.Pow(10, 22);

                                        // experimental linear regression - constant calculated according to local energy variance
                                        newC = variance * 0.009 + 1;
                                        if (currentEnergy > newC * qwe)
                                            beats++;
                                    }

                                    var BPM = beats / (trackLength/5)*30 ;
                                    System.Diagnostics.Debug.WriteLine(BPM);

                                }
                */
            }
        }

        public void StopCapture()
        {
            _waveIn?.StopRecording();
        }


        private static double rangeQuadSum(short[] samples, int start, int stop)
        {
            double tmp = 0;
            for (int i = start; i <= stop; i++)
            {
                tmp += Math.Pow(samples[i], 2);
            }

            return tmp;
        }

        private static double rangeSum(double[] data, int start, int stop)
        {
            double tmp = 0;
            for (int i = start; i <= stop; i++)
            {
                tmp += data[i];
            }

            return tmp;
        }
    }




    class BPMDetector
    {
        private string filename = null;
        private short[] leftChn;
        private short[] rightChn;
        private double BPM;
        private double sampleRate = 44100;
        private double trackLength = 0;

        public double getBPM()
        {
            return BPM;
        }

        public BPMDetector(string filename)
        {
            this.filename = filename;
            Detect();
        }

        public BPMDetector(short[] leftChn, short[] rightChn)
        {
            this.leftChn = leftChn;
            this.rightChn = rightChn;
            Detect();
        }

        public BPMDetector(byte[] bytes, int byteCtr, int inSampleRate)
        {
            sampleRate = inSampleRate;

            // convert buffer to ushort
            short[] sampleBuffer = new short[byteCtr / 2];
            Buffer.BlockCopy(bytes, 0, sampleBuffer, 0, byteCtr);

            List<short> chan1 = new List<short>();
            List<short> chan2 = new List<short>();

            for (int i = 0; i < byteCtr; i += 4)
            {
                chan1.Add((short)(bytes[i]*256+ bytes[i+1]));
                chan2.Add((short)(bytes[i+2]*256+ bytes[i+3]));
            }

            leftChn = chan1.ToArray();
            rightChn = chan2.ToArray();

            Detect();
        }

        private void Detect()
        {
            if (filename != null)
            {
                using (WaveFileReader reader = new WaveFileReader(filename))
                {
                    byte[] buffer = new byte[reader.Length];
                    int read = reader.Read(buffer, 0, buffer.Length);

                    short[] sampleBuffer = new short[read / 2];
                    Buffer.BlockCopy(buffer, 0, sampleBuffer, 0, read);

                    List<short> chan1 = new List<short>();
                    List<short> chan2 = new List<short>();

                    for (int i = 0; i < sampleBuffer.Length; i += 2)
                    {
                        chan1.Add(sampleBuffer[i]);
                        chan2.Add(sampleBuffer[i + 1]);
                    }

                    leftChn = chan1.ToArray();
                    rightChn = chan2.ToArray();
                }
            }

            
            trackLength = (float)leftChn.Length / sampleRate;

            // 0.1s window ... 0.1*44100 = 4410 samples, lets adjust this to 3600 
            int sampleStep = 3600;

            // calculate energy over windows of size sampleSetep
            List<double> energies = new List<double>();
            for (int i = 0; i < leftChn.Length - sampleStep - 1; i += sampleStep)
            {
                energies.Add(rangeQuadSum(leftChn, i, i + sampleStep));
            }

            int beats = 0;
            double average = 0;
            double sumOfSquaresOfDifferences = 0;
            double variance = 0;
            double newC = 0;
            List<double> variances = new List<double>();

            // how many energies before and after index for local energy average
            int offset = 10;

            for (int i = offset; i <= energies.Count - offset - 1; i++)
            {
                // calculate local energy average
                double currentEnergy = energies[i];
                double qwe = rangeSum(energies.ToArray(), i - offset, i - 1) + currentEnergy + rangeSum(energies.ToArray(), i + 1, i + offset);
                qwe /= offset * 2 + 1;

                // calculate energy variance of nearby energies
                List<double> nearbyEnergies = energies.Skip(i - 5).Take(5).Concat(energies.Skip(i + 1).Take(5)).ToList<double>();
                average = nearbyEnergies.Average();
                sumOfSquaresOfDifferences = nearbyEnergies.Select(val => (val - average) * (val - average)).Sum();
                variance = (sumOfSquaresOfDifferences / nearbyEnergies.Count) / Math.Pow(10, 22);

                // experimental linear regression - constant calculated according to local energy variance
                newC = variance * 0.009 + 1;
                if (currentEnergy > newC * qwe)
                    beats++;
            }

            BPM = beats / (trackLength / 5);

        }

        private static double rangeQuadSum(short[] samples, int start, int stop)
        {
            double tmp = 0;
            for (int i = start; i <= stop; i++)
            {
                tmp += Math.Pow(samples[i], 2);
            }

            return tmp;
        }

        private static double rangeSum(double[] data, int start, int stop)
        {
            double tmp = 0;
            for (int i = start; i <= stop; i++)
            {
                tmp += data[i];
            }

            return tmp;
        }
    }
}