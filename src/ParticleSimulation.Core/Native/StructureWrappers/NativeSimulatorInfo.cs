using System.Runtime.InteropServices;

namespace ParticleSimulation.Core.Native.StructureWrappers
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSimulatorInfo
    {
        public IntPtr QueueLength;
    }
}
