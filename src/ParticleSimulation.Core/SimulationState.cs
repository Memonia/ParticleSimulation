using System.Collections.Generic;

using ParticleSimulation.Core.Simulation;

namespace ParticleSimulation.Core
{
	internal struct SimulationState
	{
		public int Collisions { get; set; }
		public IReadOnlyList<Particle> Particles { get; set; }
	}
}
