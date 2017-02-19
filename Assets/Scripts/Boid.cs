using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : MonoBehaviour 
{
	//Vector3 position;// the boids transform
	Vector3 position;
	Vector3 velocity;
	Vector3 acceleration;

	public float maxForce; // Maximum steering force
	public float maxSpeed; // Maximum speed

	public float neighbourDistance;
	public float desiredSeperation;

	Quaternion newRotation;

	private TrailRenderer tRend;

	// Use this for initialization
	void Start () 
	{
		desiredSeperation = 3.0f;
		tRend = gameObject.GetComponent<TrailRenderer> ();
		
		acceleration = Vector3.zero;

		velocity.x = Random.Range(0.5f, 2.0f);
		velocity.y = Random.Range(0.5f, 2.0f);
		velocity.z = Random.Range(0.5f, 2.0f);

		//float angle = Random (TWO_PI);
		float angle = Random.Range(0.0f, (Mathf.PI * 2));
		float angle1 = Mathf.Cos (angle);
		float angle2 = Mathf.Sin (angle);
		float angle3 = Mathf.Tan (angle);
		velocity = new Vector3(angle1, angle2, angle3); 


		maxSpeed = 1.0f;
		maxForce = 0.03f;
		neighbourDistance = 20.0f;

	}

	public void run(List<Boid> boids)
	{
		flock (boids);
		update ();
	}

	void applyForce(Vector3 force)
	{
		// add force to acceleration?
		acceleration += force;
	}

	void flock(List<Boid> boids)
	{
		Vector3 sep = seperate (boids); // seperation
		Vector3 ali = align (boids);    // alignment
		Vector3 coh = cohesion(boids);  // cohesion

		// Arbitrarily weight these forces
		sep = (sep * 1.5f);
		ali = (ali * 1.0f);
		coh = (coh * 1.0f);
		applyForce (sep);
		applyForce (ali);
		applyForce (coh);
	}

	void update () 
	{
		// update velocity
		velocity = (velocity + acceleration);

		// limit speed
		//velocity.limit (maxSpeed); 
		velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

		transform.position = (transform.position + velocity);

		transform.rotation = Quaternion.LookRotation(velocity);

		// reset acceleration to 0 each cycle
		acceleration = Vector3.zero;

		checkPosition ();
	}

	// STEER = DESIRED MINUS VELOCITY
	Vector3 seek(Vector3 target)
	{
		Vector3 desired = (target - transform.position); //  a vector pointing from the position to the target
		// scale to max speed
		desired.Normalize();
		desired = (desired * maxSpeed);

		//desired.setMag (maxSpeed);
		desired = Vector3.ClampMagnitude(desired, maxSpeed);

		// steering = desired minus velocity
		Vector3 steer = (desired - velocity);
		//steer.limit (maxForce); // Limit to maximum steering force
		steer = Vector3.ClampMagnitude(steer, maxForce);
		return steer;
	}

	// Seperation
	// Method checks for nearby boids and steers away
	public Vector3 seperate(List<Boid> boids)
	{
		Vector3 steer = Vector3.zero;
		int count = 0;

		// check through every other boid
		for (int i = 0; i < boids.Count; i++) 
		{
			float d = Vector3.Distance (transform.position, boids [i].transform.position);
			// if boid is a neighbour
			if (d > 0 && d < desiredSeperation) 
			{
				// Calculate vector pointing away from neighbour
				Vector3 diff = (transform.position - boids [i].transform.position);
				diff.Normalize ();
				diff = (diff / d); // Weight by distance
				steer = (steer + diff);
				count++;
			}
		}

		// Average -- divided by how many
		if (count > 0) 
		{
			steer = (steer / count);
		}

		// as long as the vector is greater than 0
		if (steer != Vector3.zero)  // if(steer.mag() > 0)
		{
			//steer.setMag (maxSpeed);
			steer = Vector3.ClampMagnitude(steer, maxSpeed);

			// implement Reynolds: steering = desired - velocity
			steer.Normalize();
			steer = (steer * maxSpeed);
			steer = (steer - velocity);
			//steer.limit (maxForce);
			steer = Vector3.ClampMagnitude(steer, maxForce);
		}
		return steer;	
	}

	// Alignment
	// For every nearby boid in the system, calculate the average velocity
	public Vector3 align(List<Boid> boids)
	{
		// The position the boid want to be
		Vector3 sum = Vector3.zero;
		int count = 0;

		// check through every other boid
		for (int i = 0; i < boids.Count; i++) 
		{
			float d = Vector3.Distance (transform.position, boids [i].transform.position);
			// if boid is a neighbour
			if ((d > 0) && (d < neighbourDistance)) 
			{
				sum = (sum + boids[i].velocity);
				count++;
			}
		}

		if (count > 0)
		{
			sum = (sum / count);
			//sum.setMag (maxSpeed);
			sum = Vector3.ClampMagnitude(sum, maxSpeed);

			// Implement Reynolds: Steering = desired - velocity
			sum.Normalize();
			sum = (sum * maxSpeed);
			Vector3 steer = (sum - velocity);
			//steer.limit (maxForce);
			steer = Vector3.ClampMagnitude(steer, maxForce);
			return steer;
		}
		else 
			return new Vector3();
	}

	// Cohesion
	// For the average position (I.E. center) of all nearby boids, calculate steering
	// vector towards that position
	public Vector3 cohesion(List<Boid> boids)
	{
		Vector3 sum = Vector3.zero; // start with empty vector to accumulate all positions
		int count = 0;

		for (int i = 0; i < boids.Count; i++) 
		{
			float d = Vector3.Distance(transform.position, boids[i].transform.position);
			if ((d > 0) && (d < neighbourDistance)) 
			{
				sum = (sum + boids [i].transform.position); // add position
				count++;
			}
		}

		if (count > 0) {
			sum = (sum / count);

			return seek(sum); // Steer towards the position
		} 

		else 
		{
			return new Vector3 ();
		}
	}

	void checkPosition()
	{
		// Check X axis
		if (transform.position.x < -40)
		{
			tRend.Clear();
			position.x = 40;
			transform.position = new Vector3 (position.x, transform.position.y, transform.position.z);
		}

		if (transform.position.x > 40)
		{
			tRend.Clear();
			position.x = -40;
			transform.position = new Vector3 (position.x, transform.position.y, transform.position.z);
		}

		// Check Y axis
		if (transform.position.y < -40)
		{
			tRend.Clear();
			position.y = 40;
			transform.position = new Vector3 (transform.position.x, position.y, transform.position.z);
		}

		if (transform.position.y > 40)
		{
			tRend.Clear();
			position.y = -40;
			transform.position = new Vector3 (transform.position.x, position.y, transform.position.z);
		}

		// Check Z Axis
		if (transform.position.z < -40)
		{
			tRend.Clear();
			position.z = 40;
			transform.position = new Vector3 (transform.position.x, transform.position.y, position.z);

		}

		if (transform.position.z > 40)
		{
			tRend.Clear();
			position.z = -40;
			transform.position = new Vector3 (transform.position.x, transform.position.y, position.z);
		}
	}
}