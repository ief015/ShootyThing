﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	public float MoveSpeed = 5f;

	private Rigidbody2D rbody;
	private Vector2 moveDir = Vector2.zero;
	private Vector2 lookDir = Vector2.up;
	private float rotationSpeed = 0f;

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Get the look direction. Alias for transform.up.
	/// </summary>
	public Vector2 lookDirection
	{
		get { return transform.up; }
	}

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Get the movement direction.
	/// </summary>
	public Vector2 moveDirection
	{
		get { return moveDir; }
	}

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Get this character's position.
	/// </summary>
	public Vector2 position
	{
		get { return rbody.position; }
	}

	///////////////////////////////////////////////////////////////////////////
	void Awake()
	{
		rbody = GetComponent<Rigidbody2D>();
		lookDir = transform.up;
	}

	///////////////////////////////////////////////////////////////////////////
	void FixedUpdate()
	{
		Vector2 move = moveDir * MoveSpeed;

		rbody.MovePosition(rbody.position + (move * Time.fixedDeltaTime));

		if (rotationSpeed == 0f)
		{
			rbody.SetRotation(Vector2.SignedAngle(Vector2.up, lookDir));
		}
		else
		{
			float ang = Vector2.SignedAngle(Vector2.up, lookDir);
			float rot = Mathf.MoveTowardsAngle(rbody.rotation, ang,
				rotationSpeed * Time.fixedDeltaTime);
			rbody.SetRotation(rot);
		}
	}

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Look in this direction. Direction is internally normalized.
	/// </summary>
	/// <param name="dir">Relative direction to look.</param>
	/// <param name="degreesPerSec">Rotation speed. If zero, rotation is instant.</param>
	public void LookTowardsDirection(Vector2 dir, float degreesPerSec = 0f)
	{
		lookDir = dir;
		rotationSpeed = degreesPerSec;

		if (!Mathf.Approximately(lookDir.sqrMagnitude, 1f))
			lookDir.Normalize();
	}

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Look in this direction.
	/// </summary>
	/// <param name="xdir">Relative x-direction to look.</param>
	/// <param name="ydir">Relative y-direction to look.</param>
	/// <param name="degreesPerSec">Rotation speed. If zero, rotation is instant.</param>
	public void LookTowardsDirection(float xdir, float ydir, float degreesPerSec = 0f)
	{
		LookTowardsDirection(new Vector2(xdir, ydir), degreesPerSec);
	}

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Look towards a point in worldspace.
	/// </summary>
	/// <param name="worldCoord">World coordinate.</param>
	/// <param name="degreesPerSec">Rotation speed. If zero, rotation is instant.</param>
	public void LookTowards(Vector3 worldCoord, float degreesPerSec = 0f)
	{
		Vector2 dir = worldCoord - transform.position;
		LookTowardsDirection(dir, degreesPerSec);
	}

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Look towards a transform.
	/// </summary>
	/// <param name="target">Target transform.</param>
	/// <param name="degreesPerSec">Rotation speed. If zero, rotation is instant.</param>
	public void LookTowards(Transform target, float degreesPerSec = 0f)
	{
		LookTowards(target.position, degreesPerSec);
	}

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Move in direction. Direction is clamped internally to a magnitude of 1.
	/// </summary>
	/// <param name="direction">Relative direction</param>
	public void Move(Vector2 direction)
	{
		moveDir = direction;

		if (moveDir.sqrMagnitude > 1)
			moveDir.Normalize();
	}

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Move in direction. Direction is clamped internally to a magnitude of 1.
	/// </summary>
	/// <param name="xdir">Relative x-direction. (-1 to +1)</param>
	/// <param name="ydir">Relative y-direction. (-1 to +1)</param>
	public void Move(float xdir, float ydir)
	{
		Move(new Vector2(xdir, ydir));
	}
}