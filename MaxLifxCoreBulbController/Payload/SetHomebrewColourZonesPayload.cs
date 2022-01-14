using MaxLifxCoreBulbController.Controllers;
using MaxLifxCoreBulbController.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxLifxCoreBulbController.Payload
{
    public class SetHomebrewColourZonesPayload : SetColourPayload, IPayload
    {
        private byte[] _messageType = new byte[2] { 0xFE, 0x01 }; // 510
        public new byte[] MessageType { get { return _messageType; } }

        public List<GetColourZonesPayload> Zones { get; set; }
        public Dictionary<int, SetColourPayload> IndividualPayloads { get; set; }

        public new byte[] GetPayload() {

            var transition = BitConverter.GetBytes(IndividualPayloads.First().Value.TransitionDuration);

            byte[] bytes = new byte[IndividualPayloads.Count()*8+8];

            bytes[0] = transition[0];
            bytes[1] = transition[1];
            bytes[2] = transition[2];
            bytes[3] = transition[3];

            bytes[4] = 1; // apply

            bytes[5] = 0;
            bytes[6] = 0; // start zone

            bytes[7] = (byte)(IndividualPayloads.Count());

            var ctr = 0;
            foreach (var individualPayload in IndividualPayloads)
            {
                var payload = individualPayload.Value;
                if (payload.Hue < 0)
                    payload.Hue = payload.Hue + 36000;
                payload.Hue = payload.Hue % 360;

                var _hsbkColourLE = BitConverter.GetBytes(payload.Hue * 182);
                //var _hsbkColour = new byte[2] { _hsbkColourLE[0], _hsbkColourLE[1] };

                //bytes[ctr * 10 + 0 + 2] = (byte)individualPayload.Key ;
                //bytes[ctr * 10 + 1+ 2] = (byte)individualPayload.Key ;

                bytes[ctr * 8 + 8] = _hsbkColourLE[0];
                bytes[ctr * 8 + 9] = _hsbkColourLE[1];

                var _saturation = BitConverter.GetBytes(payload.Saturation);

                bytes[ctr * 8 + 10] = _saturation[0];
                bytes[ctr * 8 + 11] = _saturation[1];

                var _brightness = BitConverter.GetBytes(payload.Brightness);

                bytes[ctr * 8 + 12] = _brightness[0];
                bytes[ctr * 8 + 13] = _brightness[1];

                var _kelvin = BitConverter.GetBytes(payload.Kelvin);

                bytes[ctr * 8 + 14] = _kelvin[0];
                bytes[ctr * 8 + 15] = _kelvin[1];

                ctr++;
            }

            return  bytes;
        }
    }
}
