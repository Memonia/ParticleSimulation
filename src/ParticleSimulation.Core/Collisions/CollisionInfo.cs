using ParticleSimulation.Core.Objects;

namespace ParticleSimulation.Core.Collisions
{
	internal readonly record struct CollisionInfo
	(
		double Time,
		Particle Particle,
		ICollidable Collidable
	);
}
