using Accord.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace DesktopDuplication.Demo
{
    public partial class FormDemo : Form
    {
        private DesktopDuplicator desktopDuplicator;

        public FormDemo()
        {
            InitializeComponent();

            try
            {
                desktopDuplicator = new DesktopDuplicator(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FormDemo_Shown(object sender, EventArgs e)
        {
            while (true)
            {
                Application.DoEvents();

                DesktopFrame frame = null;
                try
                {
                    frame = desktopDuplicator.GetLatestFrame();
                }
                catch
                {
                    desktopDuplicator = new DesktopDuplicator(0);
                    continue;
                }

                if (frame != null)
                {
                    LabelCursor.Location = frame.CursorLocation;
                    LabelCursor.Visible = frame.CursorVisible;

                    this.BackgroundImage = frame.DesktopImage;

                    Color? all = null;

                    if (frame != null && frame.DesktopImage != null)
                    {
                        var rect = new Rectangle(10, 10, 10, 10);

                        BitmapData bmd = frame.DesktopImage.LockBits(rect,
                                          System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                          frame.DesktopImage.PixelFormat);

                        all = GetColourForRectFromBitmapData(rect, bmd, frame.DesktopImage.PixelFormat);

                        frame.DesktopImage.UnlockBits(bmd);
                    }
                }

               var videoFileReader = new VideoFileReader();

                var filename = @"H:\TV\Curb.Your.Enthusiasm.2000.S11E01.The.Five-Foot.Fence.1080p.HMAX.Webrip.x265.10bit.AC3.5.1.JBENTTAoE.mkv";


                videoFileReader.Open(filename);
            }
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

    }
}
