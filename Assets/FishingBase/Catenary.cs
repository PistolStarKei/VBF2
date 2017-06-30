using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Catenary script: http://en.wikipedia.org/wiki/Catenary
/// </summary>
[ExecuteInEditMode]
public class Catenary : MonoBehaviour {
	
	public delegate double Function(double x, double a, double v, double b);
	
	public Color CS = Color.yellow;
    public Color CE = Color.red;
	public Transform T1;
	public Transform T2;
	public Vector3 P1 = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 P2 = new Vector3(10.0f, 10.0f, 10.0f);
	public float W = 1.0F;
	public float L = 20.0F;
	public int N = 10;
	public bool arch = false;
	public Material catenaryMaterial;
	
	internal bool previousSolved = false;
	internal Vector3 prevP1 = new Vector3(0.0f, 0.0f, 0.0f);
	internal Vector3 prevP2 = new Vector3(10.0f, 10.0f, 10.0f);
	internal float prevW = 1.0F;
	internal float prevL = 20.0F;
	internal int prevN = 10;
	internal bool prevArch = false;
	
	[SerializeField]
	[HideInInspector]
	private string id;
	
	/// <summary>
	/// Sets Catenary's unique ID. This ID must uniquely identify this specific Catenary instance among the others.
	/// </summary>
	/// <param name="id">
	/// A <see cref="System.String"/> - unique ID.
	/// </param>
	public void SetId(string id)
	{
		this.id = id;
	}
	
	/// <summary>
	/// Returns Catenary's unique ID.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/> - Catenary's unique ID.
	/// </returns>
	public string GetId()
	{
		return this.id;
	}
	
	/// <summary>
	/// Copies configuration of other catenary
	/// </summary>
	/// <param name="other">
	/// A <see cref="Catenary"/> - other catenary.
	/// </param>
	public void CopyConfigurationFrom(Catenary other)
	{
		CS = other.CS;
		CE = other.CE;
		T1 = other.T1;
		T2 = other.T2;
		P1 = other.P1;
		P2 = other.P2;
		W = other.W;
		L = other.L;
		N = other.N;
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
    void Start()
	{
		this.UpdateEndPoints();
		if ((P2 - P1).magnitude <= 0)
		{
			Debug.LogError("Distance between catenary end-points should be more than 0");
			this.enabled = false;
		}
		this.UpdateLineRenderer();
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
    void Update()
	{
		this.UpdateEndPoints();
		if ((P2 - P1).magnitude <= 0)
		{
			return;
		}
        this.UpdateLineRenderer();
		if (P1 != prevP1 || P2 != prevP2 || L != prevL || N != prevN || W != prevW || arch != prevArch)
		{
			prevP1 = P1;
			prevP2 = P2;
			prevL = L;
			prevN = N;
			prevW = W;
			prevArch = arch;
		}
		else
		{
			return;
		}
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		// Catenary function
		Catenary.Function f = delegate(double x0, double a0, double v0, double b0)
		{
			return a0 * Math.Cosh((x0 + v0) / a0) + b0;
		};
		float w = 1.0f;// / W;
		float l = L * w;
		// Before rotation
		float x1i = P1.x * w;
		float x2i = P2.x * w;
		float y1i = P1.y * w;
		float y2i = P2.y * w;
		float z1i = P1.z * w;
		float z2i = P2.z * w;
		// Rotating P2
		P2 = new Vector3(P1.x + (new Vector3(P2.x, 0.0f, P2.z) - new Vector3(P1.x, 0.0f, P1.z)).magnitude, P2.y, 0.0f);
		// After rotation
		float x1 = P1.x * w;
		float x2 = P2.x * w;
		float y1 = P1.y * w;
		float y2 = P2.y * w;
		double a = 0; 
		double v = 0;
		double b = 0;
		bool solved = false;
		if (Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)) < l)
		{	
			solved = this.Solve(x1, (arch ? -1 : 1) * y1, x2, (arch ? -1 : 1) * y2, l, out a, out v, out b);
			if (solved) 
			{
				double dx = N <= 1 ? 0 : ((x2 - x1) / (N - 1));
				double dxi = N <= 1 ? 0 : ((x2i - x1i) / (N - 1));
				double dzi = N <= 1 ? 0 : ((z2i - z1i) / (N - 1));
				double prevY = y1;
		        for (int i = 0; i < N; i++)
				{
					double x = x1 + ((double) i) * dx;
					double xi = x1i + ((double) i) * dxi;
					double zi = z1i + ((double) i) * dzi;
					double y = (arch ? -1 : 1) * f(x, a, v, b);
					if (
						double.IsNaN(y) ||
						double.IsInfinity(y) ||
						Math.Abs(y - prevY) > l
					)
					{
						solved = false;
						break;
					}
		            Vector3 pos = new Vector3((float) xi / w, (float) y / w, (float) zi / w);
		            lineRenderer.SetPosition(i, pos);
					prevY = y;
		        }
			}
			if (!solved && previousSolved)
			{
				Debug.Log("Catenary could not be solved. Distance between end-points is aparently too short comparing to configured catenary length.");
			}
		}
		if (!solved)
		{
			double dxi = N <= 1 ? 0 : ((x2i - x1i) / (N - 1));
			double dyi = N <= 1 ? 0 : ((y2i - y1i) / (N - 1));
			double dzi = N <= 1 ? 0 : ((z2i - z1i) / (N - 1));
	        for (int i = 0; i < N; i++)
			{
				double xi = x1i + ((double) i) * dxi;
				double yi = y1i + ((double) i) * dyi;
				double zi = z1i + ((double) i) * dzi;
	            Vector3 pos = new Vector3((float) xi / w, (float) yi / w, (float) zi / w);
	            lineRenderer.SetPosition(i, pos);
	        }
		}
		previousSolved = solved;
    }
	
	/// <summary>
	/// Rotates around point.
	/// </summary>
	/// <returns>
	/// The around point.
	/// </returns>
	/// <param name='point'>
	/// Point.
	/// </param>
	/// <param name='pivot'>
	/// Pivot.
	/// </param>
	/// <param name='quat'>
	/// Quat.
	/// </param>
	private Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion quat)
	{
    	return quat * (point - pivot) + pivot;
	}
	
	/// <summary>
	/// Updates the line renderer.
	/// </summary>
	private void UpdateLineRenderer()
	{
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		if (lineRenderer == null)
		{
	        lineRenderer = gameObject.AddComponent<LineRenderer>();
		}
		lineRenderer.material = this.catenaryMaterial;
		lineRenderer.SetColors(CS, CE);
        lineRenderer.SetWidth(W, W);
		N = N < 2 ? 2 : N;
        lineRenderer.SetVertexCount(N);
	}
	
	/// <summary>
	/// Updates the end points.
	/// </summary>
	private void UpdateEndPoints()
	{
		P1 = T1 == null ? P1 : T1.position;
		P2 = T2 == null ? P2 : T2.position;
		if (P2.x < P1.x)
		{
			Vector3 tP2 = P2;
			P2 = P1;
			P1 = tP2;
		}
	}
	
	/// <summary>
	/// Solve catenary with the specified x1, y1, x2, y2, L, a, v and b.
	/// </summary>
	/// <param name='x1'>
	/// If set to <c>true</c> x1.
	/// </param>
	/// <param name='y1'>
	/// If set to <c>true</c> y1.
	/// </param>
	/// <param name='x2'>
	/// If set to <c>true</c> x2.
	/// </param>
	/// <param name='y2'>
	/// If set to <c>true</c> y2.
	/// </param>
	/// <param name='L'>
	/// If set to <c>true</c> l.
	/// </param>
	/// <param name='a'>
	/// If set to <c>true</c> a.
	/// </param>
	/// <param name='v'>
	/// If set to <c>true</c> v.
	/// </param>
	/// <param name='b'>
	/// If set to <c>true</c> b.
	/// </param>
	private static bool Solve(float x1, float y1, float x2, float y2, float L, out double a, out double v, out double b)
	{
		a = 0;
		v = 0;
		b = 0;
		double d = x2 - x1;
    	double h = y2 - y1;
		NewtonRaphson.Function f = delegate(double a0)
		{
			return 2.0 * a0 * Math.Sinh(d / (2.0 * a0)) - Math.Sqrt((L * L) - (h * h));
		};
	    NewtonRaphson.Function df = delegate(double a0)
		{
			return 2.0 * Math.Sinh(d / (2.0 * a0)) - d * Math.Cosh(d / (2.0 * a0)) / a0;
		};
		double newA = NewtonRaphson.FindRoot(0.01 * d, 0.00001, 100, f, df);
		if (double.IsNaN(newA) || double.IsInfinity(newA))
		{
			return false;
		}
		else
		{
			a = newA;
			double k = 0.5 * (a * Math.Log((L + h) / (L - h)) - d);
			v = k - x1;
			b = y1 - a * Math.Cosh(k / a);
			return true;
		}
	}
}
