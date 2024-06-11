#include <math.h>
#include <cstring>

#include "Vector.cuh"

Vector::Vector(double vx, double vy) :
	Vx{ vx },
	Vy{ vy },
	Length{ sqrt(vx * vx + vy * vy) }
{ }

Vector::Vector()
{
	std::memset(this, 0, sizeof(*this));
}

Vector Vector::Normal() const
{
	return Vector(Vy, -Vx);
}

Vector Vector::Normalised() const
{
	return Vector(Vx / Length, Vy / Length);
}

Vector Vector::Reflected(const Vector& n) const
{
	auto ned = n.Normalised();
	return *this - ned * (*this * ned) * 2;
}

Vector Vector::operator-() const
{
	return Vector(-Vx, -Vy);
}

Vector Vector::operator-(const Vector v) const
{
	return Vector(Vx - v.Vx, Vy - v.Vy);
}

Vector Vector::operator+(const Vector v) const
{
	return Vector(Vx + v.Vx, Vy + v.Vy);
}

Vector Vector::operator*(double n) const
{
	return Vector(Vx * n, Vy * n);
}

double Vector::operator*(const Vector v) const
{
	return Vx * v.Vx + Vy * v.Vy;
}
