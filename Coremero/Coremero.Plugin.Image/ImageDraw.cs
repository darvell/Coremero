using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Coremero.Attachments;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Utilities;
using ImageSharp;
using ImageSharp.Drawing;
using SixLabors.Fonts;

namespace Coremero.Plugin.Image
{
    public class ImageDraw : IPlugin
    {
        private FontCollection _fontCollection = new FontCollection();
        private Font _impactOutline;
        private Font _impactNormal;
        public ImageDraw()
        {
            _fontCollection.Install(Path.Combine(PathExtensions.ResourceDir, "impact.ttf"));
            _impactOutline = new Font(_fontCollection.Families.First(), 40);
            _impactNormal = new Font(_fontCollection.Families.First(), 36);
        }

        [Command("meme")]
        public IMessage ImageMacroText(IMessage message)
        {
            MemoryStream ms = new MemoryStream();

            using (var image = ImageSharp.Image.Load(message.Attachments[0].Contents))
            {
                var bottomVector = new Vector2(image.Width / 2.0f, image.Height);
                image.DrawText(message.Text.TrimCommand(), _impactOutline, Rgba32.Black, bottomVector);
                image.DrawText(message.Text.TrimCommand(), _impactNormal, Rgba32.White, bottomVector);
            }
            ms.Seek(0, SeekOrigin.Begin);
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }
    }
}
