using System;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    [Flags]
    public enum ColorIdentity
    {
        Colorless = 0x00,
        White     = 0x01,
        Blue      = 0x02,
        Black     = 0x04,
        Red       = 0x08,
        Green     = 0x10
    }
}
