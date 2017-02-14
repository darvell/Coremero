using System;
using System.IO;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Utilities;
using ImageSharp;
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
                image.Contrast(val).Save(ms);
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
                image.Saturation(val).Save(ms);
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
                image.Brightness(val).Save(ms);
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
                image.Pixelate(val).Save(ms);
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
                image.GaussianSharpen(val).Save(ms);
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
                image.Hue(val).Save(ms);
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
                image.DetectEdges().Save(ms);
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
                image.Invert().Save(ms);
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }
    }
}