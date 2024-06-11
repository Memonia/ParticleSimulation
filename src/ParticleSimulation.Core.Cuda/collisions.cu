#include <math.h>

#include "cuda_macros.cuh"
#include "collisions.cuh"
#include "Particle.cuh"
#include "Surface.cuh"
#include "Vector.cuh"
#include "StraightLine.cuh"

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

	Vector nv1 = p1.Velocity - x12 * (2 * p2.M / m12) * (v12 * x12 / x12LenSqr);
	Vector nv2 = p2.Velocity - x21 * (2 * p1.M / m12) * (v21 * x21 / x12LenSqr);

	p1.Velocity = nv1;
	p2.Velocity = nv2;
}


CUDA_CALLABLE_MEMBER
double sign(double n)
{	
	return n < 0 ? -1 : 1;
}

CUDA_CALLABLE_MEMBER
double distance(const Vector& p1, const Vector& p2)
{	
	return sqrt(
		(p1.Vx - p2.Vx) * (p1.Vx - p2.Vx) + (p1.Vy - p2.Vy) * (p1.Vy - p2.Vy)
	);
}

bool isCollisionDetected(
	const Particle& p, const Surface& target, double& time_out)
{
	Vector closest{ target.LineEquation.ClosestPoint(p.Center) };
	Vector dVector{ closest - p.Center };

	if (p.Velocity * dVector <= 0)
		return false;

	double sc = target.SignedDistance(p.Center);
	double se = target.SignedDistance(p.Center + p.Velocity);
	if (sc * se > 0 && fabs(sc) > p.R && fabs(se) > p.R)
		return false;

	time_out = (sc - p.R) / (sc - se);
	return true;
}

bool isCollisionDetected(
	const Particle& p, const Particle& target, double& time_out)
{
	Vector Vab{ p.Velocity - target.Velocity };

	if (distance(p.Center, target.Center) - (p.R + target.R) > Vab.Length)
		return false;

	if (StraightLine(p.Center, Vab).Distance(target.Center) > p.R + target.R)
		return false;

	Vector dVector = p.Center - target.Center;
	if (Vab * (-dVector) <= 0)
		return false;

	double A = Vab * Vab;
	double B = dVector * Vab * 2;
	double C = dVector * dVector - (p.R + target.R) * (p.R + target.R);
	double Q = -(B + sign(B) * sqrt(B * B - 4 * A * C)) / 2;

	double t0 = Q / A;
	double t1 = C / Q;
	
	time_out = fmin(t0, t1);
	return true;
}
