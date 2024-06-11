#ifndef SIMULATOR_H
#define SIMULATOR_H

#include <mutex>
#include <thread>
#include <stop_token>
#include <condition_variable>
#include <concurrent_queue.h>

#include "SimulationStateOut.h"
#include "SimulationState.h"
#include "SimulatorInfoOut.h"
#include "Collidables.cuh"

class Simulator
{
private:
	const size_t _queueSize;
	concurrency::concurrent_queue<SimulationState> _commitQueue;

	std::mutex _queueMutex;
	std::jthread _simulationThread;
	std::condition_variable _queueCondVar;

	Collidables _collidables;
	SimulatorInfoOut _simulatorInfo;
	SimulationStateOut _currentCommitedState;

	void _simulate(std::stop_token token);

public:
	Simulator(
		const Collidables& collidables,
		const SimulatorInfoOut& infoOut,
		const SimulationStateOut& stateOut,
		size_t queueSize = 2048
	);

	~Simulator();

	bool Next();
	void Step();
	void StartSimulation();
	void StopSimulation();
	void Join();
};

#endif // ! SIMULATOR_H
