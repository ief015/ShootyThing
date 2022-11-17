using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float Damage = 5f;
    public float TimeToLive = 2f;

	public Vector2 velocity { get; set; }
	public WeaponProjectile parent;
	public string Faction = "";
	public string CharacterClass = "";

	public ParticleSystem particlesPrefab;

	private float wakeTimestamp = 0f;

	///////////////////////////////////////////////////////////////////////////
	private void Start()
	{
		wakeTimestamp = Time.time;
	}

	///////////////////////////////////////////////////////////////////////////
	private void Update()
	{
		transform.Translate(velocity * Time.deltaTime);
	}

	///////////////////////////////////////////////////////////////////////////
	private void FixedUpdate()
	{
		Vector2 lastPos =
			transform.position - (Vector3)(velocity * Time.fixedDeltaTime);
		Vector2 nextPos =
			transform.position + (Vector3)(velocity * Time.fixedDeltaTime);

		// Projectiles are ignored
		RaycastHit2D[] hits = Physics2D.LinecastAll(
			lastPos, nextPos, ~(LayerMask.GetMask("Projectile")));
		foreach (RaycastHit2D hit in hits)
		{
			bool dealtDamage = false;
			if (TestCollision(hit.collider, ref dealtDamage))
			{
                Color? color = dealtDamage ? new Color(1f, 0.2f, 0.2f) : null;
                float rotation = Vector2.SignedAngle(hit.normal, Vector2.up);
                MakePoof(hit.point, rotation, color);
				Destroy(gameObject);
				return;
			}
		}

		if (Time.time >= wakeTimestamp + TimeToLive)
			Destroy(gameObject);
	}

	///////////////////////////////////////////////////////////////////////////
	private void MakePoof(Vector2 position, float rotation, Color? color)
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
    private bool TestCollision(Collider2D collider)
    {
		bool tmp = false;
		return TestCollision(collider, ref tmp);
    }

    ///////////////////////////////////////////////////////////////////////////
    private bool TestCollision(Collider2D collider, ref bool refDealtDamage)
    {
        var victim = collider.GetComponent<Character>();

        bool dealtDamage = false;

        if (victim)
        {
            bool isDiffFaction = Faction == "" || victim.Faction != Faction;
            bool isDiffClass = CharacterClass == "" || victim.CharacterClass != CharacterClass;

            if (isDiffFaction || isDiffClass)
            {
				Character inflictor = parent ? parent.GetComponent<Character>() : null;
                victim.TakeDamage(Damage, inflictor);
                dealtDamage = true;
            }
        }

		refDealtDamage = dealtDamage;

        // If we have a parent weapon,
        // ... check if we're hitting our parent character,
        // ... if so, ignore it
        if (!parent || parent != collider.GetComponent<WeaponProjectile>())
            return true; //> Physical contact happened

        // Continue testing any other collisions
        return false;
    }
}
