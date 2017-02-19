using UnityEngine;
using System.Collections;

public class SpawnBoid : MonoBehaviour 
{

	public BoidManager manager;

	private Vector3 position;
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButton (0)) 
		{
			// get the position of the mouse click
			position = (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
				Input.mousePosition.y, 120.0f)));

			// set limits on the mouse click
			setLimits ();

			// call the manger and create a new boid
			manager.spawnNewBoid(position);
		}
	}

	void setLimits()
	{
		if (position.x > 40) 
		{
			position.x = 40;
		}

		if (position.x < -40) 
		{
			position.x = -40;
		}

		if (position.y > 40) 
		{
			position.y = 40;
		}

		if (position.y < -40) 
		{
			position.y = -40;
		}
	}
}
