#ifndef STRAIGHTLINE_CUH
#define STRAIGHTLINE_CUH

#include "cuda_macros.cuh"
#include "Vector.cuh"

struct StraightLine
{
private:
	const bool _notLine = false;

public:
	const double A;
	const double B;
	const double C;
	const Vector ParallelVector;

	CUDA_CALLABLE_MEMBER StraightLine(double a, double b, double c);
	CUDA_CALLABLE_MEMBER StraightLine(Vector p, Vector v);
	CUDA_CALLABLE_MEMBER double Distance(Vector p) const;
	CUDA_CALLABLE_MEMBER Vector ClosestPoint(Vector p) const;
	CUDA_CALLABLE_MEMBER Vector DistanceVectorFrom(Vector p) const;
};


#endif // !STRAIGHTLINE_CUH
