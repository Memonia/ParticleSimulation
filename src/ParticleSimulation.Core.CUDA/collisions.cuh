#ifndef	COLLISIONS_CUH
#define COLLISIONS_CUH

#include "cuda_macros.cuh"
#include "Surface.cuh"
#include "Particle.cuh"
#include "Collidables.cuh"
#include "CollisionInfo.cuh"

CollisionInfo detectNearestAndAdvanceCuda(const Collidables& collidables, double frameTime);

void resolveCollision(Particle& p, const Surface& s);

void resolveCollision(Particle& p1, Particle& p2);

CUDA_CALLABLE_MEMBER
bool isCollisionDetected(const Particle& p, const Surface& target, double& time_out);

CUDA_CALLABLE_MEMBER
bool isCollisionDetected(const Particle& p, const Particle& target, double& time_out);

#endif // !COLLISIONS_CUH
