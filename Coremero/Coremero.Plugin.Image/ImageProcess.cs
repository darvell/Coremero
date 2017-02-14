using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Utilities;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing.Processors;

namespace Coremero.Plugin.Image
{
    public class ImageProcess : IPlugin
    {

        [Command("contrast", Arguments = "Contrast Value",
            Help = "Increases or decreases the contrast in the attached images. Values between -100 and 100.")]
        public IMessage Contrast(IInvocationContext context, IMessage message)
        {
            int val = int.Parse(message.Text.GetCommandArguments()[0]);
            if (val > 100 || -100 > val)
            {
                throw new InvalidOperationException();
            }

            MemoryStream ms = new MemoryStream();

            using (ImageSharp.Image image = new ImageSharp.Image(message.Attachments[0].Contents))
            {
                image.Contrast(val).Save(ms).Dispose();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }

        [Command("saturation", Arguments = "Saturation Value",
            Help = "Increases or decreases the saturation in the attached images. Values between -100 and 100.")]
        public IMessage Saturate(IInvocationContext context, IMessage message)
        {
            int val = int.Parse(message.Text.GetCommandArguments()[0]);
            if (val > 100 || -100 > val)
            {
                throw new InvalidOperationException();
            }

            MemoryStream ms = new MemoryStream();

            using (ImageSharp.Image image = new ImageSharp.Image(message.Attachments[0].Contents))
            {
                image.Saturation(val).Save(ms).Dispose();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }

        [Command("brightness", Arguments = "Brightness Value",
            Help = "Increases or decreases the brightness in the attached images. Values between -100 and 100.")]
        public IMessage Brightness(IInvocationContext context, IMessage message)
        {
            int val = int.Parse(message.Text.GetCommandArguments()[0]);
            if (val > 100 || -100 > val)
            {
                throw new InvalidOperationException();
            }

            MemoryStream ms = new MemoryStream();

            using (ImageSharp.Image image = new ImageSharp.Image(message.Attachments[0].Contents))
            {
                image.Brightness(val).Save(ms).Dispose();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }

        [Command("pixelate", Arguments = "Pixelate Value",
            Help = "Pixelates the image by a factor of [Pixelate Value]. Values between 0 and 16.")]
        public IMessage Pixelate(IInvocationContext context, IMessage message)
        {
            int val = int.Parse(message.Text.GetCommandArguments()[0]);
            if (val > 16 || 0 > val)
            {
                throw new InvalidOperationException();
            }

            MemoryStream ms = new MemoryStream();

            using (ImageSharp.Image image = new ImageSharp.Image(message.Attachments[0].Contents))
            {
                image.Pixelate(val).Save(ms).Dispose();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }

        [Command("sharpen", Arguments = "Sharpen Value",
            Help = "Sharpens the image by a factor of [Sharpen Value]. Values between 0 and 100.")]
        public IMessage Sharpen(IInvocationContext context, IMessage message)
        {
            int val = int.Parse(message.Text.GetCommandArguments()[0]);
            if (val > 100 || 0 > val)
            {
                throw new InvalidOperationException();
            }

            MemoryStream ms = new MemoryStream();

            using (ImageSharp.Image image = new ImageSharp.Image(message.Attachments[0].Contents))
            {
                image.GaussianSharpen(val).Save(ms).Dispose();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }

        [Command("hue", Arguments = "Hue Degrees",
            Help = "Rotates the image huge by a factor of [Hue Degrees]. Values between -360 and 360.")]
        public IMessage Hue(IInvocationContext context, IMessage message)
        {
            int val = int.Parse(message.Text.GetCommandArguments()[0]);
            if (val > 360 || -360 > val)
            {
                throw new InvalidOperationException();
            }

            MemoryStream ms = new MemoryStream();

            using (ImageSharp.Image image = new ImageSharp.Image(message.Attachments[0].Contents))
            {
                image.Hue(val).Save(ms).Dispose();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }

        [Command("edge", Help = "Detects edges in an image.")]
        public IMessage EdgeDetection(IInvocationContext context, IMessage message)
        {
            MemoryStream ms = new MemoryStream();

            using (ImageSharp.Image image = new ImageSharp.Image(message.Attachments[0].Contents))
            {
                image.DetectEdges().Save(ms).Dispose();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }

        [Command("invert", Help = "Inverts an image.")]
        public IMessage Invert(IInvocationContext context, IMessage message)
        {
            MemoryStream ms = new MemoryStream();

            using (ImageSharp.Image image = new ImageSharp.Image(message.Attachments[0].Contents))
            {
                image.Invert().Save(ms).Dispose();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }

        [Command("glow", Help = "Makes an image glow.")]
        public IMessage Glow(IMessage message)
        {
            MemoryStream ms = new MemoryStream();

            using (ImageSharp.Image image = new ImageSharp.Image(message.Attachments[0].Contents))
            {
                image.Glow().Save(ms).Dispose();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }

        [Command("blend", Help = "Blends two attachments together with a color burn.")]
        public IMessage Blend(IMessage message)
        {
            if (message.Attachments?.Count >= 2)
            {
                int blendAmount = 50;
                int.TryParse(message.Text, out blendAmount);
                MemoryStream ms = new MemoryStream();

                using (ImageSharp.Image imageSource = new ImageSharp.Image(message.Attachments[0].Contents))
                {
                    imageSource.Save(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                }

                foreach (IAttachment attachment in message.Attachments.Skip(1))
                {
                    using (ImageSharp.Image imageSource = new ImageSharp.Image(ms))
                    {
                        using (ImageSharp.Image imageTarget = new ImageSharp.Image(attachment.Contents))
                        {
                            using (var resizedImage = imageTarget.Resize(imageSource.Width,
                                    imageSource.Height))
                            {
                                ms.Dispose();
                                ms = new MemoryStream(); // Reinit the stream. This is also insane.
                                var blendedImage = imageSource.DrawImage(resizedImage, blendAmount, default(Size), default(Point));
                                blendedImage.Save(ms);
                                blendedImage.Dispose();
                            }
                        }
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                }

                // Sneak in to the ArrayPool.
                
                return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
            }
            return null;
        }
    }
}