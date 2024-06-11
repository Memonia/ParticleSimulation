#ifndef SURFACE_CUH
#define SURFACE_CUH

#include "cuda_macros.cuh"
#include "Vector.cuh"
#include "StraightLine.cuh"

class Surface
{
public:
	const Vector InBoundsNormal;
	const StraightLine LineEquation;

	CUDA_CALLABLE_MEMBER Surface(StraightLine lineEquation, Vector inBoundsNormal);
	CUDA_CALLABLE_MEMBER double SignedDistance(const Vector p) const;
};

#endif // !SURFACE_CUH
