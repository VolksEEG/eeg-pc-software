// <copyright file="frmMain.cs" company="VolksEEG Project">
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </copyright>

namespace Simple_Packet_Receiver
{
    using System.Windows.Forms;

    internal static class FormHelpers
    {
        public static void Centerform(System.Windows.Forms.Form form)
        {
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
        }
    }
}
