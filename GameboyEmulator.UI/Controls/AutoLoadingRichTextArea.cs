using System.IO;
using Eto.Forms;

namespace GameboyEmulator.UI
{
    public class AutoLoadingRichTextArea : RichTextArea
    {
        public override string Text
        {
            get { return ""; }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(value);
                writer.Flush();
                Buffer.Load(stream, RichTextAreaFormat.Rtf);
            }
        }
    }
}
