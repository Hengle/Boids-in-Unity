using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoidsCounter : MonoBehaviour {

	public Text displayNoBoids;
	public int noBoids;

	// Use this for initialization
	void Start () 
	{
		noBoids = BoidManager.getBoids ();
		displayNoBoids.text = noBoids.ToString ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		updateBoids ();
	}

	public void updateBoids()
	{
		noBoids = BoidManager.getBoids ();
		displayNoBoids.text = noBoids.ToString ();
	}
}
