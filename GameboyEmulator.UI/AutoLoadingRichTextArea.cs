using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
