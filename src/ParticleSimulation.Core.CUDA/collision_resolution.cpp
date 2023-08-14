#include "structures.cuh"
#include "collisions.cuh"

void resolveCollision(Particle& p, const Surface& s)
{
	p.Velocity = p.Velocity.Reflected(
		s.LineEquation.ParallelVector.Normal()
	);
}

void resolveCollision(Particle& p1, Particle& p2)
{
	Vector v12 = p1.Velocity - p2.Velocity;
	Vector v21 = p2.Velocity - p1.Velocity;
	Vector x12 = p1.Center - p2.Center;
	Vector x21 = p2.Center - p1.Center;
	double m12 = p1.M + p2.M;
	double x12LenSqr = x12.Length * x12.Length;

	Vector nv1 = p1.Velocity
		- x12 * (2 * p2.M / m12) * (v12 * x12 / x12LenSqr);

	Vector nv2 = p2.Velocity
		- x21 * (2 * p1.M / m12) * (v21 * x21 / x12LenSqr);

	p1.Velocity = nv1;
	p2.Velocity = nv2;
}
