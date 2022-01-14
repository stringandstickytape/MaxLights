using System;
using System.Collections.Generic;
using System.Linq;

namespace MaxLifx.Payload
{
    /// <summary>
    /// Payload for a GetLabel message
    /// </summary>
    public class SetPowerPayload : IPayload
    {
        private byte[] _messageType = new byte[2] { 21, 0 };
        public byte[] MessageType { get { return _messageType; } }

        public bool PowerState;

        public SetPowerPayload(bool powerState)
        {
            PowerState = powerState;
        }

        public byte[] GetPayload()
        {
            ushort x = (ushort)(PowerState ? 65535 : 0);

            return new byte[0].Concat(BitConverter.GetBytes(x)).ToArray();
        }
    }

    public class SetLightPowerPayload : IPayload
    {
        private byte[] _messageType = new byte[2] { 117, 0 };
        public byte[] MessageType { get { return _messageType; } }

        public ushort Power;

        public SetLightPowerPayload(ushort power)
        {
            Power = power;
        }

        public byte[] GetPayload()
        {

            return new byte[0].Concat(BitConverter.GetBytes(Power)).Concat(new List<byte> { 0, 0, 0, 0 }).ToArray();
        }
    }

    public class GetColourPayload : IPayload
    {
        private byte[] _messageType = new byte[2] { 101, 0 };
        public byte[] MessageType { get { return _messageType; } }

        public bool PowerState;

        public GetColourPayload()
        {
        }

        public byte[] GetPayload()
        {
            return new byte[0].ToArray();
        }
    }

}
