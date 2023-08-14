namespace ParticleSimulation.Core.Interface
{
	public record SpawnParameters
	(
		double BaseRadius,
		int Amount,
		int SpeedSpan,
		int RadiusSpan,
		bool BigParticle,
		double SizeMultiplyer
	);
}
