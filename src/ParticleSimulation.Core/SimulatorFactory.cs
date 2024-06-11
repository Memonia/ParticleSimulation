using System;

using Microsoft.Extensions.Configuration;

using ParticleSimulation.Core.Abstractions;

namespace ParticleSimulation.Core
{
	public static class SimulatorFactory
	{
		private static readonly bool _enableGPU;

		static SimulatorFactory()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("core_config.json")
				.Build();

			_enableGPU = Boolean.Parse(config["enable_gpu"]);

			Console.WriteLine($"GPU acceleration: {_enableGPU}\n");
		}

		public static ISimulator GetSimulator(double containerWidth, double containerHeight)
		{
			var container = new Container(containerWidth, containerHeight);

			if (_enableGPU)
			{
				return new NativeSimulator(container);
			}

			return new Simulator(container);
		}
	}
}
