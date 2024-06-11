using System;
using System.Runtime.InteropServices;

namespace ParticleSimulation.Core.Native
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct NativeSimulatorInfo
	{
		public IntPtr QueueLength;
	}
}
