using Avalonia.Media.Imaging;
using SkiaSharp;
using System.Runtime.InteropServices;
using System;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.IO;
using Avalonia;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Processing;

namespace Avalonia_Test
{
	public class ImageConverter
	{
		public static SKBitmap ConvertRawImageToSKBitmap (SIPSorceryMedia.Abstractions.RawImage rawImage)
		{
			// Create an SKImageInfo object with the correct width, height, and color type
			var imageInfo = new SKImageInfo(rawImage.Width, rawImage.Height, SKColorType.Bgra8888);

			// Get the raw image buffer
			byte[] buffer = rawImage.GetBuffer();

			// Convert the buffer from BGR to BGRA by adding an alpha channel
			byte[] bgraBuffer = new byte[rawImage.Height * rawImage.Width * 4];
			for (int i = 0, j = 0; i < buffer.Length; i += 3, j += 4)
			{
				bgraBuffer[j] = buffer[i];
				bgraBuffer[j + 1] = buffer[i + 1];
				bgraBuffer[j + 2] = buffer[i + 2];
				bgraBuffer[j + 3] = 255; // Set alpha channel to 255 (opaque)
			}

			// Create a new SKBitmap using the image info
			var skBitmap = new SKBitmap(imageInfo);

			// Allocate an unmanaged memory block for the BGRA buffer
			IntPtr bgraBufferPtr = Marshal.AllocHGlobal(bgraBuffer.Length);

			// Copy the BGRA buffer to the unmanaged memory block
			Marshal.Copy(bgraBuffer, 0, bgraBufferPtr, bgraBuffer.Length);

			// Set the SKBitmap's pixel data to the unmanaged memory block
			skBitmap.InstallPixels(imageInfo, bgraBufferPtr, imageInfo.RowBytes, (addr, ctx) => {
				// Free the unmanaged memory block when the SKBitmap no longer needs it
				Marshal.FreeHGlobal(addr);
			}, null);

			return skBitmap;
		}

		public static unsafe Image<Bgr24> ConvertRawImageToImageSharp (SIPSorceryMedia.Abstractions.RawImage rawImage)
		{
			int expectedStride = rawImage.Width * 3; // 3 bytes per pixel for Bgr24 format
			if (rawImage.Stride != expectedStride)
			{
				throw new InvalidOperationException("Stride of the raw image does not match the expected stride for ImageSharp Bgr24 format.");
			}

			// Create a new memory span from the rawImage Sample pointer
			var pixelDataSpan = new Span<byte>(rawImage.Sample.ToPointer(), rawImage.Height * rawImage.Stride);

			// Load the pixel data from the span
			var image = Image.LoadPixelData<Bgr24>(pixelDataSpan, rawImage.Width, rawImage.Height);
			image.Mutate(x => x.Flip(FlipMode.Horizontal));

			return image;
		}
		public static Bitmap ConvertImageSharpToAvaloniaBitmap (Image<Bgr24> image)
		{
			// Create a new WriteableBitmap with the same dimensions as the ImageSharp image
			var writeableBitmap = new WriteableBitmap(new PixelSize(image.Width, image.Height), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Opaque);

			// Lock the WriteableBitmap's pixel buffer for direct access
			using (var buffer = writeableBitmap.Lock())
			{
				// Copy the pixel data from the ImageSharp image to the WriteableBitmap
				for (int y = 0; y < image.Height; y++)
				{
					Span<Bgr24> pixelRowSpan = image.GetPixelRowSpan(y);
					IntPtr pixelRowPtr = buffer.Address + y * buffer.RowBytes;

					for (int x = 0; x < pixelRowSpan.Length; x++)
					{
						Bgr24 pixel = pixelRowSpan[x];
						byte[] rgbaPixel = { pixel.R, pixel.G, pixel.B, 255 };
						Marshal.Copy(rgbaPixel, 0, pixelRowPtr + x * 4, 4);
					}
				}
			}

			return writeableBitmap;
		}
		private static void SwapRedAndBlueChannels (Image<Bgr24> image)
		{
			var memoryGroup = image.GetPixelMemoryGroup();
			for (int y = 0; y < image.Height; y++)
			{
				Span<Bgr24> pixelRowSpan = memoryGroup[0].Slice(y * image.Width, image.Width).Span;
				for (int x = 0; x < image.Width; x++)
				{
					Bgr24 pixel = pixelRowSpan[x];
					byte temp = pixel.R;
					pixel.R = pixel.B;
					pixel.B = temp;
					pixelRowSpan[x] = pixel;
				}
			}
		}

		public static Bitmap ConvertImageSharpToAvaloniaBitmap_Slow (Image<Bgr24> image)
		{
			using (var memoryStream = new MemoryStream())
			{
				image.Save(memoryStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
				memoryStream.Seek(0, SeekOrigin.Begin);
				return new Bitmap(memoryStream);
			}
		}
	}
}
