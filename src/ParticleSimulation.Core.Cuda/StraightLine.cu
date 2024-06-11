#include <math.h>
#include <cstring>

#include "StraightLine.cuh"

StraightLine::StraightLine(double a, double b, double c) :
	A{ a }, B{ b }, C{ c },
	ParallelVector{ Vector(-b, a) },
	_notLine{ (a == 0) && (b == 0) }
{ }

StraightLine::StraightLine(Vector p, Vector v) :
	StraightLine
	(
		v.Vy,
		-v.Vx,
		v.Vx* p.Vy - v.Vy * p.Vx
	)
{ }

double StraightLine::Distance(Vector p) const
{
	if (!_notLine)
		return fabs(A * p.Vx + B * p.Vy + C) / sqrt(A * A + B * B);
	return NAN;
}

Vector StraightLine::ClosestPoint(Vector p) const
{
	if (_notLine)
		return Vector(0, 0);

	const double AApBB = A * A + B * B;
	double x = ((B * (B * p.Vx - A * p.Vy)) - A * C) / AApBB;
	double y = ((A * ((-B * p.Vx) + (A * p.Vy))) - B * C) / AApBB;

	return Vector(x, y);
}

Vector StraightLine::DistanceVectorFrom(Vector p) const
{
	if (!_notLine)
		return p - ClosestPoint(p);
	return Vector(0, 0);
}
