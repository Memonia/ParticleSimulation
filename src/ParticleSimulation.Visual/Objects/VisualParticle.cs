using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using ParticleSimulation.Core.Abstractions;

namespace ParticleSimulation.Visual.Objects
{
	internal class VisualParticle : Shape
	{
		public IParticle Particle { get; init; }
		public VisualTrail Trail { get; init; }
		public VisualVelocity Velocity { get; init; }
		public VisualTrajectory Trajectory { get; init; }

		public VisualParticleState State { get; set; }

		private readonly EllipseGeometry _ellipseGeometry;

		public VisualParticle(IParticle particle)
		{
			Particle = particle;
			Trail = new VisualTrail(particle);
			Velocity = new VisualVelocity();
			Trajectory = new VisualTrajectory();

			_ellipseGeometry =
				new() { RadiusX = particle.R, RadiusY = particle.R, };

			// Flip the 'Tracked' flag
			MouseDown += (_, _) => State ^= VisualParticleState.Tracked;
			Cursor = Cursors.Hand;
		}

		protected override Geometry DefiningGeometry => _ellipseGeometry;
	}
}
