using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDFSimSharp
{
    internal static class FormHelpers
    {
        public static void Centerform(System.Windows.Forms.Form form)
        {
            form.Left = ((Screen.PrimaryScreen.Bounds.Width - form.Width) / 2);
            form.Top = ((Screen.PrimaryScreen.Bounds.Height - form.Height) / 2);
        }
    }
}
