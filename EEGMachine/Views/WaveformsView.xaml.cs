using System.Windows.Controls;
using System.IO.Ports;
using System.Linq;
using System.Windows;

namespace EEGMachine.Views
{
    /// <summary>
    /// Interaction logic for WaveformsView.xaml
    /// </summary>
    public partial class WaveformsView : UserControl
    {
        public WaveformsView()
        {
            InitializeComponent();

            string[] ports = SerialPort.GetPortNames();

            CBB_PortSelection.Items.Clear();

            foreach (string port in ports.OrderByDescending(s => s))
            {
                CBB_PortSelection.Items.Add(port);
            }

            if (0 == CBB_PortSelection.Items.Count)
            {
                MessageBox.Show("No Serial ports detected!");
            }
        }
    }
}
