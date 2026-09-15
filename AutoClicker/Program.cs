using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AutoClicker
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    internal sealed class MainForm : Form
    {
        private const int HotkeyId = 1;
        private const int WmHotkey = 0x0312;
        private const uint VkF6 = 0x75;
        private const uint InputMouse = 0;
        private const uint MouseeventfLeftdown = 0x0002;
        private const uint MouseeventfLeftup = 0x0004;

        private readonly NumericUpDown intervalInput;
        private readonly Button toggleButton;
        private readonly Label statusValue;
        private System.Threading.Timer clickTimer;
        private bool running;

        public MainForm()
        {
            Text = "Auto Clicker";
            ClientSize = new Size(310, 145);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = true;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);

            var intervalLabel = new Label
            {
                AutoSize = true,
                Location = new Point(18, 21),
                Text = "Interval (ms):"
            };

            intervalInput = new NumericUpDown
            {
                Location = new Point(122, 18),
                Size = new Size(165, 23),
                Minimum = 1,
                Maximum = 3600000,
                Value = 100,
                ThousandsSeparator = true
            };

            toggleButton = new Button
            {
                Location = new Point(18, 57),
                Size = new Size(269, 34),
                Text = "Start (F6)"
            };
            toggleButton.Click += delegate { ToggleClicking(); };

            var statusLabel = new Label
            {
                AutoSize = true,
                Location = new Point(18, 110),
                Text = "Status:"
            };

            statusValue = new Label
            {
                AutoSize = true,
                Location = new Point(72, 110),
                Text = "Stopped",
                ForeColor = Color.Firebrick
            };

            Controls.Add(intervalLabel);
            Controls.Add(intervalInput);
            Controls.Add(toggleButton);
            Controls.Add(statusLabel);
            Controls.Add(statusValue);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!RegisterHotKey(Handle, HotkeyId, 0, VkF6))
            {
                MessageBox.Show(this, "F6 is already in use by another application.",
                    "Auto Clicker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            StopClicking();
            UnregisterHotKey(Handle, HotkeyId);
            base.OnHandleDestroyed(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmHotkey && m.WParam.ToInt32() == HotkeyId)
            {
                ToggleClicking();
            }
            base.WndProc(ref m);
        }

        private void ToggleClicking()
        {
            if (running)
            {
                StopClicking();
            }
            else
            {
                StartClicking();
            }
        }

        private void StartClicking()
        {
            int interval = Decimal.ToInt32(intervalInput.Value);
            clickTimer = new System.Threading.Timer(ClickOnce, null, 0, interval);
            running = true;
            intervalInput.Enabled = false;
            toggleButton.Text = "Stop (F6)";
            statusValue.Text = "Running";
            statusValue.ForeColor = Color.ForestGreen;
        }

        private void StopClicking()
        {
            System.Threading.Timer timer = clickTimer;
            clickTimer = null;
            if (timer != null)
            {
                timer.Dispose();
            }
            running = false;

            if (!IsDisposed)
            {
                intervalInput.Enabled = true;
                toggleButton.Text = "Start (F6)";
                statusValue.Text = "Stopped";
                statusValue.ForeColor = Color.Firebrick;
            }
        }

        private static void ClickOnce(object state)
        {
            INPUT[] inputs = new INPUT[2];
            inputs[0].type = InputMouse;
            inputs[0].mi.dwFlags = MouseeventfLeftdown;
            inputs[1].type = InputMouse;
            inputs[1].mi.dwFlags = MouseeventfLeftup;
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    }
}
