using ParticleSimulation.Core.Objects;

namespace ParticleSimulation.Core
{
	internal struct SimulationState
    {
		public int Collisions { get; set; }
		public IReadOnlyList<Particle> Particles { get; set; }
	}
}
