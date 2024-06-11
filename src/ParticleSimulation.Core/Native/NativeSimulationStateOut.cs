using System;
using System.Runtime.InteropServices;

namespace ParticleSimulation.Core.Native
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct NativeSimulationStateOut
	{
		public IntPtr Collisions;
		public IntPtr Particles;
	}
}
