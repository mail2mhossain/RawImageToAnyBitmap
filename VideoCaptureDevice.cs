using IronSoftware.Drawing;
using SIPSorceryMedia.FFmpeg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_Test
{
    public class VideoCaptureDevice : IDisposable
    {
        public event EventHandler<UpdateVideoFrameEventArgs> UPDATE_VIDEO_FRAME;
        private FFmpegCameraSource _videoSource;

        public void CaptureVideo (string camPath)
        {
            _videoSource = new FFmpegCameraSource(camPath);
            _videoSource.OnVideoSourceRawSampleFaster += _videoSource_OnVideoSourceRawSampleFaster;
            _videoSource.StartVideo().Wait();
        }
        private void _videoSource_OnVideoSourceRawSampleFaster (uint durationMilliseconds, SIPSorceryMedia.Abstractions.RawImage rawImage)
        {
            AnyBitmap bmpImage = new System.Drawing.Bitmap(rawImage.Width, rawImage.Height, rawImage.Stride, System.Drawing.Imaging.PixelFormat.Format24bppRgb, rawImage.Sample);

            //byte[] managedArray = new byte[rawImage.Height * rawImage.Stride];
            //Marshal.Copy(rawImage.Sample, managedArray, 0, managedArray.Length);
            //AnyBitmap bmpImage = AnyBitmap.FromBytes(managedArray);
            FireUpdateVideoControlEvent(bmpImage, rawImage);
        }
        protected void FireUpdateVideoControlEvent (AnyBitmap image, SIPSorceryMedia.Abstractions.RawImage rawImage)
        {
            UpdateVideoFrameEventArgs args = new UpdateVideoFrameEventArgs();
            args.Image = image;
            args.RawImage = rawImage;

            EventHandler<UpdateVideoFrameEventArgs> handler = UPDATE_VIDEO_FRAME;
            if (handler != null)
            {
                handler.Invoke(this, args);
            }
        }
        public void CloseVideo ()
        {
            if (_videoSource == null) return;
            _videoSource.OnVideoSourceRawSampleFaster -= _videoSource_OnVideoSourceRawSampleFaster;
            _videoSource.Close();
            _videoSource.Dispose();
        }
        public void Dispose ()
        {
            CloseVideo();
        }
    }
}
