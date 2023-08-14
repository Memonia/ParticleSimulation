using System.Runtime.InteropServices;

namespace ParticleSimulation.Core.Native.StructureWrappers
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSurface
    {
        public NativeVector InBoundsNormal;
        public NativeStraightLine LineEquation;
    }
}
