using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
	private Character target;
	//private bool awake = false;
	//private float wakeTimeout = 60f;
	//private float sleepTimestamp = 0f;
	private float awarenessSize = 50f;

	private bool seesTarget = false;
	private Vector2 targetLastSeenPosition = Vector2.zero;
	private bool targetLastSeenPositionVisited = false;

	private Character self;
	private MovementController movement;
	private WeaponProjectile weapon;

	public float fieldOfView = 180f;
	public float travelDistance = 2f;
	public float wanderTimeSeconds = 20f;
	public bool allowInfighting = true;

	public bool isMoving { get; private set; }
	public Vector2 moveDestination { get; private set; }
	private float moveStopTimestamp = 0f;
	private float moveMinDistance = 0f;
	private float wanderTimeout = 0f;

	[Range(0f,1f)]
	public float aggression = 0.25f;


	///////////////////////////////////////////////////////////////////////////
	void Awake()
	{
		self     = GetComponent<Character>();
		movement = GetComponent<MovementController>();
		weapon   = GetComponent<WeaponProjectile>();

		isMoving = false;
		moveDestination = Vector2.zero;

		self.OnDamaged += OnDamaged;
		weapon.OnShoot += OnShoot;
    }

	///////////////////////////////////////////////////////////////////////////
	void Start()
	{
		const float THINK_RATE = 0.2f;
		InvokeRepeating("Think", Random.Range(0f, THINK_RATE), THINK_RATE);
	}

	///////////////////////////////////////////////////////////////////////////
	void OnDrawGizmos()
	{
		//Gizmos.color = awake ? (target ? Color.green : Color.yellow) : Color.red;
		Gizmos.color = target ? Color.green : Color.yellow;
		Gizmos.DrawSphere(transform.position + new Vector3(-0.2f, 0.2f), 0.1f);

		if (isMoving)
		{
			Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
			Gizmos.DrawLine(transform.position, moveDestination);
		}
	}

	///////////////////////////////////////////////////////////////////////////
	void Update()
	{
		
	}

	///////////////////////////////////////////////////////////////////////////
	void FixedUpdate()
	{
		//if (!CheckAwake())
		//	return;

		DoMovement();

		if (!target)
		{
			seesTarget = false;
			return; // Sleep until next target is found
		}

		if (seesTarget = CanSee(target))
		{
			movement.LookTowards(target.transform, 360f);
			targetLastSeenPosition = target.position;
			targetLastSeenPositionVisited = false;

			// Reset wake timer while we can see target
			//ResetSleepTimeout();
		}

		if (isMoving)
		{
			if (!targetLastSeenPositionVisited)
			{
				if ((targetLastSeenPosition - self.position).sqrMagnitude < 1f)
				{
					targetLastSeenPositionVisited = true;
					wanderTimeout = Time.time + wanderTimeSeconds;
                }
			}
		}
	}

	///////////////////////////////////////////////////////////////////////////
	void Think()
	{
		if (!target)
		{
			StopMoving();
			weapon.StopFire();

            if (FindTarget(false))
            {
                //Debug.Log(self + " found target: " + target);
            }

            return;
		}

		if (seesTarget)
		{
			if (weapon.readyToFire)
			{
				if (Random.value <= aggression)
				{
					StopMoving();
					weapon.StartFire();
				}
			}
		}
		else
		{
			if (!seesTarget)
			{
				weapon.StopFire();
			}

			if (!isMoving)
			{
				if (targetLastSeenPositionVisited)
				{
					if (Time.time < wanderTimeout)
                    {
                        // Wander a bit, as if searching for target nearby
                        Wander();
					}
					else
					{
						// Target lost
						ClearTarget();
					}
				}
				else
				{
					// Try to move to last known location
					MoveTowards(targetLastSeenPosition);
					movement.LookTowards(targetLastSeenPosition, 360f);
				}
			}
		}
	}

	///////////////////////////////////////////////////////////////////////////
	private void DoMovement()
	{
		if (isMoving)
		{
			if (Time.time > moveStopTimestamp)
			{
				// Give up trying to move to destination
				StopMoving();
			}
			else if ((moveDestination - self.position).sqrMagnitude
					 < moveMinDistance * moveMinDistance)
			{
				// Reached destination
				StopMoving();
			}
			else
			{
				// Keep moving
				movement.Move(moveDestination - self.position);
			}
		}
		else
		{
			movement.Move(Vector2.zero);
		}
	}

	///////////////////////////////////////////////////////////////////////////
	/*private bool CheckAwake()
	{
		if (awake)
		{
			// Stay awake until timeout period, otherwise do wake check.
			if (Time.time < sleepTimestamp)
				return true;
		}

		if (IsMoving)
		{
			// DISABLED: AI will never sleep because of this
			//ResetSleepTimeout();
			//return (awake = true);
		}

		Vector2 areaSz = new Vector2(awarenessSize, awarenessSize);

		Collider2D[] chars =
			Physics2D.OverlapAreaAll(movement.position - areaSz,
			movement.position + areaSz,
			LayerMask.GetMask("Character"));

		foreach (Collider2D col in chars)
		{
			Character ch = col.GetComponent<Character>();
			if (ch && ch.Faction != self.Faction)
			{
				ResetSleepTimeout();
				return (awake = true);
			}
		}

		target = null;
		return (awake = false);
	}*/

	///////////////////////////////////////////////////////////////////////////
	/*private void ResetSleepTimeout()
	{
		sleepTimestamp = Time.time + wakeTimeout;
	}*/

	///////////////////////////////////////////////////////////////////////////
	private Character FindTarget(bool searchOnly = false)
	{
		Vector2 areaSz = new Vector2(awarenessSize, awarenessSize);

		Collider2D[] chars =
			Physics2D.OverlapAreaAll(movement.position - areaSz,
			movement.position + areaSz, LayerMask.GetMask("Character"));

		foreach (Collider2D col in chars)
		{
			Character ch = col.GetComponent<Character>();
			if (ch && ch.faction != self.faction)
			{
				if (CanSee(ch))
				{
					if (!searchOnly)
					{
						SetTarget(ch);
					}
					return ch;
				}
			}
		}

		return null;
    }

    ///////////////////////////////////////////////////////////////////////////
    public void SetTarget(Character ch)
    {
        targetLastSeenPosition = ch.position;
        targetLastSeenPositionVisited = false;
        target = ch;
    }

    ///////////////////////////////////////////////////////////////////////////
    public void ClearTarget()
    {
        target = null;
    }

    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Check if a character 'ch' can be seen.
    /// Returns true if 'ch' is in line of sight.
    /// Field of view is also checked if 'ch' is not the current target.
    /// </summary>
    /// <param name="ch">Target</param>
    /// <returns></returns>
    public bool CanSee(Character ch)
	{
		return ((target != ch) || InFieldOfView(ch)) && InLineOfSight(ch);
	}

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Check if target is in line of sight.
	/// Returns true if line of sight in unobstructed.
	/// </summary>
	/// <param name="ch">Target</param>
	/// <returns></returns>
	public bool InLineOfSight(Character ch)
	{
		int mask = ~LayerMask.GetMask("Character");
        RaycastHit2D hit = Physics2D.Linecast(self.position, ch.position, mask);

		return hit.collider == null || hit.collider.transform == ch.transform;
	}

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Check if target is in field of view.
	/// Returns true if target is "in front" of
	/// this character relative to field of view angle.
	/// </summary>
	/// <param name="ch"></param>
	/// <returns></returns>
	public bool InFieldOfView(Character ch)
	{
		// Check if target outside field of view
		float ang = Vector2.SignedAngle(movement.lookDirection,
			ch.position - self.position);
		if (Mathf.Abs(ang) > fieldOfView * 0.5f)
			return false;
		return true;
	}

	///////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// AI will move this character towards this location.
	/// </summary>
	/// <param name="destination">Target destination.</param>
	/// <param name="minDistance">Minimum distance required before stopping.</param>
	/// <param name="timeout">Seconds until giving up and stopping.
	/// If 0, actor will only stop moving until it has reached the destination.
	/// If < 0, timeout will be automatically calculated based on move speed and destination distance.
	/// </param>
	public void MoveTowards(Vector2 destination, float minDistance = 0.5f, float timeout = -1f)
	{
		if (movement.moveSpeed <= 0f)
			return;

		if (timeout < 0f)
		{
			float dist = (destination - self.position).magnitude;
			float speed = movement.moveSpeed;
			timeout = dist / speed * 2f; // double for leeway
		}
		else if (timeout == 0f)
		{
			moveStopTimestamp = float.PositiveInfinity;
        }

        moveDestination = destination;
		moveMinDistance = Mathf.Max(0f, minDistance);
		moveStopTimestamp = Time.time + timeout;

		movement.LookTowards(moveDestination, 360f);

		isMoving = true;
    }

    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Move towards a random nearby spot.
    /// </summary>
    public void Wander()
    {
        MoveTowards(self.position
            + Random.insideUnitCircle.normalized * travelDistance,
            0.5f, Random.Range(0.25f, 1f));
    }

    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Move towards the target a little bit.
    /// </summary>
    public void AdvanceTowardsTarget()
    {
        MoveTowards(targetLastSeenPosition
            + Random.insideUnitCircle.normalized * travelDistance,
            0.5f, Random.Range(0.25f, 1f));
    }

    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Stop moving. Crazy, right?
    /// </summary>
    public void StopMoving()
	{
		isMoving = false;
	}

	///////////////////////////////////////////////////////////////////////////
	public void OnCollisionEnter2D(Collision2D collision)
    {
        //if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Character")) != 0)
        //{
        //    Debug.Log(LayerMask.LayerToName(collision.gameObject.layer));
        //    return;
		//}

		if (isMoving)
		{
			Vector2 norm = collision.GetContact(0).normal;
			Vector2 reflect = Vector2.Reflect(movement.moveDirection, norm);

			// Bumping into something, move around a bit
			MoveTowards(self.position + reflect * travelDistance,
				0.5f, Random.Range(0.2f, 0.4f));
		}
	}

	///////////////////////////////////////////////////////////////////////////
	private void OnDamaged(float damage, Character inflictor)
	{
		if (inflictor && allowInfighting)
		{
             SetTarget(inflictor);
        }
	}

    ///////////////////////////////////////////////////////////////////////////
    public void OnShoot(Projectile[] projectiles)
	{
		if (!weapon.automatic)
		{
            AdvanceTowardsTarget();
        }
    }
}
