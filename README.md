GitHub username: Mr-Carrot0
repo: 
commit hash: 
commit url: 

## Controls
 move: WASD / Arrow Keys 
 jump: Space 

## Playtest

### movement

Slow constant Acceleration feels too slippery and Instant acceleration was too snappy therefore, acceleration method 3, Ease Acceleration was preferred. 



# ass copy

```yml
title: Game Feel - Context & Camera
src: https://canvas.ltu.se/courses/24405/assignments/201129?module_item_id=541430
```
## Game Feel - Context & Camera

### Assignment Description

This assignment is about exploring Context and Camera. The simulated space, and the perception of the simulated space through the camera.

The literature Swink, Steve. (2009). *Game feel: a game designer's guide to virtual sensation*. Covers context in chapter 8, and the camera sparingly throughout the book. Here are some of the camera examples P237-239, P248 & P265-268, P204-205 & P219

For this assignment you are expected to use a Joystick.

The book does not cover that many camera aspects, but **[this reference Links to an external site.](https://www.youtube.com/watch?v=C7307qRmlMI)** is quite extensive for 3rd person camera systems. It is a GDC talk of the camera system in the game "Journey" which covers many camera principles. In this assignment you will replicate some parts of that camera system, explained further in the specification below.

Here is a simple and incomplete example of the assignment. But it gives an idea of how to approach it.

<iframe title="Video player for Context_Camera.mp4" src="https://canvas.ltu.se/media_attachments_iframe/4661021" allowfullscreen="allowfullscreen" allow="fullscreen"></iframe>

### Assignment requirement specification:

**Implementation Specification**

3rd Person camera system with:

- Smoothly follow the character positional change. Not positional fixation to the character, but using either interpolation or proportional feedback loop to position the camera at the player position (camera cannot be in the character hierarchy).
- Smooth Camera orbit around the player based on joystick input.
	- Orbit must be both vertical and horizontal and include vertical limits.
- Slowly align the camera to aim in the characters movement direction if no player orbit input is applied.
- Obstacle avoidance by follow the "Whisker" ray-cast example from the reference video.
	- This includes pushing the camera to the sides, pushing it inwards when close to walls and pushing it upwards & inwards when the camera is close to the ground. (Shoot rays from player towards camera, not the other way around)
		- Small obstacles should not be included in the avoidance system.
- Add a "Hint" area, similar to the reference game, where the camera is overridden with other behaviour and not strictly following the character.  
	This behaviour should smoothly change the camera position, fov, rotation etc. The camera should smoothly change back to target the character when exiting the area.

A level where:

- You showcase intentional design for: High - Medium and low level context.
	- Make the level feel complete, blocked in, in a whitebox manner.
		- Has small obstacles and large obstacles and is watertight in terms of collision.
		- Areas with "Hints" where you showcase the solution and the camera dynamics at work.
		- Supports the movement and the camera solution.

Add a simple character controller that can walk and jump. Make the movement support the showcase of the requirements above.

## Submission Requirements

**Playtest**

Do a simple playtest and note down:

- Explain the "High-Medium-Low" context of your level design using the provided terminology.
- Was the camera mechanics intuitive to the player?
- How could the camera mechanics be changed to support the exploration of the level or vice-versa?

Submit the notes to this assignment as plain text.

**Repository Reference**

- Also post your repository link and git username and a commit hash.

---

## Git Delivery

Create your own GitHub public repository for the assignment.

1. Commit, push and then **upload the url of your repository, the commit hash, *and* your Github username** to the Canvas submission. Make sure you don't commit any binaries (ex:.exe,.pdb) or temporary files.
	- Make sure you put the Godot project at the repository root and not in a subfolder.
2. Exception to the rule: You must build and submit a game executable to \[PROJECT\_ROOT\]/bin/game.exe

A good idea is to re-clone your repository from Github into a separate folder after you've pushed, and make sure that it compiles and runs without errors - or ask a fellow student to test it. This way, you make sure that the assignment is completed and that the teacher or TA don't run into problems.

NOTE: Remember to comment your code thoroughly. Make sure you build, run and test your submission *before* handing it in.