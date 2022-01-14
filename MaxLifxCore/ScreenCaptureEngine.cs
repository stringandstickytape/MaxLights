using DesktopDuplication;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MaxLifxCore
{
    public class ScreenCaptureEngine
    {

        public Dictionary<string, List<Rectangle>> ZoneAreas = new Dictionary<string, List<Rectangle>>();

        private DesktopDuplicator _desktopDuplicator;


        public bool TerminateThread { get; set; }
        public ScreenCaptureEngine(int monitor)
        {
            _desktopDuplicator = new DesktopDuplicator(monitor);

        }

        private Color? previousColour = null;
        private ushort prevx, prevy, prevw, prevh;

        public unsafe Color? GetColour(ushort x, ushort y, ushort width, ushort height)
        {
            // If the frame hasn't become out of date, and the coords are the same as last time, return the same colour as last time
            if(!reset && frame != null && x == prevx && y == prevy && width == prevw && height == prevh)
            {
                return previousColour;
            }
            prevx = x;  prevy = y; prevw = width; prevh = height;
            // user specifies centre of rectangle.  System wants top-left.
            previousColour = getAverageColourForArea(new Rectangle(x-width/2, y-height/2, width, height));
            return previousColour;
        }

        DesktopFrame frame;
        private bool reset = false;
        private unsafe Color? getAverageColourForArea(Rectangle rect, bool getNewFrame = false)
        {
            if (getNewFrame || reset)
            {
                reset = false;
                var nextFrame = _desktopDuplicator.GetLatestFrame();
                if (nextFrame != null)
                    frame = nextFrame;
            }

            Color? all = null;
            
            if (frame != null && frame.DesktopImage != null)
            {


                BitmapData bmd = frame.DesktopImage.LockBits(rect,
                                  System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                  frame.DesktopImage.PixelFormat);

                all = GetColourForRectFromBitmapData(rect, bmd, frame.DesktopImage.PixelFormat);

                frame.DesktopImage.UnlockBits(bmd);
            }

            return all;
        }

        public static unsafe Color? GetColourForRectFromBitmapData(Rectangle rect, BitmapData bmd, PixelFormat pixelFormat)
        {
            Color? all;
            int rTot = 0, bTot = 0, gTot = 0;
            
            int PixelSize = pixelFormat == PixelFormat.Format32bppRgb || pixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;

            unsafe
            {
                for (int y = 0; y < rect.Height; y++)
                {
                    byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);

                    for (int x = 0; x < rect.Width; x++)
                    {
                        bTot += row[x * PixelSize];
                        gTot += row[x * PixelSize + 1];
                        rTot += row[x * PixelSize + 2];
                    }
                }
            }

            int ct = rect.Width * rect.Height;

            all = Color.FromArgb(rTot / ct, gTot / ct, bTot / ct);
            return all;
        }

        internal void ClearFrame()
        {
            reset = true;
        }
    }


}
