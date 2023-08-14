using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

using Microsoft.Extensions.Configuration;

namespace ParticleSimulation.GUI.Visuals
{
	internal partial class VisualCanvas
	{
		static VisualCanvas()
		{
			void _showError(string message) => 
				MessageBox.Show(
					message, "Error",
					MessageBoxButton.OK, MessageBoxImage.Error
				);
			
			try
			{
				var converter = new BrushConverter();
				var config = new ConfigurationBuilder()
					.AddJsonFile("colors.json")
					.Build();

				var tokens = new string[]
				{ 
					"particle", 
					"tracked_particle", 
					"slowest_particle",
					"fastest_particle",
					"trajectory_line",
					"velocity_vector",
					"trail"
				};

				for (int i = 0; i < tokens.Length; ++i)
				{
					var s = config[tokens[i]];
					if (s == null)
					{
						_showError($"'{tokens[i]}' token is missing in colors.json");
						throw new Exception();
					}

					tokens[i] = s;
				}

				_particleColor		  = (Brush)converter.ConvertFromString(tokens[0])!;
				_trackedParticleColor = (Brush)converter.ConvertFromString(tokens[1])!;
				_slowestParticleColor = (Brush)converter.ConvertFromString(tokens[2])!;
				_fastestParticleColor = (Brush)converter.ConvertFromString(tokens[3])!;
				_trajectoryLineColor  = (Brush)converter.ConvertFromString(tokens[4])!;
				_velocityVectorColor  = (Brush)converter.ConvertFromString(tokens[5])!;
				_trailColor			  = (Brush)converter.ConvertFromString(tokens[6])!;
				_hiddenParticleColor  = Brushes.Black;	
			}

			catch (Exception ex)
			{
				if (ex is InvalidDataException)
				{
					_showError("Invalid colors.json file");
				}

				if (ex is FormatException)
				{
					_showError("Check if all values in colors.json are valid");
				}

				Application.Current.Shutdown();
			}
		}

		private static readonly Brush _particleColor;
		private static readonly Brush _trackedParticleColor;
		private static readonly Brush _slowestParticleColor;
		private static readonly Brush _fastestParticleColor;
		private static readonly Brush _trailColor;
		private static readonly Brush _trajectoryLineColor;
		private static readonly Brush _velocityVectorColor;
		private static readonly Brush _hiddenParticleColor;

		private static readonly int _trailThickness = 1;
		private static readonly int _trajectoryLineThickness = 1;
		private static readonly int _velocityVectorThickness = 1;
	}
}
