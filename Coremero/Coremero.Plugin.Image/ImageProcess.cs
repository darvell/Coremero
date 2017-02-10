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
        [Command("contrast","Contrast Value", Help = "Increases or decreases the contrast in the attached images. Values between -100 and 100.")]
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
            return Message.Create(null, new StreamAttachment(ms, message.Attachments[0].Name));
        }
    }
}
