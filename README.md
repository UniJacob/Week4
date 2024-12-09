https://unijacob.itch.io/week-4

### Description
A 3D bowling-like game where the the goal is to knock all of the pins in 2 or less ball-throws. There are 2 stages (where the 2nd one is harder), 
but more can be created with ease using only a text editor.

### Structure
The "Stage Manager" is the heart of the game: It's the component responsible for the game logic and the instansiation and positioning of other game-objects.

The (bowling) ball is a simple game object with a sphere-collider that is also scripted by [DragAndLaunch](https://github.com/UniJacob/Week4/blob/main/Assets/Scripts/DragAndLaunch.cs), which allows it to be aimed and thrown at the pins.

The pins are clones of a prefab with imported mesh, its mesh collider had to be set to "convex" to allow RigidBody behavior. This caused them to slightly
lose some realism, but in a barely noticable manner.

In each stage, the arrangement (positions) of the pins is determined by parsing from a "stage text file" with specific syntax. For example, the following string represents 3 adjucant pins: "v x cv x v", where 'c' represents the center of the arrangement. Similarily, the following string represents a column of 3 pins but without a center pin: <br />v<br />cx<br />v

## Weight And Friction Parameters
These were taken from the real world - for example: An average bowling ball weighs about 6kg and a bolwing pin 1.53kg. There for in the game the RigidBody of the ball has "Mass = 6" and a pin has "Mass = 1.53".

