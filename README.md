# Capture the Flag AI


![Capture the Flag sample image](CTFimg.png)


Capture the Flag is a project I've been working on in 2018. I'm using it to practice modeling and to test computer player behavior. I've made all of the Assets so far and have written all of the code myself. The original purpose was to balance making it a complete game with testing new features, but now the purpose is to develop a neural network capable of playing capture the flag.


An old build from before the game's focus became neural networks can be found below. A new build will be uploaded when the neural network is stable enough to allow weight importing and exporting.


The current version contains a fully implemented neural network for determining the computer player actions. As of right now, the weights for the network cannot be imported or exported, rather, the CPU players always start with random weights and are trained whenever the human player takes part in a game. The current version is showing lots of promise. I've been able to successfully train the CPUs to obtain the flag at the enemy goal, however I haven't been able to get them to return to their own goal yet.


The neural network currently has 8 inputs, 3 outputs, 1 hidden layer and 3 nodes per layer. Each layer uses Sigmoid as the activation function. The structure will continue to change until the CPU players can pose a challenge to human players. The inputs and outputs are as follows:


Inputs:

Estimated flag offset (angle and distance)

Home goal offset (angle and distance)

Closest enemy offset (angle and distance)

Enemy has flag (0 or 1)

Carrying flag (0 or 1)


Outputs:

Angle to turn (0 to 1, -180 to 180)

Jump (0 or 1)

Attack (0 or 1)


#### Old Build:

[Capture the Flag 0.0.8](CurrentBuild/CTF_008.zip)


#### Controls:

WASD to Move

Space to Jump

Mouse controls the Camera

Click or CTRL to Attack

Q or E to Drop the Flag

M to Toggle the Mouse

ESC to Quit the Game
