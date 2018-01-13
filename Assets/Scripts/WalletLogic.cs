using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletLogic : MonoBehaviour {
	

	[SerializeField] GameHandler handler;
	[SerializeField] List<BoxCollider2D> pocketColliders;

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {
        List<float> distances = new List<float>();
        foreach (BoxCollider2D pocketCollider in pocketColliders)
        {
            distances.Add(Vector3.Distance(transform.position, pocketCollider.transform.position));
        }

        distances.Sort((d1, d2) => d1.CompareTo(d2));
        handler.UpdateDistance(distances[0]);
       

    }
}
