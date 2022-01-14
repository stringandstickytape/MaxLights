﻿
namespace MaxLifxCoreBulbController.Payload
{
    /// <summary>
    /// Payload for a GetLabel message
    /// </summary>
    public class GetLabelPayload : IPayload
    {
        private byte[] _messageType = new byte[2] { 23, 0 };
        public byte[] MessageType { get { return _messageType; } }

        public byte[] GetPayload()
        {
            return new byte[0];
        }
    }

    public class GetLightPowerPayload : IPayload
    {
        private byte[] _messageType = new byte[2] { 20, 0 };
        public byte[] MessageType { get { return _messageType; } }

        public byte[] GetPayload()
        {
            return new byte[0];
        }
    }
}
