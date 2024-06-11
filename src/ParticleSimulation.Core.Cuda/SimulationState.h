#ifndef SIMULATIONSTATE_H
#define SIMULATIONSTATE_H

#include "Particle.cuh"

struct SimulationState
{
public:
	int Collisions;
	Particle* Particles;
};

#endif // !SIMULATIONSTATE_H
