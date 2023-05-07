using SIPSorceryMedia.FFmpeg;
using System;

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
            FireUpdateVideoControlEvent(rawImage);
        }
        protected void FireUpdateVideoControlEvent (SIPSorceryMedia.Abstractions.RawImage rawImage)
        {
            UpdateVideoFrameEventArgs args = new UpdateVideoFrameEventArgs();
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
