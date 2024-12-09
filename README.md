https://unijacob.itch.io/week-4

### Description
A 3D bowling-like game where the the goal is to knock all of the pins in 2 or less ball-throws. There are 2 stages (where the 2nd one is harder), 
but more can be created with ease using only a text editor.

### Structure
The "Stage Manager" is the heart of the game: It's the component responsible for the game logic and the instansiation and positioning of other game-objects.

The (bowling) ball is a simple game object with a sphere-collider that is also scripted by [DragAndLaunch](https://github.com/UniJacob/Week4/blob/main/Assets/Scripts/DragAndLaunch.cs), which allows it to be aimed and thrown at the pins.

The pins are clones of a prefab with imported mesh, its mesh collider had to be set to to "convex" to allow RigidBody behavior. This caused them to slightly
lose some realism, but in a barely noticable manner.

In each stage, the positions of the pins are placed by parsing from a text file with specific syntax. For example, the following string represents 3 adjucant pins: "v x cv x v" where 'c' represents the center of the 
