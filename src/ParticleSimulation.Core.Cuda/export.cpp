#define EXPORT __declspec(dllexport)

#include "Simulator.h"
#include "SimulatorInfoOut.h"
#include "SimulationStateOut.h"
#include "Collidables.cuh"

extern "C" 
{
	EXPORT 
	void* getSimulatorHandle(
		Collidables collidables, 
		SimulatorInfoOut infoOut,
		SimulationStateOut stateOut
	)
	{
		return new Simulator(collidables, infoOut, stateOut);
	}

	EXPORT 
	void __cdecl stepSimulation(Simulator* simulator)
	{
		simulator->Step();
	}

	EXPORT 
	void __cdecl startSimulation(Simulator* simulator)
	{
		simulator->StartSimulation();
	}

	EXPORT 
	void __cdecl stopSimulation(Simulator* simulator)
	{
		simulator->StopSimulation();
	}

	EXPORT 
	bool __cdecl nextSimulationState(Simulator* simulator)
	{
		return simulator->Next();
	}

	EXPORT 
	void __cdecl joinSimulator(Simulator* simulator)
	{
		simulator->Join();
	}

	EXPORT 
	void __cdecl freeHandle(Simulator* simulator)
	{
		delete simulator;
	}
}
