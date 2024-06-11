using System;
using System.Collections.Generic;
using System.Threading;

namespace ParticleSimulation.Core.Abstractions
{
	public interface ISimulator : IDisposable
	{
		IReadOnlyCollection<IParticle> Particles { get; }

		FrameInfo CurrentFrameInfo { get; }

		int QueueLength { get; }

		bool Next();

		void Reset();

		void Join();

		void Step();

		void StartSimulation(CancellationToken token);

		void SpawnParticles(SpawnParameters spawnParams, int? seed = null);
	}
}
