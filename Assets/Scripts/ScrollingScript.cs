using System.Collections;
using System.Linq;
using UnityEngine;


/// <summary>
/// Parallax scrolling script that should be assigned to a layer
///
/// This is related to the tutorial http://pixelnest.io/tutorials/2d-game-unity/parallax-scrolling/
///
/// See the result: http://pixelnest.io/tutorials/2d-game-unity/parallax-scrolling/-img/multidir_scrolling.gif
/// </summary>
public class ScrollingScript : MonoBehaviour
{
	/// <summary>
	/// Scrolling speed
	/// </summary>
	public Vector2 speed = new Vector2(10, 10);
	
	/// <summary>
	/// Moving direction
	/// </summary>
	public Vector2 direction = new Vector2(-1, 0);
	
	/// <summary>
	/// Movement should be applied to camera
	/// </summary>
	public bool isLinkedToCamera = false;
	
	/// <summary>
	/// Background is inifnite
	/// </summary>
	public bool isLooping = false;
	
	private ArrayList backgroundPart;
	private Vector2 repeatableSize;

	
	//GameControllerScript controller;
	
	
	void Start()
	{
	
		// For infinite background only
		if (isLooping)
		{
			//---------------------------------------------------------------------------------
			// 1 - Retrieve background objects
			// -- We need to know what this background is made of
			// -- Store a reference of each object
			// -- Order those items in the order of the scrolling, so we know the item that will be the first to be recycled
			// -- Compute the relative position between each part before they start moving
			//---------------------------------------------------------------------------------
			
			// Get all part of the layer
			backgroundPart = new ArrayList();//Transform
			
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				// Only visible children
				if (child.renderer != null)
				{
					backgroundPart.Add(child);
				}
			}

			if (backgroundPart.Count == 0)
			{
				Debug.LogError("Nothing to scroll!");
			}
			else {
			
				// Sort by position 
				// -- Depends on the scrolling direction
				//Debug.Log(" Rendering " + backgroundPart.Count + " parts ");
				
				backgroundPart.Sort(new SortTransformByAxisX());
				//OrderBy(t => t.position.x * (-1 * direction.x)).ThenBy(t => t.position.y * (-1 * direction.y)).ToList();
				
				
				int size = backgroundPart.Count;
				// Get the size of the repeatable parts
				Transform first = (Transform)backgroundPart[0];
				Transform last = (Transform)backgroundPart[size-1];
				
				repeatableSize = new Vector2(
					Mathf.Abs(last.position.x - first.position.x),
					Mathf.Abs(last.position.y - first.position.y)
					);
			}
			
			
		}
	}
	
	/**
	* Need to implement this due to issues with generics and Linq on IOS (JIT vs AOT compilation)
	**/
	private class SortTransformByAxisX : IComparer {
		int IComparer.Compare(object a, object b) {
			Transform t1=(Transform)a;
			Transform t2=(Transform)b;
			if (t1.position.x > t2.position.x)
				return 1;
			if (t1.position.x < t2.position.x)
				return -1;
			else
				return 0;
		}
	}
	
	public static IComparer SortTransformByAxisXAscending()
	{      
		return (IComparer) new SortTransformByAxisX();
	}
	
	void Update()
	{
		// Movement
		Vector3 movement = new Vector3(
			speed.x * direction.x,
			speed.y * direction.y,
			0);
		
	 //enableScroll = controller.IsGameStarted();
	
	 
		movement *= Time.deltaTime;
		transform.Translate(movement);
		
		// Move the camera
		if (isLinkedToCamera)
		{
			Camera.main.transform.Translate(movement);
		}
		
		// Loop
		if (isLooping)
		{
			//---------------------------------------------------------------------------------
			// 2 - Check if the object is before, in or after the camera bounds
			//---------------------------------------------------------------------------------
			
			// Camera borders
			var dist = (transform.position - Camera.main.transform.position).z;
			float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
			float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
			float width = Mathf.Abs(rightBorder - leftBorder);
			var topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
			var bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
			float height = Mathf.Abs(topBorder - bottomBorder);
			
			// Determine entry and exit border using direction
			Vector3 exitBorder = Vector3.zero;
			Vector3 entryBorder = Vector3.zero;
			
			if (direction.x < 0)
			{
				exitBorder.x = leftBorder;
				entryBorder.x = rightBorder;
			}
			else if (direction.x > 0)
			{
				exitBorder.x = rightBorder;
				entryBorder.x = leftBorder;
			}
			
			if (direction.y < 0)
			{
				exitBorder.y = bottomBorder;
				entryBorder.y = topBorder;
			}
			else if (direction.y > 0)
			{
				exitBorder.y = topBorder;
				entryBorder.y = bottomBorder;
			}
			
			// Get the first object
			Transform firstChild = backgroundPart.Count>0 ? (Transform)backgroundPart[0] : null;//.FirstOrDefault()
			
			if (firstChild != null)
			{
				bool checkVisible = false;
				
				// Check if we are after the camera
				// The check is on the position first as IsVisibleFrom is a heavy method
				// Here again, we check the border depending on the direction
				if (direction.x != 0)
				{
					if ((direction.x < 0 && (firstChild.position.x < exitBorder.x))
					    || (direction.x > 0 && (firstChild.position.x > exitBorder.x)))
					{
						checkVisible = true;
					}
				}
				if (direction.y != 0)
				{
					if ((direction.y < 0 && (firstChild.position.y < exitBorder.y))
					    || (direction.y > 0 && (firstChild.position.y > exitBorder.y)))
					{
						checkVisible = true;
					}
				}
				
				// Check if the sprite is really visible on the camera or not
				if (checkVisible)
				{
					//---------------------------------------------------------------------------------
					// 3 - The object was in the camera bounds but isn't anymore.
					// -- We need to recycle it
					// -- That means he was the first, he's now the last
					// -- And we physically moves him to the further position possible
					//---------------------------------------------------------------------------------
					
					if (firstChild.renderer.IsVisibleFrom(Camera.main) == false)
					{
						// Set position in the end
						firstChild.position = new Vector3(
							firstChild.position.x + ((repeatableSize.x + firstChild.renderer.bounds.size.x) * -1 * direction.x),
							firstChild.position.y + ((repeatableSize.y + firstChild.renderer.bounds.size.y) * -1 * direction.y),
							firstChild.position.z
							);
						
						// The first part become the last one
						backgroundPart.Remove(firstChild);
						backgroundPart.Add(firstChild);
					}
				}
			}
			
		}
		
	}
  
}

