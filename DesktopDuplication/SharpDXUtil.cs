using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopDuplication
{
    static class SharpDXUtil
    {

        public static int GetWidth(this RawRectangle r)
        {
            return r.Right - r.Left;
        }

        public static int GetHeight(this RawRectangle r)
        {
            return r.Bottom - r.Top;
        }
    }
}
