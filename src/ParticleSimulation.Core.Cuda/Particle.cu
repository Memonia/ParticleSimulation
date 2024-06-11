#include <cstring>

#include "Particle.cuh"
#include "Vector.cuh"

Particle::Particle(Vector center, Vector v, double r, double m) :
	Velocity{ v },
	Center{ center },
	R{ r },
	M{ m }
{ }

Particle::Particle()
{
	std::memset(this, 0, sizeof(*this));
}
