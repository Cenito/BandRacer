using BandRacer.Plugin;
using UnityEngine;
using System.Collections;

namespace BandRacer.Platform
{
    public sealed class Platform : PlatformBase
    {
        public static void InitializeStatic()
        {
            Current = new Platform();
        }
    }
}