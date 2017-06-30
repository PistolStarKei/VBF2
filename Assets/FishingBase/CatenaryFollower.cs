using UnityEngine;
using System.Collections;

/// <summary>
/// CatenaryFollower scripts uses LookAt() function to look at wherever selected Catenary is.
/// </summary>
public class CatenaryFollower : MonoBehaviour
{
	/// <summary>
	/// Catenary to follow.
	/// </summary>
	public Catenary catenary;
	public bool catenaryInfoEnabled = true;
	
	internal int boardX = 10;
	internal int boardY = 10;
	internal int boardWidth = 200;
	internal int boardHeight = 120;
	
	/// <summary>
	/// Looks at the Catenary.
	/// </summary>
	void LateUpdate()
	{
		if (this.catenary != null) 
		{
			Vector3 P1 = catenary.T1 == null ? catenary.P1 : catenary.T1.position;
			Vector3 P2 = catenary.T2 == null ? catenary.P2 : catenary.T2.position;
			transform.LookAt(P1 + (P2 - P1) / 2);
		}
	}
	
	/// <summary>
	/// Update Catenary information.
	/// </summary>
	void OnGUI()
	{
		if (this.catenary != null && this.catenaryInfoEnabled)
		{
			this.ShowAll(this.catenary);
		}
	} 
	
	/// <summary>
	/// Resets GUI positions.
	/// </summary>
	void ResetPosition()
	{
		this.boardX = 10;
		this.boardY = 10;
		this.boardWidth = 200;
		this.boardHeight = 120;
	}
	
	/// <summary>
	/// Shows all the Catenary information.
	/// </summary>
	/// <param name="catenary">
	/// A <see cref="Catenary"/>
	/// </param>
	void ShowAll(Catenary catenary)
	{
		ResetPosition();
		DisplayCatenaryInfo(catenary);
	}
	
	/// <summary>
	/// Draws GUI parts.
	/// </summary>
	/// <param name="catenary">
	/// A <see cref="Catenary"/>
	/// </param>
	void DisplayCatenaryInfo(Catenary catenary)
	{
		GUI.Box(new Rect(boardX, boardY, boardWidth, boardHeight), "Catenary Information");
		boardX += 5;
		boardY += 20;
		boardWidth = 190;
		boardHeight = 20;
		GUI.Label(new Rect(boardX, boardY, boardWidth, boardHeight), "P1: " + catenary.P1);
		boardY += 20;
		GUI.Label(new Rect(boardX, boardY, boardWidth, boardHeight), "P2: " + catenary.P2);
		boardY += 20;
		GUI.Label(new Rect(boardX, boardY, boardWidth, boardHeight), "Length: " + catenary.L);
		boardY += 20;
		GUI.Label(new Rect(boardX, boardY, boardWidth, boardHeight), "Vertices: " + catenary.N);
	}
}
