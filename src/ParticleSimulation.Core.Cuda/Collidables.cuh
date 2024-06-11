#ifndef COLLIDABLES_CUH
#define COLLIDABLES_CUH

#include "Surface.cuh"
#include "Particle.cuh"

struct Collidables
{
public:
	Surface* Surfaces;
	int SurfaceCount;
	Particle* Particles;
	int ParticleCount;
};

#endif // !COLLIDABLES_CUH
