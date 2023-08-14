using ParticleSimulation.Core.Objects;

namespace ParticleSimulation.Core.Collisions
{
    internal readonly record struct Collidables
    (
        IReadOnlyList<Surface> Surfaces,
        IReadOnlyList<Particle> Particles
    );
}
