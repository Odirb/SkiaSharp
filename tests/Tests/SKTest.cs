﻿using System;
using System.IO;
using Xunit;

namespace SkiaSharp.Tests
{
	public abstract class SKTest : BaseTest
	{
		protected static void SaveBitmap(SKBitmap bmp, string filename = "output.png")
		{
			using (var bitmap = new SKBitmap(bmp.Width, bmp.Height))
			using (var canvas = new SKCanvas(bitmap))
			{
				canvas.Clear(SKColors.Transparent);
				canvas.DrawBitmap(bmp, 0, 0);
				canvas.Flush();

				using (var stream = File.OpenWrite(Path.Combine(PathToImages, filename)))
				using (var image = SKImage.FromBitmap(bitmap))
				using (var data = image.Encode())
				{
					data.SaveTo(stream);
				}
			}
		}

		protected static SKBitmap CreateTestBitmap(byte alpha = 255)
		{
			var bmp = new SKBitmap(40, 40);
			bmp.Erase(SKColors.Transparent);

			using (var canvas = new SKCanvas(bmp))
			using (var paint = new SKPaint())
			{

				var x = bmp.Width / 2;
				var y = bmp.Height / 2;

				paint.Color = SKColors.Red.WithAlpha(alpha);
				canvas.DrawRect(SKRect.Create(0, 0, x, y), paint);

				paint.Color = SKColors.Green.WithAlpha(alpha);
				canvas.DrawRect(SKRect.Create(x, 0, x, y), paint);

				paint.Color = SKColors.Blue.WithAlpha(alpha);
				canvas.DrawRect(SKRect.Create(0, y, x, y), paint);

				paint.Color = SKColors.Yellow.WithAlpha(alpha);
				canvas.DrawRect(SKRect.Create(x, y, x, y), paint);
			}

			return bmp;
		}

		protected static void ValidateTestBitmap(SKBitmap bmp, byte alpha = 255)
		{
			Assert.NotNull(bmp);
			Assert.Equal(40, bmp.Width);
			Assert.Equal(40, bmp.Height);

			Assert.Equal(SKColors.Red.WithAlpha(alpha), bmp.GetPixel(10, 10));
			Assert.Equal(SKColors.Green.WithAlpha(alpha), bmp.GetPixel(30, 10));
			Assert.Equal(SKColors.Blue.WithAlpha(alpha), bmp.GetPixel(10, 30));
			Assert.Equal(SKColors.Yellow.WithAlpha(alpha), bmp.GetPixel(30, 30));
		}

		protected static void ValidateTestPixmap(SKPixmap pix, byte alpha = 255)
		{
			Assert.NotNull(pix);
			Assert.Equal(40, pix.Width);
			Assert.Equal(40, pix.Height);

			Assert.Equal(SKColors.Red.WithAlpha(alpha), pix.GetPixelColor(10, 10));
			Assert.Equal(SKColors.Green.WithAlpha(alpha), pix.GetPixelColor(30, 10));
			Assert.Equal(SKColors.Blue.WithAlpha(alpha), pix.GetPixelColor(10, 30));
			Assert.Equal(SKColors.Yellow.WithAlpha(alpha), pix.GetPixelColor(30, 30));
		}

		protected GlContext CreateGlContext()
		{
			try
			{
				if (IsLinux)
				{
					return new GlxContext();
				}
				else if (IsMac)
				{
					return new CglContext();
				}
				else if (IsWindows)
				{
					return new WglContext();
				}
				else
				{
					throw new PlatformNotSupportedException();
				}
			}
			catch (Exception ex)
			{
				throw new SkipException("Unable to create GL context: " + ex.Message);
			}
		}
	}
}
