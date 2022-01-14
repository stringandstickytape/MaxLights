using MaxLifxCoreBulbController.Payload;
using MaxLifxCoreBulbController.Util;
using System;
using System.Linq;

namespace MaxLifxCoreBulbController.Packets
{
    public class PacketFactory
    {
        // from https://stackoverflow.com/questions/415291/best-way-to-combine-two-or-more-byte-arrays-in-c-sharp
        public static byte[] Combine(byte[] first, byte[] second, byte[] third)
        {
            byte[] ret = new byte[first.Length + second.Length + third.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length,
                             third.Length);
            return ret;
        }

        public static byte[] GetPacket(byte[] targetMacAddress, IPayload Payload)
        {
            if (targetMacAddress.Length != 8) throw new NotImplementedException();

            // http://lan.developer.lifx.com/v2.0/docs/header-description#frame

            byte[] sizelessHeader = GetSizelessHeader(targetMacAddress, Payload);
            byte[] payloadBytes = Payload.GetPayload();

            var frameSizeInt = Convert.ToInt16(2 + sizelessHeader.Length + payloadBytes.Length);
            var frameSize = BitConverter.GetBytes(frameSizeInt);

            return Combine(frameSize, sizelessHeader, payloadBytes);
        }

        // The following is interpreted from https://community.lifx.com/t/building-a-lifx-packet/59
        private static byte[] GetSizelessHeader(byte[] targetMacAddress, IPayload payload)
        {
            var sizelessHeader2 = new byte[34];

            var originAddressTaggedProtocol = new byte[2];
            
            if ((targetMacAddress.Sum(x => x) == 0) || payload.GetType() == typeof(GetServicePayload))
                Utils.SetBit(ref originAddressTaggedProtocol, 5);
            
            Utils.SetBit(ref originAddressTaggedProtocol, 4);
            Utils.SetBit(ref originAddressTaggedProtocol, 2);
            
            Array.Reverse(originAddressTaggedProtocol, 0, originAddressTaggedProtocol.Length);

            sizelessHeader2[0] = originAddressTaggedProtocol[0];
            sizelessHeader2[1] = originAddressTaggedProtocol[1];

            // The next 32 bit(4 bytes) are the source, which are unique to the client and used to identify broadcasts that it cares about.Since 
            // we are a dumb client and don't care about the response, lets set this all to zero (0). If you are writing a client program you'll 
            // want to use a unique value here.
            // we want the response of the following payloads:
            var type = payload.GetType();
            if ((targetMacAddress.Any(x => x > 0)) && type != typeof(GetServicePayload) && type != typeof(GetLabelPayload) && type != typeof(GetColourZonesPayload) && type != typeof(GetVersionPayload))
            {
                sizelessHeader2[2] = 1;
                sizelessHeader2[3] = 2;
                sizelessHeader2[4] = 3;
                sizelessHeader2[5] = 4;
            }

            Buffer.BlockCopy(targetMacAddress, 0, sizelessHeader2, 6, 6);

            sizelessHeader2[30] = payload.MessageType[0];
            sizelessHeader2[31] = payload.MessageType[1];

            return sizelessHeader2;
        }
    }
}
