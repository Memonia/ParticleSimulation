using System;

namespace ParticleSimulation.Visual.Objects
{
	[Flags]
	internal enum VisualParticleState
	{
		None = 0,
		Tracked = 1,
		Slowest = 2,
		Fastest = 4,
		SlowestAndFastest = Slowest | Fastest,
		All = SlowestAndFastest | Tracked
	}
}
