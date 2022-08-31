using System;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

class Hello
{
  static void Main(string[] args)
  {
    Console.WriteLine("Hello World");
    Matrix<double> A = DenseMatrix.OfArray(new double[,] {
    {1,1,1,1},
    {1,2,3,4},
    {4,3,2,1}});
    Vector<double>[] nullspace = A.Kernel();

    // verify: the following should be approximately (0,0,0)
    
  }
}