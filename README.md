# Assignment 3: Antymology

This repository is an assignment for CPSC 565 (Emergent Computing) taken at the University of Calgary.

The objective of this assignment is to create a colony of ants that exhibit "intelligent" behaviour. The only goal of this assignment is to implement an evolutionary algorithm which maximizes nest production.

## The Simulation Environment
The world is made up of a number of worker ants and a single queen ant. Each ant can do the following:
- Move in any direction
- Eat mulch to restore health
- Donate health to a nearby ant
- Dig downwards
- Do nothing (Sometimes the only way to win the game is not to play)

Every "tick" of the simulation, each ant's health decreases by a pre-determined amount.

The queen ant can do everything the worker ant can do PLUS the ability to build nest blocks. However, building a nest block consumes 1/3 of the queens current health. Thus, the goal of building a nest becomes a balancing act between consuming health to build the nest and receiving enough health from other ants to stay alive.

## My Approach
To implement this system, I decided to use a neural network to determine the behaviors of the ants on every tick and an evolutionary algorithm to tweak the networks weights for the ants to "learn".

### Neural Network Ant Brains
Each tick the following inputs are fed into the neural network for each ant
- Ant's current health
- Number of Blocks Dug (or Number of Blocks built if it is the queen)
- Number of Mulch Eaten
- Amount of Health Donated
- Distance to Queen (or Distance to nearest neighbour if it is the queen)

My motivation for choosing these inputs, is that I hope for the ant's to eventually learn to maximize their own health while also donating health to others (more importantly donating health to the queen). I also hope that over time the ants learn to congregate around the queen (or maybe not, who knows what will happen, thats the fun of emergence).

The output layer corresponds to the set of all actions an ant can take. A value is generated between -1 and 1. These values are sorted, and the highest value decides the decision the ant will make. If the highest rated action is invalid (i.e. an ant is unable to move forward) then the next highest rated action will be taken, and so on and so forth.




## Controls

## Generated Data
