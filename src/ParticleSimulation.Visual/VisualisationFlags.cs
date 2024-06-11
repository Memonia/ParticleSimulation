using System;

namespace ParticleSimulation.Visual
{
	[Flags]
	internal enum VisualisationFlags
	{
		TrackAll = 1,
		HideNotTracked = 2,
		ShowVelocityVector = 4,
		ShowTrajectoryLine = 8,
		ShowSlowest = 16,
		ShowFastest = 32,
		ShowTrail = 64,
		ForwardSimulation = 128,
	}
}
