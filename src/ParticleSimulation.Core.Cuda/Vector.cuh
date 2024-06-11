#ifndef VECTOR_CUH
#define VECTOR_CUH

#include "cuda_macros.cuh"

struct Vector
{
public:
	double Vx;
	double Vy;
	double Length;

	CUDA_CALLABLE_MEMBER Vector();
	CUDA_CALLABLE_MEMBER Vector(double vx, double vy);

	CUDA_CALLABLE_MEMBER Vector Normal() const;
	CUDA_CALLABLE_MEMBER Vector Normalised() const;
	CUDA_CALLABLE_MEMBER Vector Reflected(const Vector& n) const;

	CUDA_CALLABLE_MEMBER Vector operator-() const;
	CUDA_CALLABLE_MEMBER Vector operator-(const Vector v) const;
	CUDA_CALLABLE_MEMBER Vector operator+(const Vector v) const;
	CUDA_CALLABLE_MEMBER Vector operator*(double n) const;
	CUDA_CALLABLE_MEMBER double operator*(const Vector v) const;
};

#endif // VECTOR_CUH
