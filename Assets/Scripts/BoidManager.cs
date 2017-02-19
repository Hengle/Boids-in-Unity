using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidManager : MonoBehaviour 
{
	public GameObject boid_prefab;
	private List<Boid> boids;

	private Vector3 position;

	static int noBoids;

	// Use this for initialization
	void Start () 
	{
		boids = new List<Boid>();
	}

	// Update is called once per frame
	void Update () 
	{
		setBoids ();
	}

	void setBoids()
	{
		for (int i = 0; i < boids.Count; i++)
		{
			boids[i].run (boids);
		}
	}

	public void spawnNewBoid(Vector3 position)
	{
		if (noBoids < 200) 
		{
			GameObject new_boid = Instantiate (boid_prefab, position, Quaternion.Euler (0, 0, 0)) as GameObject;

			boids.Add (new_boid.GetComponent<Boid> ());
			noBoids++;
		}
	}

	public static int getBoids()
	{
		return noBoids;
	}
}
