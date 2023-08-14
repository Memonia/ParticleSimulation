using System.Runtime.InteropServices;

namespace ParticleSimulation.Core.Native.StructureWrappers
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeCollisionInfo
    {
        // inf if no collision happened
        public double Time;

        // [0, particles.Count - 1]
        public int ParticleIndex;

        // [0, particles.Count - 1] 
        // or [0, surfaces.Count - 1]
        public int CollidableIndex;
    }
}
