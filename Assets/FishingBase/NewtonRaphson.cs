using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 
/// Find roots of equations using Newton-Raphson method.
/// 
/// http://en.wikipedia.org/wiki/Newton's_method
/// 
/// </summary>
public class NewtonRaphson
{

	public delegate double Function(double x);
	
	/// <summary>
	/// Find root using the Method of Newton-Raphson.
	/// </summary>
	/// <param name="x0">
	/// A <see cref="System.Double"/> - First approximation to root value.
	/// </param>
	/// <param name="tolerance">
	/// A <see cref="System.Double"/> - Desired accuracy for root value.
	/// </param>
	/// <param name="maxIterations">
	/// A <see cref="System.Int32"/> - Maximum number of iterations.
	/// </param>
	/// <param name="function">
	/// A <see cref="Function"/> - Provides function values.
	/// </param>
	/// <param name="derivativeFunction"> - Provides function derivative values.
	/// A <see cref="Function"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Double"/> - Approximation to root.
	/// </returns>
	public static double FindRoot(
		double x0,
	    double tolerance,
		int maxIterations,
	    Function function,
		Function derivativeFunction)
	{
        if (function == null || derivativeFunction == null)
		{
            throw new Exception("Method requires both specified: function and derivative function.");
        }
		double fx;
        double dfx;
        double xPrevious;
        double x = x0;
        for (int iteration = 0; iteration < maxIterations; iteration++)
		{
            xPrevious = x;
            fx = function(xPrevious);
            dfx = derivativeFunction(xPrevious);
            x = xPrevious - (fx / dfx);
            if (Converged(x, xPrevious, fx, tolerance, tolerance))
			{
                break;
            }
        }
        return x;
    }

	/// <summary>
	/// 
	/// Find root using the Method of Newton-Raphson (with max 100 iterations).
    /// 
	/// </summary>
	/// <param name="x0">
	/// A <see cref="System.Double"/> - First approximation to root value.
	/// </param>
	/// <param name="function">
	/// A <see cref="Function"/> - Provides function values.
	/// </param>
	/// <param name="derivativeFunction"> - Provides function derivative values.
	/// A <see cref="Function"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Double"/> - Approximation to root.
	/// </returns>
	public static double FindRoot(
		double x0,
		Function function,
		Function derivativeFunction)
	{
	    return FindRoot(x0, double.Epsilon, 100, function, derivativeFunction);
	}
	
	/// <summary>
	/// 
	/// Test for convergence in root finder.
	///
	/// </summary>
	/// <param name="xCurrent">
	/// A <see cref="System.Double"/> - Current root.
	/// </param>
	/// <param name="xPrevious">
	/// A <see cref="System.Double"/> - Previous root.
	/// </param>
	/// <param name="fxCurrent">
	/// A <see cref="System.Double"/> - Function value at current root.
	/// </param>
	/// <param name="xTolerance">
	/// A <see cref="System.Double"/> - Convergence tolerance for estimates.
	/// </param>
	/// <param name="fxTolerance">
	/// A <see cref="System.Double"/> - Convergence tolerance for function values.
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
    public static bool Converged(
		double xCurrent,
	    double xPrevious,
	    double fxCurrent,
        double xTolerance,
	    double fxTolerance)
	{
        return (Math.Abs(xCurrent - xPrevious) <= xTolerance) || (Math.Abs(fxCurrent) <= fxTolerance);
    }
}
