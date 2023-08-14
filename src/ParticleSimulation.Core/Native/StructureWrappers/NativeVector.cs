using System.Runtime.InteropServices;

namespace ParticleSimulation.Core.Native.StructureWrappers
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeVector
    {
        public double Vx;
        public double Vy;
        public double Length;
    }
}
