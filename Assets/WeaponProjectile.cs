using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
	public GameObject ProjectilePrefab;
	public float Damage = 5f;
	public float ShotsPerSecond = 4f;
	public bool Automatic = true;
	public int ProjectilesPerShot = 1;
	public float ProjectileSpeed = 40f;
	public float ProjectileStartOffset = 1f;
	public float ProjectileLifetime = 2f;
	public float SpreadDegrees = 2f;

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
				if (ShotsPerSecond > 0f)
					nextShotTimestamp = Time.time + (1f / ShotsPerSecond);

				Shoot();

				if (!Automatic)
					firing = false;
			}
		}
	}

	///////////////////////////////////////////////////////////////////////////
	private void Shoot()
	{
		var projectiles = new List<Projectile>(ProjectilesPerShot);
		for (int i = 0; i < ProjectilesPerShot; i++)
		{
			GameObject obj = Instantiate(ProjectilePrefab,
				transform.position +
				(transform.up * ProjectileStartOffset),
				Quaternion.identity);

			if (!obj)
				return;

			var wielder = GetComponent<Character>();

            Projectile p = obj.GetComponent<Projectile>();
			p.parent = this;
            p.Faction = wielder.Faction;
            p.CharacterClass = wielder.CharacterClass;
            p.Damage = Damage;
            p.TimeToLive = ProjectileLifetime;

			if (SpreadDegrees > 0f)
			{
				float ang = Vector2.SignedAngle(Vector2.right, transform.up);
				ang += Random.Range(SpreadDegrees * -0.5f, SpreadDegrees * 0.5f);
				p.velocity = Ang2Vec2(ang) * ProjectileSpeed;
			}
			else
			{
				p.velocity = transform.up * ProjectileSpeed;
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
