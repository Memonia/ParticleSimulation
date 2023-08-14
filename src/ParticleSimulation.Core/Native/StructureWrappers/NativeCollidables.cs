using System.Runtime.InteropServices;

namespace ParticleSimulation.Core.Native.StructureWrappers
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct NativeCollidables
    {
        public IntPtr Surfaces;
        public int SurfaceCount;
        public IntPtr Particles;
        public int ParticleCount;

        public NativeSurface* SurfacesPtr => (NativeSurface*)Surfaces;
        public NativeParticle* ParticlesPtr => (NativeParticle*)Particles;
    }
}
