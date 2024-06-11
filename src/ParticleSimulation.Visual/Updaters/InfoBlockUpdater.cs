using System.Windows.Controls;

namespace ParticleSimulation.Visual.Updaters
{
	internal class InfoBlockUpdater
	{
		private readonly TextBlock _infoBlock;

		public InfoBlockUpdater(TextBlock infoBlock)
		{
			_infoBlock = infoBlock;
		}

		public void Update(int collisions, int queueLength)
		{
			_infoBlock.Text =
				$"<Queue Length: {queueLength:00000} " +
				$"Collisions: {collisions:000}>";
		}
	}
}
