#ifndef PARTICLE_CUH
#define PARTICLE_CUH

#include "cuda_macros.cuh"
#include "Vector.cuh"

class Particle
{
public:
	Vector Velocity;
	Vector Center;
	double R;
	double M;

	CUDA_CALLABLE_MEMBER Particle();
	CUDA_CALLABLE_MEMBER Particle(Vector center, Vector v, double r, double m);
};

#endif // !PARTICLE_CUH
