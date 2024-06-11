using System.Collections.Generic;
using System.Windows.Controls;

using ParticleSimulation.Visual.Objects;

namespace ParticleSimulation.Visual
{
	internal partial class VisualCanvas : Canvas
	{
		public VisualCanvas()
		{
			this.ClipToBounds = true;
		}

		public void Init(List<VisualParticle> vpl)
		{
			this.Children.Clear();
			foreach (var vp in vpl)
			{
				this.Children.Add(vp);
				this.Children.Add(vp.Velocity);
				this.Children.Add(vp.Trajectory);
			}
		}

		public void Update(List<VisualParticle> vpl, VisualisationFlags vf)
		{
			foreach (var vp in vpl)
			{
				this.Children.Remove(vp.Trail);
				this.Children.Remove(vp.Velocity);
				this.Children.Remove(vp.Trajectory);

				if (vf.HasFlag(VisualisationFlags.TrackAll) ||
					(vp.State & VisualParticleState.All) != VisualParticleState.None)
				{
					if (vf.HasFlag(VisualisationFlags.ShowTrail))
					{
						_drawTrail(vp);
					}

					// See outer 'else' comment
					else
					{
						vp.Trail.ClearPointsUI();
					}

					if (vf.HasFlag(VisualisationFlags.ShowTrajectoryLine))
					{
						_drawTrajectoryLine(vp);
					}

					if (vf.HasFlag(VisualisationFlags.ShowVelocityVector))
					{
						_drawVelocityVector(vp);
					}
				}

				else
				{
					// Trail is only updated when it is shown to improve performance.
					// Because of that, when the trailing is off, the points which made up
					// the trail remain inside the 'VisualTrail'. When trailing is on again,
					// the trail consists of old points before being off and the fresh ones,
					// which creates glitchy effect of long "random" lines appearing, but then
					// stablising.
					//
					// Even though it's better to clear the points on trailing off, rather than
					// doing it in a loop for every point, it's easier this way. Particle tracking
					// is triggered in several places by different state bitflags. We could introduce
					// something like 'ChangeState' method  in 'VisualParticle' and detect when to clear
					// the points, but then we still have to take care of the 'TrackAll' case.
					//
					// This solution is acceptable, because clearing points consists of simply setting
					// the inner list count to 0, so the overhead is negligible.
					vp.Trail.ClearPointsUI();
				}

				_updateTracked(vp, vf);
				_drawVisualParticle(vp);
			}
		}

		private void _updateTracked(VisualParticle vp, VisualisationFlags vf)
		{
			var isTracked = vp.State.HasFlag(VisualParticleState.Tracked);
			var isTrackAll = vf.HasFlag(VisualisationFlags.TrackAll);

			if (vp.State.HasFlag(VisualParticleState.Slowest))
			{
				vp.Fill = _slowestParticleColor;
			}
			else if (vp.State.HasFlag(VisualParticleState.Fastest))
			{
				vp.Fill = _fastestParticleColor;
			}
			else if (vf.HasFlag(VisualisationFlags.HideNotTracked) && !isTrackAll && !isTracked)
			{
				vp.Fill = _hiddenParticleColor;
			}
			else if (isTracked || isTrackAll)
			{
				vp.Fill = _trackedParticleColor;
			}
			else
			{
				vp.Fill = _particleColor;
			}
		}

		private void _drawVisualParticle(VisualParticle vp)
		{
			Canvas.SetTop(vp, vp.Particle.Center.Vy);
			Canvas.SetLeft(vp, vp.Particle.Center.Vx);
		}

		private void _drawTrail(VisualParticle vp)
		{
			vp.Trail.AddPointUI(new(vp.Particle.Center.Vx, vp.Particle.Center.Vy));
			vp.Trail.StrokeThickness = _trailThickness;
			vp.Trail.Stroke = _trailColor;
			this.Children.Add(vp.Trail);
		}

		private void _drawVelocityVector(VisualParticle vp)
		{
			vp.Velocity.StrokeThickness = _velocityVectorThickness;
			vp.Velocity.Stroke = _velocityVectorColor;
			this.Children.Add(vp.Velocity);
		}

		private void _drawTrajectoryLine(VisualParticle vp)
		{
			vp.Trajectory.StrokeThickness = _trajectoryLineThickness;
			vp.Trajectory.Stroke = _trajectoryLineColor;
			this.Children.Add(vp.Trajectory);
		}
	}
}
