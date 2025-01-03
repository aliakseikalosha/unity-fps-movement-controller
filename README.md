# Unity Fps Movement Controller
A simple to setup and easy to edit unity fps controller using rigidbody movement in c#. Almost every variable used in this player controller is customisable and accessible from the inspector **(No programing required)**.

Feel free to use it in your own projects if you'd like!

[Trello Link](https://trello.com/b/maiUD4Te/unity-fps-movementcontroller) to follow the progress of the next update 🎉
## Features

- [x] Fully momentum based movement
- [x] Customizable speeds for:
	- [x] Walking
	- [x] Sprinting
	- [x] Crouching
	- [x] Air speed	
- [x] Jumping
	- [x] Raycast ground detection
	- [X] [Coyote time](https://github.com/B0N3head/unity-fps-movement-controller/#visual-examples)
	- [X] [Jump cooldown](https://github.com/B0N3head/unity-fps-movement-controller/#visual-examples)
	- [x] [Variable jump height](https://github.com/B0N3head/unity-fps-movement-controller/#visual-examples)
- [x] Crouching
	- [x] [Optional smoothed crouching using `lerp`](https://github.com/B0N3head/unity-fps-movement-controller/#visual-examples)
	- [x] Configurable to toggle or hold
	- [x] Allow crouching midair or on ground only
- [x] Inspector
	- [x] Simple named vars (easy to understand)
	- [x] All vars contain quick descriptions within tool tips
  	- [x] GUI based layer choice (choosing ground layer)
- [x] Camera 
	- [x] Variable smoothing of the camera
  	- [x] Clamped look directions
  	- [x] Variable sensitivity
  	- [x] Cursor lock/unlock
- [x] Configurable keys: (![See controls](https://github.com/B0N3head/unity-fps-movement-controller/#controls))
	- [x] Jumping
	- [x] Crouching
	- [x] Sprinting
	- [x] Lock/Unlocking mouse
- [x] Fast auto setup in one button button click
	- [x] Creates and sets up rigidbody
	- [x] Creates and sets up camera 
	- [x] Sets unity physics gravity to -19
		- [x] Ask user if they want to set custom gravity
	- [x] Ask and create "Ground" tag if it does not exist

### Visual examples
| Variable Jump Height & Jump Cooldown | Smoothed `lerp` Crouch vs Non-Smothed | Coyote Time *(Slowed & Exaggerated)* |
| ------ | ------ |------ |
| <img src="https://raw.githubusercontent.com/B0N3head/unity-fps-movement-controller/main/readmeAssets/varJumpHeight.gif" width="250" /> | <img src="https://raw.githubusercontent.com/B0N3head/unity-fps-movement-controller/main/readmeAssets/crouching.gif" width="250" /> | <img src="https://raw.githubusercontent.com/B0N3head/unity-fps-movement-controller/main/readmeAssets/coyoteTime.gif" width="250" /> |

<br/>

## Controls
This script is currently setup with these default controls.
These are all configurable from the editor. **(don't require any coding)**

| Key(s) | Action |
| ------ | ------ |
| WSAD | Simple character movement |
| Space | Jump |
| Shift | Sprint |
| Z* | Crouch |
| Q | Lock/Unlock Mouse |

###### *CTRL creates issues when testing in editor so Z is used (ctrl + most keys = will run unity hotkey)

<br/>

# How to setup the character

## Using unity package

1. Export unity package, from release.
2. User Prefab or setup as before.

## Using manual approach

1. Download and add the folder to your assets folder

<img src="https://raw.githubusercontent.com/aliakseikalosha/unity-fps-movement-controller/refs/heads/main/readmeAssets/1.PNG" width="600" />

2. Add a capsule to your scene

<img src="https://raw.githubusercontent.com/B0N3head/unity-fps-movement-controller/main/readmeAssets/2.PNG" width="600" />

3. Add the Player Movement script through Component -> Player Movement and Camera Controller

<img src="https://raw.githubusercontent.com/B0N3head/unity-fps-movement-controller/main/readmeAssets/3.PNG" width="600" />

4. Click the "Setup Player & World" button (I have not updated the old images yet, tho everything is in the same location)

<img src="https://raw.githubusercontent.com/B0N3head/unity-fps-movement-controller/main/readmeAssets/4.PNG" height="500" />

**Done**, you should now be left with something similar to the bellow

<img src="https://raw.githubusercontent.com/B0N3head/unity-fps-movement-controller/main/readmeAssets/5.PNG" width="600" />

### One more thing
**Please make sure that whatever the player is intended to stand/jump onto or from is set to the `Ground` tag.** 

When setting up the script, you will be prompted to let the script create the tag for you (I have not and prob wont create an algorythm/ai to detect what should and shouldn't be ground... so you will have to manually lable what should be ground).

<img src="https://raw.githubusercontent.com/B0N3head/unity-fps-movement-controller/main/readmeAssets/groundLayer.gif" width="600" />

