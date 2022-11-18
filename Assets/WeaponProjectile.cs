using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
	public GameObject projectilePrefab;
	public float damage = 5f;
	public float shotsPerSecond = 4f;
	public bool automatic = true;
	public int projectilesPerShot = 1;
	public float projectileSpeed = 40f;
	public float projectileStartOffset = 1f;
	public float projectileLifetime = 2f;
	public float spreadDegrees = 2f;

	private bool firing = false;
	private float nextShotTimestamp = 0f;

	public delegate void ShootEvent(Projectile[] projectiles);
	public event ShootEvent OnShoot;

	///////////////////////////////////////////////////////////////////////////
	public bool readyToFire
	{
		get { return Time.time >= nextShotTimestamp; }
	}

	///////////////////////////////////////////////////////////////////////////
	void Awake()
	{

	}

	///////////////////////////////////////////////////////////////////////////
	void Update()
	{
		if (firing)
		{
			if (readyToFire)
			{
				if (shotsPerSecond > 0f)
					nextShotTimestamp = Time.time + (1f / shotsPerSecond);

				Shoot();

				if (!automatic)
					firing = false;
			}
		}
	}

	///////////////////////////////////////////////////////////////////////////
	private void Shoot()
	{
		var projectiles = new List<Projectile>(projectilesPerShot);
		for (int i = 0; i < projectilesPerShot; i++)
		{
			GameObject obj = Instantiate(projectilePrefab,
				transform.position +
				(transform.up * projectileStartOffset),
				Quaternion.identity);

			if (!obj)
				return;

			var wielder = GetComponent<Character>();

            Projectile p = obj.GetComponent<Projectile>();
			p.parent = this;
            p.faction = wielder.faction;
            p.characterClass = wielder.characterClass;
            p.damage = damage;
            p.timeToLive = projectileLifetime;

			if (spreadDegrees > 0f)
			{
				float ang = Vector2.SignedAngle(Vector2.right, transform.up);
				ang += Random.Range(spreadDegrees * -0.5f, spreadDegrees * 0.5f);
				p.velocity = Ang2Vec2(ang) * projectileSpeed;
			}
			else
			{
				p.velocity = transform.up * projectileSpeed;
			}

			projectiles.Add(p);

        }

        OnShoot?.Invoke(projectiles.ToArray());
	}

	///////////////////////////////////////////////////////////////////////////
	public void StartFire()
	{
		firing = true;
	}

	///////////////////////////////////////////////////////////////////////////
	public void StopFire()
	{
		firing = false;
	}

	///////////////////////////////////////////////////////////////////////////
	private Vector2 Ang2Vec2(float deg)
	{
		float x = Mathf.Cos(deg * Mathf.Deg2Rad);
		float y = Mathf.Sin(deg * Mathf.Deg2Rad);

        return new Vector2(x, y);
	}
}
