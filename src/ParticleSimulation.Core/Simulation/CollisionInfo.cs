namespace ParticleSimulation.Core.Simulation
{
	internal readonly record struct CollisionInfo
	(
		double Time,
		Particle Particle,
		ICollidable Collidable
	);
}
