using ParticleSimulation.Utils.Math;

namespace ParticleSimulation.Core.Interface
{
    public interface IParticle
    {
        double R { get; }

        Vector Center { get; }

        Vector Velocity { get; }

        StraightLine TrajectoryLine { get; }
    }
}
