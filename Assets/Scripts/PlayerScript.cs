using UnityEngine;
using System.Collections;


public class PlayerScript : MonoBehaviour {

	public float jumpForce = 500f;			// Amount of force added when the player jumps.
	private bool jump = false;				// Condition for whether the player should jump.
	// Use this for initialization
	public bool facingRight = true;			// For determining which way the player is currently facing.
	private bool isAlive = false;
	private bool grounded = true;
	private bool isVisible;

	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 2f;				// The fastest the player can travel in the x axis.
	public float moveSpeed = 0.5f;

	private int coins = 0;

	public Transform groundCheck;
	private GameControllerScript controller;
	private MoveScript moveScript;

	PickupCounterScript coinsCounter;

	private bool moveForward = false;
	private bool moveBackward = false;

	public float pushPower = 2.0F;
	private bool moving = false;
	private SpecialEffectsHelper specialEffects;
	
	//is the characther on a ladder? either can be up or down
	private bool onLadder = false;
	private bool isClibing = false; 
	private bool isDescending = false;
	
	//swipe directions
	//Vector2 firstPressPos;
	//Vector2 secondPressPos;
	//Vector2 currentSwipe;
	
	private static RuntimePlatform platform;
	public bool isMobilePlatform = true;
	
	//swipe 2nd version
	public enum Swipe { None, Up, Down, Left, Right };
	
	public float minSwipeLength = 200f;
	Vector2 firstPressPos;
	Vector2 secondPressPos;
	Vector2 currentSwipe;
	
	public static Swipe swipeDirection;

	void Start () {
	  isAlive = true;
	  grounded = false;
	  coins = 0;
	  isVisible = false;

	  GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
	  if(scripts!=null) {
		 controller = scripts.GetComponent<GameControllerScript>();
		 coinsCounter = scripts.GetComponent<PickupCounterScript>();
		 specialEffects = scripts.GetComponent<SpecialEffectsHelper>(); 
	  }

	  groundCheck = transform.Find("groundCheck");
	  moveScript = GetComponent<MoveScript>();

	  InvokeRepeating("ApplyDust",2f,3f);
	  isMobilePlatform = (platform == RuntimePlatform.Android) || (platform == RuntimePlatform.IPhonePlayer); 
	  
	}

	public void IncreaseCoins(int value) {
	  coins+=value;
	  if(coinsCounter!=null)
	    coinsCounter.AddPickup();

	}
	
	// Update is called once per frame

	void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  


      if(!isMobilePlatform) {
		
			
			float input = Input.GetAxis("Vertical");

			if(input!=0f) {
			
			  if(onLadder) {
			    if(input > 0f) {
				  StartClibing();
			    }
			    else {
			      StartDescending();
			    }
			  }
			  else if(input > 0f && grounded) {
			    Jump();
			  }
		    }
		    else {//no input
		      isClibing = false;
		      isDescending = false;  
		    }	
      }
	
	
	}

	public void Jump() {
	  if(grounded && !onLadder)
	    jump = true;
	}

	//to push something	
    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) {
            return;

        } else if(hit.collider.gameObject.GetComponent<PushableScript>()!=null) {
        //something i can push???
		 if (hit.moveDirection.y < -0.3F)
            return;
        
         Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
         body.velocity = pushDir * pushPower;

        }

    }

	void ApplyDust() {
	 if(moving && grounded) {
	   specialEffects.PlayDustEffect(transform.position);
	 }
	}

    public void SetOnLadder(bool on) {
      onLadder = on;
      if(!onLadder) {
		rigidbody2D.gravityScale = 1;
		isClibing = isDescending = false;
      }
    }
	public void StartClibing() {
	   onLadder = true;
	   isClibing = true;
	   isDescending = false;
	   rigidbody2D.gravityScale = 0;
	}
	
	public void StartDescending() {
		onLadder = true;
		isClibing = false;
		isDescending = true;
		rigidbody2D.gravityScale = 0;
	}
	
	public void StopClibing() {

		isClibing = false;
		rigidbody2D.gravityScale = 0;
		
	}
	
	public void StopDescending() {
		
		isClibing = false;
		rigidbody2D.gravityScale = 0;
		
	}
	
	
	
	public bool IsClibingLadder() {
	  return onLadder && isClibing;
	}
	
	public bool IsDescendingLadder() {
	  return onLadder && isDescending;
	}
	
	public bool IsOnLadder() {
	  return onLadder;
	}
	
	

	void FixedUpdate ()
	{

		//Vector2	movement = new Vector2(moveScript.speed.x * moveScript.direction.x,moveScript.speed.y * moveScript.direction.y);
				
				
			// 6 - Make sure we are not outside the camera bounds (player leaving scene)
			/*var dist = (transform.position - Camera.main.transform.position).z;
			
			var leftBorder = Camera.main.ViewportToWorldPoint(
				new Vector3(0, 0, dist)
				).x;
			
			var rightBorder = Camera.main.ViewportToWorldPoint(
				new Vector3(1, 0, dist)
				).x;
			
			var topBorder = Camera.main.ViewportToWorldPoint(
				new Vector3(0, 0, dist)
				).y;
			
			var bottomBorder = Camera.main.ViewportToWorldPoint(
				new Vector3(0, 1, dist)
				).y;
				
			transform.position = new Vector3(
				Mathf.Clamp(transform.position.x, leftBorder, rightBorder),
				Mathf.Clamp(transform.position.y, topBorder, bottomBorder),
				transform.position.z
				);*/

		//-------------------------------


		//------------------------------


		// If the player should jump...
		if(jump && !onLadder)
		{
		
			// Add a vertical force to the player.
		    rigidbody2D.AddForce(new Vector2(0f, jumpForce));
			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}
		
		if(onLadder) {

			  if(isClibing || isDescending) {

					float speed = isClibing ? moveSpeed : -moveSpeed;
					
					if(speed * rigidbody2D.velocity.y < maxSpeed)
						// ... add a force to the player.
						rigidbody2D.AddForce(Vector2.up * speed * moveForce);
					
					// If the player's vertical velocity is greater than the maxSpeed...
					if(Mathf.Abs(rigidbody2D.velocity.y) > maxSpeed)
						
						rigidbody2D.velocity = new Vector2(moving ? rigidbody2D.velocity.x : 0f, Mathf.Sign(rigidbody2D.velocity.y) * maxSpeed);
			// ... set the player's velocity to the maxSpeed in the x axis.
			}
			else {
			  Debug.Log("DO NOTHING!!!!");
				rigidbody2D.velocity = new Vector2(0f,0f);
				rigidbody2D.gravityScale = 0f;
			}  
		}
						
		// Cache the horizontal input.
		if(!isMobilePlatform) {

				float input =  Input.GetAxis("Horizontal");
				
				if(input!=0) {
					moving = true;
				}
				else {
					moving = false;
				}
				
				if(moving) {
					// The Speed animator parameter is set to the absolute value of the horizontal input.
					//anim.SetFloat("Speed", Mathf.Abs(h));
					
					// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
					if(input * rigidbody2D.velocity.x < maxSpeed)
						// ... add a force to the player.
						rigidbody2D.AddForce(Vector2.right * input * moveForce);
					
					// If the player's horizontal velocity is greater than the maxSpeed...
					if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
						// ... set the player's velocity to the maxSpeed in the x axis.
						rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
					
					// If the input is moving the player right and the player is facing left...
					if(input > 0 && !facingRight)
						// ... flip the player.
						Flip();
					// Otherwise if the input is moving the player left and the player is facing right...
					else if(input < 0 && facingRight)
						// ... flip the player.
						Flip();
				}
				
				
		  }
		  else {
				// The Speed animator parameter is set to the absolute value of the horizontal input.
				//anim.SetFloat("Speed", Mathf.Abs(h));
				
				// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
				if(moving) {

				    float speed = moveForward ? moveSpeed : -moveSpeed;
				    
					if(speed * rigidbody2D.velocity.x < maxSpeed)
						// ... add a force to the player.
						rigidbody2D.AddForce(Vector2.right * speed * moveForce);
					
					// If the player's horizontal velocity is greater than the maxSpeed...
					if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
						// ... set the player's velocity to the maxSpeed in the x axis.
						rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
					
					// If the input is moving the player right and the player is facing left...
					if(speed > 0 && !facingRight)
						// ... flip the player.
						Flip();
					// Otherwise if the input is moving the player left and the player is facing right...
					else if(speed < 0 && facingRight)
						// ... flip the player.
						Flip();
				
				} 
		  
		  }
		  

	  //rigidbody2D.velocity = movement;
	}

	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void KillPlayer() {
	  isAlive = false;
	  controller.EndGame(false);

	  PlayExplosion();

	  transform.parent.gameObject.AddComponent<GameOverScript>();
	  Destroy(gameObject);
	}

	void PlayExplosion() {
	  GameObject scripts = GameObject.FindGameObjectWithTag("Scripts");
	  if(scripts!=null) {
	    SpecialEffectsHelper effects = scripts.GetComponent<SpecialEffectsHelper>();
	    if(effects!=null) {
	      effects.PlayExplosionEffect(transform.position);
	    }
	  }
	}

	void OnBecameVisible() {
		
		isVisible = true;
		
	}

	void OnBecameInvisible() {

		
		if(isVisible) {
		  Vector3 cameraPosition = Camera.main.transform.position;
		  if(transform.position.x < cameraPosition.x) {
			KillPlayer();
		  }
		}
		   
		isVisible = false;
		
	}

	public void MoveBackward() {
	  moveBackward = true;
	  moveForward = false;
	  moving = true;
	}
	public void MoveForward() {
	  moveForward = true;
	  moveBackward =  false;
	  moving = true;
	}
	
	public bool IsMovingForward() {
	  return moveForward;
	}
	
	public bool IsMovingBackward() {
		return moveBackward;
	}

	public void PlayerStationary() {
	 moveForward = moveBackward = false;
	 moving = false;
	}

	public bool IsPlayerAlive(){
	  return isAlive;
	}
}
/*
---

	//inside class
	Vector2 firstPressPos;
Vector2 secondPressPos;
Vector2 currentSwipe;

public void Swipe()
{
	if(Input.touches.Length > 0)
	{
		Touch t = Input.GetTouch(0);
		if(t.phase == TouchPhase.Began)
		{
			//save began touch 2d point
			firstPressPos = new Vector2(t.position.x,t.position.y);
		}
		if(t.phase == TouchPhase.Ended)
		{
			//save ended touch 2d point
			secondPressPos = new Vector2(t.position.x,t.position.y);
			
			//create vector from the two points
			currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
			
			//normalize the 2d vector
			currentSwipe.Normalize();
			
			//swipe upwards
			if(currentSwipe.y > 0  currentSwipe.x > -0.5f  currentSwipe.x < 0.5f)
			{
				Debug.Log("up swipe");
			}
			//swipe down
			if(currentSwipe.y < 0  currentSwipe.x > -0.5f  currentSwipe.x < 0.5f)
			{
				Debug.Log("down swipe");
			}
			//swipe left
			if(currentSwipe.x < 0  currentSwipe.y > -0.5f  currentSwipe.y < 0.5f)
			{
				Debug.Log("left swipe");
			}
			//swipe right
			if(currentSwipe.x > 0  currentSwipe.y > -0.5f  currentSwipe.y < 0.5f)
			{
				Debug.Log("right swipe");
			}
		}
	}
}*/
