using System.Collections.Generic;
using RGB.NET.Core;

namespace RGB.NET.Devices.Logitech
{
    /// <inheritdoc cref="LogitechRGBDevice{TDeviceInfo}" />
    /// <summary>
    /// Represents a logitech per-key-lightable device.
    /// </summary>
    public class LogitechPerKeyRGBDevice : LogitechRGBDevice<LogitechRGBDeviceInfo>, IUnknownDevice //TODO DarthAffe 18.04.2020: It's know which kind of device this is, but they would need to be separated
    {
        #region Properties & Fields

        private readonly LedMapping<LogitechLedId> _ledMapping;

        #endregion

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:RGB.NET.Devices.Logitech.LogitechPerKeyRGBDevice" /> class.
        /// </summary>
        internal LogitechPerKeyRGBDevice(LogitechRGBDeviceInfo info, IUpdateQueue updateQueue, LedMapping<LogitechLedId> ledMapping)
            : base(info, updateQueue)
        {
            this._ledMapping = ledMapping;

            InitializeLayout();
        }

        #endregion

        #region Methods

        private void InitializeLayout()
        {

            //AddLed(LedId.Keyboard_Escape, new Point(0, 0), new Size(10, 10));
            //AddLed(LedId.Keyboard_F1, new Point(10, 10), new Size(10, 10));

            var iCtr = 0;
            foreach (var i in this._ledMapping.LedIds)
            {
                AddLed(i, new Point(iCtr * 10, 0), new Size(10, 10));
                iCtr++;
            }

            //var iCtr = 0;
            //foreach (var i in this._ledMapping.LedIds)
            //{
            //    
            //    AddLed(i, new Point(iCtr, 0), new Size(10, 10));
            //    iCtr++;
            //} 
        }

       // protected virtual void InitializeLayout()
       // {
       //     //_CorsairLedPositions? nativeLedPositions = (_CorsairLedPositions?)Marshal.PtrToStructure(_CUESDK.CorsairGetLedPositionsByDeviceIndex(DeviceInfo.CorsairDeviceIndex), typeof(_CorsairLedPositions));
       //     //if (nativeLedPositions == null) return;
       //     //
       //     //int structSize = Marshal.SizeOf(typeof(_CorsairLedPosition));
       //     //IntPtr ptr = nativeLedPositions.pLedPosition;
       //     //
       //     //for (int i = 0; i < nativeLedPositions.numberOfLed; i++)
       //     //{
       //     //    _CorsairLedPosition? ledPosition = (_CorsairLedPosition?)Marshal.PtrToStructure(ptr, typeof(_CorsairLedPosition));
       //     //    if (ledPosition == null)
       //     //    {
       //     //        ptr = new IntPtr(ptr.ToInt64() + structSize);
       //     //        continue;
       //     //    }
       //     //
       //     //    LedId ledId = Mapping.TryGetValue(ledPosition.LedId, out LedId id) ? id : LedId.Invalid;
       //     //    Rectangle rectangle = ledPosition.ToRectangle();
       //     //    AddLed(ledId, rectangle.Location, rectangle.Size);
       //     //
       //     //    ptr = new IntPtr(ptr.ToInt64() + structSize);
       //     //}
       // }

        #endregion

        #region Methods

        /// <inheritdoc />
        protected override object GetLedCustomData(LedId ledId)
        {
            return _ledMapping.TryGetValue(ledId, out LogitechLedId logitechLedId) ? logitechLedId : -1;
        }

        /// <inheritdoc />
        protected override void UpdateLeds(IEnumerable<Led> ledsToUpdate) => UpdateQueue.SetData(GetUpdateData(ledsToUpdate));

        #endregion
    }
}
