#ifndef COLLISIONINFO_CUH
#define COLLISIONINFO_CUH

struct CollisionInfo
{
public:
	// inf if no collision happened
	double Time;

	// [0, particles.Count - 1]
	int ParticleIndex;

	// [0, particles.Count - 1] or
	// [particles.Count, particles.Count + surfaces.Count - 1]
	int CollidableIndex;
};

#endif // !COLLISIONINFO_CUH
