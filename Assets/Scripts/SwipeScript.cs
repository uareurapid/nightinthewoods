﻿using UnityEngine;
using System.Collections;

public class SwipeScript : MonoBehaviour {
	
	
	private float fingerStartTime  = 0.0f;
	private Vector2 fingerStartPos = Vector2.zero;
	
	private bool isSwipe = false;
	private float minSwipeDist  = 50.0f;
	private float maxSwipeTime = 0.5f;
	
	
	private PlayerScript player;
	
	void Start()  {
	
	   GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
	   player = playerObj.GetComponent<PlayerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		float gestureTime;
		float gestureDist;
		
		if (Input.touchCount > 0){
			
			foreach (Touch touch in Input.touches)
			{
				switch (touch.phase)
				{
				case TouchPhase.Began :
					/* this is a new touch */
					isSwipe = true;
					fingerStartTime = Time.time;
					fingerStartPos = touch.position;
					break;
					
				case TouchPhase.Canceled :
					/* The touch is being canceled */
					isSwipe = false;
					break;
					
				case TouchPhase.Moved :
					/* The touch is being moved */
					isSwipe = true;
					
					gestureTime = Time.time - fingerStartTime;
					gestureDist = (touch.position - fingerStartPos).magnitude;
					
					if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist){
						Vector2 direction = touch.position - fingerStartPos;
						Vector2 swipeType = Vector2.zero;
						
						if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
							// the swipe is horizontal:
							swipeType = Vector2.right * Mathf.Sign(direction.x);
						}else{
							// the swipe is vertical:
							swipeType = Vector2.up * Mathf.Sign(direction.y);
						}
						
						if(swipeType.x != 0.0f){
							if(swipeType.x > 0.0f){
								// MOVE RIGHT
								player.MoveForward();
							}else{
								// MOVE LEFT
								player.MoveBackward();
							}
						}
						
						//
						if(swipeType.y != 0.0f ){
							if(swipeType.y > 0.0f){
								// MOVE UP
								if(player.IsDescendingLadder() || player.IsOnLadder()) {
								  player.StartClibing();
								}
								
							}else{
								// MOVE DOWN
								if(player.IsClibingLadder()) {
								  player.StartDescending();
								}//else crouch for instance??
							}
						}
						
						
					}
					break;
				case TouchPhase.Stationary :
					/* The touch is being moved */
					isSwipe = true;
					if(player.IsMovingBackward()) {
						player.MoveBackward();
					}
					else if(player.IsMovingForward()) {
						player.MoveForward();
					}
					break;
					
				case TouchPhase.Ended :
					
					gestureTime = Time.time - fingerStartTime;
					gestureDist = (touch.position - fingerStartPos).magnitude;
					
					if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist){
						Vector2 direction = touch.position - fingerStartPos;
						Vector2 swipeType = Vector2.zero;
						
						if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
							// the swipe is horizontal:
							swipeType = Vector2.right * Mathf.Sign(direction.x);
						}else{
							// the swipe is vertical:
							swipeType = Vector2.up * Mathf.Sign(direction.y);
						}
						
						if(swipeType.x != 0.0f){
							if(swipeType.x > 0.0f){
								// MOVE RIGHT
								player.MoveForward();
							}else{
								// MOVE LEFT
								player.MoveBackward();
							}
						}
						/*else {
						  player.PlayerStationary();
						}*/
						
						if(swipeType.y != 0.0f ){
							if(swipeType.y > 0.0f){
								// MOVE UP
								if(!player.IsOnLadder()) {
								  player.Jump();
								}
								else {
								  player.StartClibing();
								}
							}else{
								// MOVE DOWN
							}
						}
						
					}
					
					break;
				}
			}
		}
		else {
		  player.PlayerStationary();
		}
		
	}
}