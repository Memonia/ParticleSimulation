﻿using System.Runtime.InteropServices;

namespace ParticleSimulation.Core.Native
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct NativeParticle
	{
		public NativeVector Velocity;
		public NativeVector Center;
		public double R;
		public double M;
	}
}
