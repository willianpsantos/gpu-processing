using System.Runtime.InteropServices;

namespace GPU.Placeholders.Processing.Core.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CharHelper
    {
        public fixed byte AllCharsBytes[char.MaxValue];
        public fixed char AllChars[char.MaxValue];

        public CharHelper()
        {
            LoadAllChars();
        }

        public void LoadAllChars()
        {
            for (var i = char.MinValue; i <= char.MaxValue; i++)
            {
                if (char.IsControl(i))
                    continue;

                char c = Convert.ToChar(i);
                AllCharsBytes[i] = (byte)i;
                AllChars[i] = c;
            }
        }
    }
}
