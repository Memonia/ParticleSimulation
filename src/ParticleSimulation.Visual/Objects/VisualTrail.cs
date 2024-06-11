using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using ParticleSimulation.Core.Abstractions;

namespace ParticleSimulation.Visual.Objects
{
	internal class VisualTrail : Shape
	{
		private readonly int _trailSegmentCount = 16;

		private readonly PathFigure _pathFigure;
		private readonly PathGeometry _pathGeometry;

		public VisualTrail(IParticle particle)
		{
			_pathFigure = new() { Segments = new() };
			_pathGeometry = new PathGeometry { Figures = { _pathFigure } };
		}

		public void ClearPointsUI() => _pathFigure.Segments.Clear();

		public void AddPointUI(Point point)
		{
			/* Meant to be called from a UI thread, since it's not too computationally expensive */

			if (_pathFigure.Segments.Count == _trailSegmentCount)
			{
				_pathFigure.Segments.RemoveAt(0);
			}

			_pathFigure.Segments.Add(new LineSegment(point, true));
			_pathFigure.StartPoint = ((LineSegment)_pathFigure.Segments[0]).Point;
		}

		protected override Geometry DefiningGeometry => _pathGeometry;
	}
}
