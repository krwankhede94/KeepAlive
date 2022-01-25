using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardAlive
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInput
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInput
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInput
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)] public MouseInput mi;
        [FieldOffset(0)] public KeyboardInput ki;
        [FieldOffset(0)] public HardwareInput hi;
    }

    public struct Input
    {
        public int type;
        public InputUnion u;
    }

    [Flags]
    public enum KeyEventF
    {
        KeyDown = 0x0000,
        ExtendedKey = 0x0001,
        KeyUp = 0x0002,
        Unicode = 0x0004,
        Scancode = 0x0008
    }

    [Flags]
    public enum MouseEventF
    {
        Absolute = 0x8000,
        HWheel = 0x01000,
        Move = 0x0001,
        MoveNoCoalesce = 0x2000,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        VirtualDesk = 0x4000,
        Wheel = 0x0800,
        XDown = 0x0080,
        XUp = 0x0100
    }

    [Flags]
    public enum InputType
    {
        Mouse = 0,
        Keyboard = 1,
        Hardware = 2
    }

    public partial class Form1 : Form
    {
        private ushort i = 0;

        private bool IsKey = true;

        private readonly Timer t = new Timer();

        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        private void T_Tick(object sender, EventArgs e)
        {
            PressKey(0x11);
            PressKey(0x0e);
            ContentTextbox.Text = "Running...(Refreshed at: " + DateTime.Now + ")";
        }


        private void PressKey(ushort key)
        {
            Input[] inputs =
            {
                new Input
                {
                    type = (int)InputType.Keyboard,
                    u = new InputUnion
                    {
                        ki = new KeyboardInput
                        {
                            wVk = 0,

                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                },
                new Input
                {
                    type = (int)InputType.Keyboard,
                    u = new InputUnion
                    {
                        ki = new KeyboardInput
                        {
                            wVk = 0,
                            wScan = key,
                            dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                }
            };

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartAlive();
        }

        private void StartAlive()
        {
            if (t.Enabled) t.Stop();

            t.Interval = int.Parse(durationTextBox.Text) * 1000;
            t.Tick += T_Tick;
            ContentTextbox.Focus();
            ContentTextbox.BackColor = Color.Aquamarine;
            ContentTextbox.Text = "Running...";
            t.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StopAlive();
        }

        private void StopAlive()
        {
            t.Tick -= T_Tick;
            t.Stop();
            ContentTextbox.BackColor = Color.Coral;
            ContentTextbox.Text = "Stopped!";
        }


        private void ContentTextBox_Enter(object sender, EventArgs e)
        {
            StartAlive();
        }

        private void ContentTextBox_Leave(object sender, EventArgs e)
        {
            StopAlive();
        }


        private void Form1_Activated(object sender, EventArgs e)
        {
            StartAlive();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            StopAlive();
        }
    }
}