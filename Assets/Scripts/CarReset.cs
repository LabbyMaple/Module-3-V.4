﻿/*
This script is used for reseting the Car to the nearest Checkpoint
*/

using UnityEngine;
using System.Collections.Generic;

public class CarReset : MonoBehaviour
{
	public GameObject vehicle;
	public Transform spawnPoints;

	private DriftCamera m_DriftCamera;
	private int m_VehicleId;

	void Start () 
    {
		m_DriftCamera = GetComponent<DriftCamera>();
	}
	
	void Update () 
    {
		if (Input.GetKeyUp(KeyCode.R))
		{
			Transform vehicleTransform = vehicle.transform;
			vehicleTransform.rotation = Quaternion.identity;

			Transform closest = spawnPoints.GetChild(0);

			// Find the closest spawn point.
			for (int i = 0; i < spawnPoints.childCount; ++i)
			{
				Transform thisTransform = spawnPoints.GetChild(i);

				float distanceToClosest = Vector3.Distance(closest.position, vehicleTransform.position);
				float distanceToThis = Vector3.Distance(thisTransform.position, vehicleTransform.position);

				if (distanceToThis < distanceToClosest)
				{
					closest = thisTransform;
				}
			}

			// Spawn at the closest spawn point.
#if UNITY_EDITOR
			Debug.Log("Teleporting to " + closest.name);
#endif
			vehicleTransform.rotation = closest.rotation;

			// Try refining the spawn point so it's closer to the ground.
            // Here we assume there is only one renderer.  If not, looping over all the bounds could do the trick.
			var renderer = vehicleTransform.gameObject.GetComponentInChildren<MeshRenderer>();
            // A valid car must have at least one wheel.
			var wheel = vehicleTransform.gameObject.GetComponentInChildren<WheelCollider>(); 

			RaycastHit hit;
            // Boxcast everything except cars.
			if (Physics.BoxCast(closest.position, renderer.bounds.extents, Vector3.down, out hit, vehicleTransform.rotation, float.MaxValue, ~(1 << LayerMask.NameToLayer("Car"))) )
			{
				vehicleTransform.position = closest.position + Vector3.down * (hit.distance - wheel.radius);
			}
			else
			{
				Debug.Log("Failed to locate the ground below the spawn point " + closest.name);
				vehicleTransform.position = closest.position;
			}

			// Reset the velocity.
			var vehicleBody = vehicleTransform.gameObject.GetComponent<Rigidbody>();
			vehicleBody.linearVelocity = Vector3.zero;
			vehicleBody.angularVelocity = Vector3.zero;
		}
	}
}
