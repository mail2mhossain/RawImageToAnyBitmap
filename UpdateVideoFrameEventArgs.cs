using IronSoftware.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_Test
{
    public class UpdateVideoFrameEventArgs
    {
        public AnyBitmap Image { get; set; }
        public SIPSorceryMedia.Abstractions.RawImage RawImage { get; set; }
    }
}
