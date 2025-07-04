using System.Runtime.InteropServices;

namespace MyKUIPi.Input;

public class TouchReader : InputDeviceReader<TouchReader.InputEvent>
{
    private const ushort EV_ABS = 0x03;
    private const ushort EV_KEY = 0x01;
    private const ushort EV_SYN = 0x00;

    private const ushort ABS_X = 0x00;
    private const ushort ABS_Y = 0x01;
    private const ushort ABS_MT_POSITION_X = 0x35;
    private const ushort ABS_MT_POSITION_Y = 0x36;

    private const ushort BTN_TOUCH = 0x14a;

    public int TouchX { get; private set; }
    public int TouchY { get; private set; }
    public bool IsTouching { get; private set; }

    [StructLayout(LayoutKind.Sequential)]
    public struct InputEvent
    {
        public TimeVal Time;
        public ushort Type;
        public ushort Code;
        public int Value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TimeVal
    {
        public long TvSec;
        public long TvUsec;
    }

    public TouchReader(string devicePath) : base(devicePath)
    {
    }

    protected override void OnInputEvent(InputEvent inputEvent)
    {
        lock (LockObject)
        {
            switch (inputEvent.Type)
            {
                case EV_ABS:
                    if (inputEvent.Code == ABS_X || inputEvent.Code == ABS_MT_POSITION_X)
                        TouchX = inputEvent.Value;
                    else if (inputEvent.Code == ABS_Y || inputEvent.Code == ABS_MT_POSITION_Y)
                        TouchY = inputEvent.Value;
                    break;
                case EV_KEY:
                    if (inputEvent.Code == BTN_TOUCH)
                        IsTouching = inputEvent.Value != 0;
                    break;
                case EV_SYN:
                    // Touch frame complete
                    break;
            }
        }
    }

    public (float normX, float normY, bool isTouching) GetTouchState()
    {
        if (MyEngine.Instance is null)
        {
            throw new NullReferenceException("MyEngine Instance is null.");
        }

        lock (LockObject)
        {
            var engine = MyEngine.Instance;

            // Prevent divide-by-zero in case of calibration error
            float rangeX = engine.MaxTouchX - engine.MinTouchX;
            float rangeY = engine.MaxTouchY - engine.MinTouchY;

            if (rangeX <= 0 || rangeY <= 0)
            {
                return (0f, 0f, IsTouching); // fallback if calibration is invalid
            }

            float normX = Math.Clamp((float)(TouchX - engine.MinTouchX) / rangeX, 0f, 1f);
            float normY = Math.Clamp((float)(TouchY - engine.MinTouchY) / rangeY, 0f, 1f);

            return (normX, normY, IsTouching);
        }
    }


    public (float x, float y, bool isTouching) GetAbsTouchState()
    {
        lock (LockObject)
        {
            return (TouchX, TouchY, IsTouching);
        }
    }
}