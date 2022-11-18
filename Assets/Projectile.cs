using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float damage = 5f;
	public float timeToLive = 2f;

	public Vector2 velocity { get; set; }
	public WeaponProjectile parent;
	public string faction = "";
	public string characterClass = "";
	
	public ParticleSystem particlesPrefab;

	private float wakeTimestamp = 0f;
	private Vector2 lastTracePosition;

	///////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		wakeTimestamp = Time.time;
		lastTracePosition = transform.position;
	}

	///////////////////////////////////////////////////////////////////////////
	private void Update()
	{
		transform.Translate(velocity * Time.deltaTime);
	}

	///////////////////////////////////////////////////////////////////////////
	private void FixedUpdate()
	{
		// Check if projectile lifetime expired.
		if (Time.time >= wakeTimestamp + timeToLive)
		{
			Destroy(gameObject);
			return;
		}

		Vector2 nextPos = lastTracePosition + (velocity * Time.fixedDeltaTime);
		int mask = ~LayerMask.GetMask("Projectile");

		// Projectiles are ignored.
		RaycastHit2D[] hits = Physics2D.LinecastAll(lastTracePosition, nextPos, mask);
		lastTracePosition = nextPos;

		// Check projectile path for collisions.
		foreach (RaycastHit2D hit in hits)
		{
			if (!ShouldCollide(hit.collider))
				continue;
			
			var victim = hit.collider.GetComponent<Character>();
			bool dealDamage = CanDealDamageTo(victim);

			if (dealDamage)
			{
				Character inflictor = parent ? parent.GetComponent<Character>() : null;

				victim.TakeDamage(damage, inflictor);
			}

			Color? color = dealDamage ? new Color(1f, 0.2f, 0.2f) : null;
			float rotation = Vector2.SignedAngle(hit.normal, Vector2.up);

			MakePoof(hit.point, rotation, color);

			Destroy(gameObject);
			return;
		}
	}

	///////////////////////////////////////////////////////////////////////////
	private void MakePoof(Vector2 position, float rotation = 0f, Color? color = null)
	{
		var parts = Instantiate(particlesPrefab);

		parts.transform.position = position;
		parts.transform.Rotate(0f, rotation, 0f);

		if (color != null)
		{
			var partSys = parts.GetComponentInChildren<ParticleSystem>().main;
			partSys.startColor = (Color)color;
		}
	}

	///////////////////////////////////////////////////////////////////////////
	private bool ShouldCollide(Collider2D collider)
	{
		if (!collider)
			return false;

		// Ignore colliding with our parent.
		return !parent || parent != collider.GetComponent<WeaponProjectile>();
	}

	///////////////////////////////////////////////////////////////////////////
	private bool CanDealDamageTo(Character victim)
	{
		if (!victim)
			return false;

		bool isDiffFaction = faction == "" || victim.faction != faction;
		bool isDiffClass = characterClass == "" || victim.characterClass != characterClass;

		return isDiffFaction || isDiffClass;
	}
}
