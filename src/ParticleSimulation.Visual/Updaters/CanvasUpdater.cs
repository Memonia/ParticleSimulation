using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using ParticleSimulation.Core.Abstractions;
using ParticleSimulation.Core.Simulation;
using ParticleSimulation.Visual.Objects;

using WindowsPoint = System.Windows.Point;
using WindowsVector = System.Windows.Vector;

namespace ParticleSimulation.Visual.Updaters
{
	internal class CanvasUpdater
	{
		/* Runs tasks which are too expensive to do in a UI thread.
		*
		* Runs in a different thread, so the WPF objects are not updated directly.
		* Instead, it updates non-WPF parts of VisualObjects, which then are used
		* to construct Geometries to be drawn by 'VisualCanvas'.
		*/

		public InfoBlockUpdater? InfoBlockUpdater { get; set; }

		public VisualisationFlags Flags { get; set; }

		private VisualisationFlags _lockedFlags;

		private bool _isInitialised = false;

		// Accessed from outside and inner update thread
		private volatile bool _isStopped = true;

		private Thread _updateThread;
		private readonly double _updateInterval;

		private readonly ISimulator _simulator;
		private readonly VisualCanvas _canvas;

		// Updater works outside of UI thread, so it can't create visual
		// elements directly, so we pass the info needed to create those
		// elements to canvas
		private readonly List<VisualParticle> _visualParticles;

		public CanvasUpdater(VisualCanvas canvas, ISimulator simulator, double updateInterval)
		{
			_updateThread = new(() => { });
			_updateInterval = updateInterval;
			_simulator = simulator;
			_canvas = canvas;
			_visualParticles = new();

			Debug.WriteLine("CanvasUpdater.ctor");
			Debug.IndentLevel = 1;
			Debug.WriteLine($"_updateInterval: {_updateInterval}");
			Debug.IndentLevel = 0;
		}

		public void Init()
		{
			Debug.WriteLine("CanvasUpdater.Init");

			_visualParticles.Clear();
			foreach (var p in _simulator.Particles)
			{
				_visualParticles.Add(new VisualParticle(p));
			}

			_canvas.Init(_visualParticles);
			_canvas.Update(_visualParticles, _lockedFlags);
			_isInitialised = true;
		}

		public void StartUpdating(CancellationToken token)
		{
			void _updateLoop(CancellationToken token)
			{
				while (true)
				{
					if (token.IsCancellationRequested)
					{
						_isStopped = true;
						break;
					}

					var before = Stopwatch.GetTimestamp();

					// Construct UI elements
					_update(token);

					try
					{
						// UI thread is activated only after everything has been precalculated.
						// Task is scheduled with a background priority, so the UI doesn't freeze
						// completely in the face of a huge amount of items to be processed
						_canvas.Dispatcher.Invoke(
							() => _canvas.Update(_visualParticles, _lockedFlags),
							DispatcherPriority.Background,
							token
						);

						// Update info block
						_canvas.Dispatcher.Invoke(
							() => InfoBlockUpdater?.Update(
									_simulator.CurrentFrameInfo.Collisions,
									_simulator.QueueLength
								),
							DispatcherPriority.Background,
							token
						);
					}

					catch (TaskCanceledException) { continue; }

					var elapsed = TimeSpan.FromTicks(Stopwatch.GetTimestamp() - before)
						.TotalMilliseconds;

					// If the canvas was updated faster than requested frame rate, wait for the full
					// time of a frame to elapse, otherwise start the next update immediately
					if (_updateInterval - elapsed > 0)
					{
						SpinWait.SpinUntil(() => false, (int)(_updateInterval - elapsed));
					}
				}
			}

			Debug.WriteLine("CanvasUpdater.StartUpdating");

			if (!_isInitialised)
			{
				throw new InvalidOperationException(
					"Can't start updating before 'Init' is called"
				);
			}

			if (!_isStopped)
			{
				throw new InvalidOperationException(
					"Updating has been already started"
				);
			}

			_isStopped = false;
			_updateThread = new Thread(() => _updateLoop(token));
			_updateThread.Start();
		}

		public void Join()
		{
			Debug.WriteLine("CanvasUpdater.Join");
			_updateThread.Join();
		}

		private void _lockFlags()
		{
			// Lock the flags so they aren't changed by the user during update
			_lockedFlags = Flags;
		}

		private void _update(CancellationToken token)
		{
			void _updateParticle(VisualParticle vp, VisualParticleState includeFlags)
			{
				vp.State |= includeFlags;

				if (_lockedFlags.HasFlag(VisualisationFlags.ShowTrajectoryLine))
				{
					_updateTrajectoryLine(vp);
				}

				if (_lockedFlags.HasFlag(VisualisationFlags.ShowVelocityVector))
				{
					_updateVelocityVector(vp);
				}
			}

			// The flags might be changed in the middle of the long-running calculation.
			// When updated particles are passed to the canvas, the canvas might not expect
			// certain objects to not be there, while the flags suggest otherwise.
			//
			// In order to avoid any weird artefacs of that, the settings are locked and
			// passed to the canvas explicitly

			_lockFlags();

			if (_lockedFlags.HasFlag(VisualisationFlags.ForwardSimulation))
			{
				_simulator.Next();
			}

			var min = _visualParticles[0];
			var max = _visualParticles[0];
			foreach (var vp in _visualParticles)
			{
				if (token.IsCancellationRequested)
				{
					return;
				}

				// Reset 'Slowest' and 'Fastest' flags
				vp.State &= ~VisualParticleState.SlowestAndFastest;

				if (min.Particle.Velocity.Length > vp.Particle.Velocity.Length)
				{
					min = vp;
				}
				else if (max.Particle.Velocity.Length < vp.Particle.Velocity.Length)
				{
					max = vp;
				}

				if (vp.State.HasFlag(VisualParticleState.Tracked) ||
					_lockedFlags.HasFlag(VisualisationFlags.TrackAll))
				{
					_updateParticle(vp, VisualParticleState.None);
				}
			}

			if (_lockedFlags.HasFlag(VisualisationFlags.ShowSlowest))
			{
				_updateParticle(min, VisualParticleState.Slowest);
			}

			if (_lockedFlags.HasFlag(VisualisationFlags.ShowFastest))
			{
				_updateParticle(max, VisualParticleState.Fastest);
			}
		}

		private void _updateVelocityVector(VisualParticle vp)
		{
			var p = vp.Particle;
			var normal = p.Velocity.Normal();

			WindowsVector velocity = new(p.Velocity.Vx, p.Velocity.Vy);
			WindowsVector velocityNormal = new(normal.Vx, normal.Vy);

			velocity.Normalize();
			velocityNormal.Normalize();

			// Arrow tip height and width
			double h = p.R / 10;
			double w = p.R / 10;

			WindowsPoint p1 = new(p.Center.Vx, p.Center.Vy);
			WindowsPoint p2 = p1 + velocity * (p.R + p.Velocity.Length);
			WindowsPoint p4 = p2;
			WindowsPoint p6 = p2;

			WindowsPoint O = p2 - velocity * h;
			WindowsPoint p3 = O + velocityNormal * w;
			WindowsPoint p5 = O + velocityNormal * -w;

			vp.Velocity.SetPoints(
			new WindowsPoint[] { p1, p2, p3, p4, p5, p6 }
			);
		}

		private void _updateTrajectoryLine(VisualParticle vp)
		{
			var lineType = vp.Particle.TrajectoryLine.Type;
			if (lineType == StraightLineType.NotLine)
			{
				return;
			}

			double x1, x2, y1, y2;
			if (lineType == StraightLineType.Horizontal)
			{
				x1 = 0;
				x2 = _canvas.ActualWidth;
				y1 = vp.Particle.TrajectoryLine.GetY(x1);
				y2 = vp.Particle.TrajectoryLine.GetY(x2);
			}

			// Vertical or arbitrary line
			else
			{
				y1 = 0;
				y2 = _canvas.ActualHeight;
				x1 = vp.Particle.TrajectoryLine.GetX(y1);
				x2 = vp.Particle.TrajectoryLine.GetX(y2);
			}

			vp.Trajectory.P1 = new(x1, y1);
			vp.Trajectory.P2 = new(x2, y2);
		}
	}
}