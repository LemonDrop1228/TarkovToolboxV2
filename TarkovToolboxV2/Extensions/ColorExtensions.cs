using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TarkovToolboxV2.Extensions
{
    public static class ColorExtensions
    {
        public static System.Windows.Media.Color WithAlpha(this System.Drawing.Color color, int newA)
        {
            var aplhColor = System.Drawing.Color.FromArgb(newA, color);
            return System.Windows.Media.Color.FromArgb(aplhColor.A, aplhColor.R, aplhColor.G, aplhColor.B);
        }
    }
}
