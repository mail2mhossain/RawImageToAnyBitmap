using ReactiveUI;
using Avalonia.Media.Imaging;
using System;
using System.IO;
using SIPSorceryMedia.FFmpeg;
using System.Collections.Generic;
using System.Reactive;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using Avalonia.Controls.Shapes;
using Avalonia.Platform;
using Avalonia.Controls;
using Avalonia;

namespace Avalonia_Test.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static List<Camera>? _cameras;
        private VideoCaptureDevice? _videoSource;
        public string Greeting => "Welcome to Avalonia!";
        private Bitmap? _localVideo;
        public Bitmap? LocalVideo
        {
            get => _localVideo;
            private set => this.RaiseAndSetIfChanged(ref _localVideo, value);
        }
        private bool _isToggled;
        public bool IsToggled
        {
            get => _isToggled;
            set
            {
                if (value == true)
                {
                    Strech = "Fill";
                }
                if (value == false)
                {
                    Strech = "Uniform";
                }
                this.RaiseAndSetIfChanged(ref _isToggled, value);
            }

        }
        private string? _strech;
        public string Strech
        {
            get => _strech;
            set => this.RaiseAndSetIfChanged(ref _strech, value);
        }

		private static OperatingSystemType? OperatingSystem => AvaloniaLocator.Current
			.GetService<IRuntimePlatform>()
			.GetRuntimeInfo().OperatingSystem;

		public ReactiveCommand<Unit, Unit>? ShowVideoCommand { get; set; }
        public ReactiveCommand<Unit, Unit>? CloseVideoCommand { get; set; }
        public MainWindowViewModel ()
        {
            ShowVideoCommand = ReactiveCommand.Create(ShowVideo);
            CloseVideoCommand = ReactiveCommand.Create(CloseVideo);
            InitializeDevices();
            SetVideoSourceDevice();
        }
        private void InitializeDevices ()
        {
			string ffmpegPath = GetFfmpegPath();

            if (Directory.Exists(ffmpegPath))
            {
                FFmpegInit.Initialise(FfmpegLogLevelEnum.AV_LOG_VERBOSE, ffmpegPath);
            }
        }
		private static string GetFfmpegPath ()
		{
			OperatingSystemType osType = AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem;

			if (osType == OperatingSystemType.Linux)
				return "/usr/lib/x86_64-linux-gnu";

			if (osType == OperatingSystemType.OSX)
				return "/usr/local/Cellar/ffmpeg@4/4.4.3_4/lib";

			var dir = AppDomain.CurrentDomain.BaseDirectory;
			var ffmpegWinPath = System.IO.Path.Combine(dir, "ffmpeg");

			return ffmpegWinPath;
		}
		private void SetVideoSourceDevice()
        {
            _cameras = FFmpegCameraManager.GetCameraDevices();
        }
        private void ShowVideo ()
        {
            _videoSource = new VideoCaptureDevice();
            _videoSource.UPDATE_VIDEO_FRAME += _videoSource_UPDATE_VIDEO_FRAME;
            _videoSource.CaptureVideo(_cameras![0].Path);
        }
        private void _videoSource_UPDATE_VIDEO_FRAME (object? sender, UpdateVideoFrameEventArgs e)
        {
			Image<Bgr24> image = ImageConverter.ConvertRawImageToImageSharp(e.RawImage);
			Bitmap avaloniaBitmap = ImageConverter.ConvertImageSharpToAvaloniaBitmap(image);
			LocalVideo = avaloniaBitmap;
		}
        private void CloseVideo ()
        {
            if (_videoSource == null) return;
            _videoSource.UPDATE_VIDEO_FRAME -= _videoSource_UPDATE_VIDEO_FRAME;
            _videoSource.CloseVideo();
            _videoSource.Dispose();
        }
    }
}
