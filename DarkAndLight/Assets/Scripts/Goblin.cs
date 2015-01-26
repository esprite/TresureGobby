﻿using UnityEngine;
using System.Collections;

public class Goblin : MonoBehaviour {
	float radius = 10.0f;
	public bool meactive;

	public GameObject pc; 

	public float moveSpeed = 2f;		// The speed the enemy moves at.
	public int HP = 2;					// How many times the enemy can be hit before it dies.
	public Sprite deadEnemy;			// A sprite of the enemy when it's dead.
	public Sprite damagedEnemy;			// An optional sprite of the enemy when it's damaged.
	public AudioClip[] deathClips;		// An array of audioclips that can play when the enemy dies.
	public GameObject hundredPointsUI;	// A prefab of 100 that appears when the enemy dies.
	public float deathSpinMin = -100f;			// A value to give the minimum amount of Torque when dying
	public float deathSpinMax = 100f;			// A value to give the maximum amount of Torque when dying

	bool grounded;
	private Animator anim;

	// private SpriteRenderer ren;			// Reference to the sprite renderer.
	private Transform frontCheck;		// Reference to the position of the gameobject used for checking if something is in front.
	private bool dead = false;			// Whether or not the enemy is dead.
	private Score score;				// Reference to the Score script.

	void Awake()
	{
		// Setting up the references.
		meactive = true;
		grounded = true;
		anim = GetComponent<Animator> ();
		pc = GameObject.FindGameObjectWithTag("Player");
		// ren = transform.Find("body").GetComponent<SpriteRenderer>();
		frontCheck = transform;
		// score = GameObject.Find("Score").GetComponent<Score>();
	}
	
	public void Hurt()
	{
		// Reduce the number of hit points by one.
		HP--;
	}
	
	void Death()
	{
		// Find all of the sprite renderers on this object and it's children.
		SpriteRenderer[] otherRenderers = GetComponentsInChildren<SpriteRenderer>();
		
		// Disable all of them sprite renderers.
		foreach(SpriteRenderer s in otherRenderers)
		{
			s.enabled = false;
		}
		
		// Re-enable the main sprite renderer and set it's sprite to the deadEnemy sprite.
		// ren.enabled = true;
		// ren.sprite = deadEnemy;
		
		// Increase the score by 100 points
		score.score += 100;
		
		// Set dead to true.
		dead = true;
		
		// Allow the enemy to rotate and spin it by adding a torque.s
		rigidbody2D.fixedAngle = false;
		rigidbody2D.AddTorque(Random.Range(deathSpinMin,deathSpinMax));
		
		// Find all of the colliders on the gameobject and set them all to be triggers.
		Collider2D[] cols = GetComponents<Collider2D>();
		foreach(Collider2D c in cols)
		{
			c.isTrigger = true;
		}
		
		// Play a random audioclip from the deathClips array.
		int i = Random.Range(0, deathClips.Length);
		AudioSource.PlayClipAtPoint(deathClips[i], transform.position);
		
		// Create a vector that is just above the enemy.
		Vector3 scorePos;
		scorePos = transform.position;
		scorePos.y += 1.5f;
		
		// Instantiate the 100 points prefab at this point.
		Instantiate(hundredPointsUI, scorePos, Quaternion.identity);
	}
	
	
	public void Flip()
	{
		// Multiply the x component of localScale by -1.
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}

	void FixedUpdate ()
	{
		Collider2D[] frontHits = Physics2D.OverlapPointAll(frontCheck.position, 0);

		foreach (Collider2D c in frontHits) {
			if (c.tag == "Wall") {
				Flip ();
				break;
			}
			else if (c.tag == "Obstacle") {
				transform.rigidbody2D.gravityScale = 0;
				grounded = true;
			}
			else if (c.tag == "Player" && Physics2D.GetIgnoreCollision(pc.GetComponent<CircleCollider2D>().collider2D, GetComponentsInParent<CircleCollider2D>()[0])) {

			}
		}

		if (pc != null && radiiOverlap() && pc.GetComponent<PlayerControl>().on && grounded) {	
			meactive = false;
			moveSpeed = 0f;
		}

		if (meactive)
		{	
			rigidbody2D.velocity = new Vector2(transform.localScale.x * moveSpeed, rigidbody2D.velocity.y);
			collider2D.enabled = true;
		} 
		else if (!meactive) 
		{
			anim.SetTrigger("Cower");
			collider2D.enabled = false;
		}

		// If the enemy has one hit point left and has a damagedEnemy sprite...
		if(HP == 1 && damagedEnemy != null)
			// ... set the sprite renderer's sprite to be the damagedEnemy sprite.
			// ren.sprite = damagedEnemy;
		
		// If the enemy has zero or fewer hit points and isn't dead yet...
		if(HP <= 0 && !dead)
			// ... call the death function.
			Death ();
	}

	public bool radiiOverlap() {
		if (Mathf.Abs(pc.transform.position.x - transform.position.x) <= radius) {
			return true;
		}
		return false;
	}
}
