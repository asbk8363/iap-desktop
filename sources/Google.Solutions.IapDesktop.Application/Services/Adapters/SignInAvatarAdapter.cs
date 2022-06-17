using Google.Solutions.Common.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Google.Solutions.IapDesktop.Application.Services.Adapters
{
    public interface ISignInAvatarAdapter
    {
        /// <summary>
        /// Download avatar and resize it. Can return null.
        /// </summary>
        Task<Image> TryGetAvatarAsync(
            UserInfo userInfo,
            Size size,
            CancellationToken cancellationToken);
    }

    public class SignInAvatarAdapter : ISignInAvatarAdapter
    {
        public async Task<Image> TryGetAvatarAsync(
            UserInfo userInfo,
            CancellationToken cancellationToken)
        {
            using (ApplicationTraceSources.Default.TraceMethod().WithParameters(userInfo?.Picture))
            {
                if (string.IsNullOrEmpty(userInfo.Picture))
                {
                    return null;
                }

                try
                {
                    using (var client = new HttpClient())
                    using (var response = await client
                        .GetAsync(new Uri(userInfo.Picture), cancellationToken)
                        .ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();

                        using (var stream = await response.Content
                            .ReadAsStreamAsync()
                            .ConfigureAwait(false))
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public async Task<Image> TryGetAvatarAsync(
            UserInfo userInfo,
            Size size,
            CancellationToken cancellationToken)
        {
            using (var avatar = await TryGetAvatarAsync(userInfo, cancellationToken)
                .ConfigureAwait(false))
            {
                if (avatar == null)
                {
                    return null;
                }

                //
                // Resize while maintaining decent quality, see
                // https://stackoverflow.com/a/24199315/4372.
                //
                var scaledAvatar = new Bitmap(size.Width, size.Height);

                scaledAvatar.SetResolution(
                    avatar.HorizontalResolution, 
                    avatar.VerticalResolution);

                using (var graphics = Graphics.FromImage(scaledAvatar))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(
                            avatar,
                            new Rectangle(new Point(0, 0), size),
                            0, 
                            0, 
                            avatar.Width, 
                            avatar.Height, 
                            GraphicsUnit.Pixel, 
                            wrapMode);
                    }
                }

                return scaledAvatar;
            }
        }
    }
}
