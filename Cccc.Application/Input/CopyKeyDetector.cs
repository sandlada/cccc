namespace Cccc.Application
{
    public sealed class CopyKeyDetector
    {
        private bool cDown;

        public bool Feed(int key, bool down, bool control, bool altOrShiftOrWin)
        {
            if (key != 0x43) return false;
            if (!down) { cDown = false; return false; }
            if (cDown) return false;
            cDown = true;
            return control && !altOrShiftOrWin;
        }

        public void Reset() => cDown = false;
    }
}
