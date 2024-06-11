using ParticleSimulation.Core.Simulation;

namespace ParticleSimulation.Core.Abstractions
{
	public interface IParticle
	{
		double R { get; }

		Vector Center { get; }

		Vector Velocity { get; }

		StraightLine TrajectoryLine { get; }
	}
}
