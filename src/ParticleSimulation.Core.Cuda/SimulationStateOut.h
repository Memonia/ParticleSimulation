#ifndef SIMULATIONSTATEOUT_H
#define SIMULATIONSTATEOUT_H

#include "Particle.cuh"

struct SimulationStateOut
{
public:
	int* Collisions;
	Particle* Particles;
};

#endif // !SIMULATIONSTATEOUT_H
