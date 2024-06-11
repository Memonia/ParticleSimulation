using System.Collections.Generic;

namespace ParticleSimulation.Core.Simulation
{
	internal readonly record struct Collidables
	(
		IReadOnlyList<Surface> Surfaces,
		IReadOnlyList<Particle> Particles
	);
}
