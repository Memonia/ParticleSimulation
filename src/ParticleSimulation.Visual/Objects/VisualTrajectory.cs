using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ParticleSimulation.Visual.Objects
{
	internal class VisualTrajectory : Shape
	{
		public Point P1 { get; set; }
		public Point P2 { get; set; }

		protected override Geometry DefiningGeometry => new LineGeometry(P1, P2);
	}
}
