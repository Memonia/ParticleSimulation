#include "Surface.cuh"
#include "StraightLine.cuh"

Surface::Surface(StraightLine lineEquation, Vector inBoundsNormal) :
	LineEquation{ lineEquation },
	InBoundsNormal{ inBoundsNormal }
{ }

double Surface::SignedDistance(const Vector p) const
{
	double d = LineEquation.Distance(p);
	Vector dVector = LineEquation.DistanceVectorFrom(p);

	if (InBoundsNormal * dVector > 0)
		return d;
	return -d;
}
