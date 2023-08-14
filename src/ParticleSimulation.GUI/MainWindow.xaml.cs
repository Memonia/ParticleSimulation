using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ParticleSimulation.Core;
using ParticleSimulation.Core.Interface;
using ParticleSimulation.GUI.Actions;
using ParticleSimulation.GUI.Visuals;
using ParticleSimulation.GUI.Visuals.Updaters;

using CTS = System.Threading.CancellationTokenSource;
using UserAction = ParticleSimulation.GUI.Actions.Action;

namespace ParticleSimulation.GUI
{
	public partial class MainWindow : Window
	{
		private ISimulator _simulator;
		private CanvasUpdater _canvasUpdater;
		private VisualCanvas _selfUpdatingCanvas;
		private readonly ActionStateMachine _actionStateMachine;

		// spf = 1/fps
		// multiply by 1000 so it's milliseconds and not seconds
		private const double _canvasUpdateInterval = 1d/144 * 1000;
		private UserAction _prevAction = UserAction.Start;

		private CTS _simulatorCTS = new();
		private CTS _canvasUpdaterCTS = new();

		private readonly Regex _regexIntOnly = new(@"[0-9]");

		public MainWindow()
		{
			void _loadedCallback(object o, RoutedEventArgs e)
			{
				_simulator = SimulatorFactory.GetSimulator
				(
					CanvasContainer.ActualWidth,
					CanvasContainer.ActualHeight
				);

				_selfUpdatingCanvas = new();
				CanvasContainer.Child = _selfUpdatingCanvas;

				_canvasUpdater = new(_selfUpdatingCanvas, _simulator, _canvasUpdateInterval)
				{
					InfoBlockUpdater = new(PerformanceInfoBlock)
				};

				_simulator.SpawnParticles(
					SpawnSettings.SpawnParameters, SpawnSettings.Seed
				);

				// Start updating immediately
				_canvasUpdater.Init();
				_canvasUpdater.StartUpdating(_canvasUpdaterCTS.Token);
			}

			InitializeComponent();

			Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
			Debug.AutoFlush = true;

			// Show initial values
			SpeedBox.Text = SpawnSettings.SpeedSpan.ToString();
			SizesBox.Text = SpawnSettings.RadiusSpan.ToString();
			AmountBox.Text = SpawnSettings.ParticleAmount.ToString();
			SizeMultBox.Text = SpawnSettings.SizeMultiplyer.ToString();

			// The size is known when it fires
			this.Loaded += _loadedCallback;
			
			// Callbacks for each user action
			_actionStateMachine = new();
			_actionStateMachine.OnStart += _onStart;
			_actionStateMachine.OnStep += _onStep;
			_actionStateMachine.OnPause += _onPause;
			_actionStateMachine.OnResume += _onResume;
			_actionStateMachine.OnReset += _onReset;
		}

		private void _onStart()
		{
			_canvasUpdater.Flags |= VisualisationFlags.ForwardSimulation;
			_simulator.StartSimulation(_simulatorCTS.Token);

			StartOrResumeButton.Content = "Resume";
			StartOrResumeButton.IsEnabled = false;
			StepButton.IsEnabled = false;
			ResetButton.IsEnabled = false;
			PauseButton.IsEnabled = true;
			_prevAction = UserAction.Start;
		}
				
		private void _onStep()
		{
			_canvasUpdater.Flags |= VisualisationFlags.ForwardSimulation;
			_simulator.Step();

			StartOrResumeButton.IsEnabled = true;
			ResetButton.IsEnabled = true;
			PauseButton.IsEnabled = false;
			_prevAction = UserAction.Step;
		}

		private void _onPause()
		{
			_canvasUpdater.Flags &= ~VisualisationFlags.ForwardSimulation;

			StartOrResumeButton.Content = "Resume";
			StartOrResumeButton.IsEnabled = true;
			StepButton.IsEnabled = false;
			ResetButton.IsEnabled = true;
			PauseButton.IsEnabled = false;
			_prevAction = UserAction.Pause;
		}

		private void _onResume()
		{
			_canvasUpdater.Flags |= VisualisationFlags.ForwardSimulation;

			StartOrResumeButton.IsEnabled = false;
			StepButton.IsEnabled = false;
			ResetButton.IsEnabled = false;
			PauseButton.IsEnabled = true;
			_prevAction = UserAction.Resume;
		}

		private void _onReset()
		{
			// Stop the simulation and the canvas updater 
			Debug.WriteLine($"Attempting to stop CanvasUpdater");
			_canvasUpdaterCTS.Cancel();
			Debug.WriteLine($"Attempting to stop Simulator");
			_simulatorCTS.Cancel();

			// Wait for both to stop
			_simulator.Join();
			Debug.WriteLine($"Simulator stopped");
			_canvasUpdater.Join();
			Debug.WriteLine($"CanvasUpdater Stopped");

			_simulatorCTS = new CTS();
			_canvasUpdaterCTS = new CTS();

			// Reset the simulator
			_simulator.Reset();
			_simulator.SpawnParticles(
				SpawnSettings.SpawnParameters, SpawnSettings.Seed
			);

			// Reset and start the canvas updater
			_canvasUpdater.Flags &= ~VisualisationFlags.ForwardSimulation;
			_canvasUpdater.Init();
			_canvasUpdater.StartUpdating(_canvasUpdaterCTS.Token);

			StartOrResumeButton.Content = "Start";
			StartOrResumeButton.IsEnabled = true;
			StepButton.IsEnabled = true;
			ResetButton.IsEnabled = true;
			PauseButton.IsEnabled = false;
			_prevAction = UserAction.Reset;
		}

		private void StartOrResume(object sender, RoutedEventArgs e)
		{
			switch (_prevAction)
			{
				case UserAction.Start:
				case UserAction.Step:
				case UserAction.Reset:
					_actionStateMachine.Switch(UserAction.Start);
					break;

				default:
					_actionStateMachine.Switch(UserAction.Resume);
					break;
			}
		}
			
		private void Step(object sender, RoutedEventArgs e)
		{
			_actionStateMachine.Switch(UserAction.Step);
		}

		private void Pause(object sender, RoutedEventArgs e)
		{
			_actionStateMachine.Switch(UserAction.Pause);
		}

		private void Reset(object sender, RoutedEventArgs e)
		{
			_actionStateMachine.Switch(UserAction.Reset);
		}

		private void SettingInput(object sender, TextCompositionEventArgs e)
		{
			if (!_regexIntOnly.IsMatch(e.Text))
			{
				e.Handled = true;
			}
		}

		private void SettingsChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is not TextBox t)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(t.Text))
			{
				t.Text = "0";
			}

			if (t.Name == AmountBox.Name)
			{
				if (Int32.Parse(t.Text) > SpawnSettings.MaxParticleAmount)
				{
					t.Text = SpawnSettings.MaxParticleAmount.ToString();
				}

				if (Int32.Parse(t.Text) < SpawnSettings.MinParticleAmount)
				{
					t.Text = SpawnSettings.MinParticleAmount.ToString();
				}

				SpawnSettings.ParticleAmount = Int32.Parse(t.Text);
			}

			else
			if (t.Name == SpeedBox.Name)
			{
				if (Int32.Parse(t.Text) > SpawnSettings.MaxSpeedSpan)
				{
					t.Text = SpawnSettings.MaxSpeedSpan.ToString();
				}

				SpawnSettings.SpeedSpan = Int32.Parse(t.Text);
			}

			else
			if (t.Name == SizesBox.Name)
			{
				if (Int32.Parse(t.Text) > SpawnSettings.MaxRadiusSpan)
				{
					t.Text = SpawnSettings.MaxRadiusSpan.ToString();
				}

				SpawnSettings.RadiusSpan = Int32.Parse(t.Text);
			}

			else
			if (t.Name == SizeMultBox.Name)
			{
				if (Int32.Parse(t.Text) > SpawnSettings.MaxSizeMultiplyer)
				{
					t.Text = SpawnSettings.MaxSizeMultiplyer.ToString();
				}

				if (Int32.Parse(t.Text) < 1)
				{
					t.Text = "1";
				}

				SpawnSettings.SizeMultiplyer = Int32.Parse(t.Text);
			}

			t.CaretIndex = t.Text.Length;
		}

		private void TrackingInfoChanged(object sender, RoutedEventArgs e)
		{
			void _flipFlag(VisualisationFlags flag) => _canvasUpdater.Flags ^= flag;		
			
			void _flipBoolFlag(ref bool flag) => flag = !flag;

			if (sender is not CheckBox t)
			{
				return;
			}

			if (t.Name == TrajectoryCheck.Name)
			{
				_flipFlag(VisualisationFlags.ShowTrajectoryLine);
			}
			else if (t.Name == VelocityCheck.Name)
			{
				_flipFlag(VisualisationFlags.ShowVelocityVector);
			}
			else if (t.Name == TrailCheck.Name)
			{
				_flipFlag(VisualisationFlags.ShowTrail);
			}
			else if (t.Name == SlowestCheck.Name)
			{
				_flipFlag(VisualisationFlags.ShowSlowest);
			}
			else if (t.Name == FastestCheck.Name)
			{
				_flipFlag(VisualisationFlags.ShowFastest);
			}
			else if (t.Name == TrackAllCheck.Name)
			{
				_flipFlag(VisualisationFlags.TrackAll);
			}
			else if (t.Name == HideCheck.Name)
			{
				_flipFlag(VisualisationFlags.HideNotTracked);
			}
			else if (t.Name == BigParticleCheck.Name)
			{
				_flipBoolFlag(ref SpawnSettings.BigParticle);
			}
		}

		private void OnClosing(object sender, CancelEventArgs e)
		{
			_simulatorCTS.Cancel();
			_canvasUpdaterCTS.Cancel();
		}
	}
}
