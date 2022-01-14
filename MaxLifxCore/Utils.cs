using System;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using NAudio.Wave;

namespace MaxLifxCore
{
    public static class Utils
    {
        public static string ToXmlString<T>(this T input)
        {
            using (var writer = new StringWriter())
            {
                input.ToXml(writer);
                return writer.ToString();
            }
        }

        public static void ToXml<T>(this T objectToSerialize, Stream stream)
        {
            new XmlSerializer(typeof (T)).Serialize(stream, objectToSerialize);
        }

        public static void ToXml<T>(this T objectToSerialize, StringWriter writer)
        {
            new XmlSerializer(typeof (T)).Serialize(writer, objectToSerialize);
        }

        public static void FromXml<T>(this T objectToDeserialize, Stream stream)
        {
            objectToDeserialize = (T) (new XmlSerializer(typeof (T)).Deserialize(stream));
        }

        public static void FromXml<T>(this T objectToDeserialize, StringReader reader)
        {
            objectToDeserialize = (T) (new XmlSerializer(typeof (T)).Deserialize(reader));
        }

        public static void FromXmlString<T>(this string input, ref T output)
        {
            using (var reader = new StringReader(input))
            {
                output.FromXml(reader);
            }
        }

        public static ushort HueBetween(ushort hue1, ushort hue2, double dist)
        {
            ushort posDist = (ushort)(hue2 - hue1);
            ushort negDist = (ushort)(hue1 - hue2);
            if (posDist < negDist)
            {
                return (ushort)(hue1 + dist == 0.5 ? posDist >> 1 : posDist * dist);
            }
            else
            {
                return (ushort)(hue1 - dist == 0.5 ? negDist >> 1 : negDist * dist);
            }
        }

        // Color.GetSaturation() and Color.GetBrightness() return HSL values, not HSV
        // so need this to convert
        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            ColorToHSV2(color.R, color.G, color.B, out hue, out saturation, out value);
        }

        public static void ColorToHSV2(byte R, byte G, byte B, out double hue, out double saturation, out double value)
        {
            var hsv = RGB.NET.Core.HSVColor.GetHSV(new RGB.NET.Core.Color(R,G,B));

            hue = hsv.hue;
            saturation = hsv.saturation;
            value = hsv.value;
        }

    }

    public class LoopStream : WaveStream
    {
        private readonly WaveStream sourceStream;

        /// <summary>
        ///     Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">
        ///     The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        ///     or else we will not loop to the start again.
        /// </param>
        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            EnableLooping = true;
        }

        /// <summary>
        ///     Use this to turn looping on or off
        /// </summary>
        public bool EnableLooping { get; set; }

        /// <summary>
        ///     Return source stream's wave format
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        /// <summary>
        ///     LoopStream simply returns
        /// </summary>
        public override long Length
        {
            get { return sourceStream.Length; }
        }

        /// <summary>
        ///     LoopStream simply passes on positioning to source stream
        /// </summary>
        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                var bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        // something wrong with the source stream
                        break;
                    }
                    // loop
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }

 
    }
}