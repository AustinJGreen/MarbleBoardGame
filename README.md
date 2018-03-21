# MarbleBoardGame
Custom game based off of the rules and layout of [Aggravation](https://en.wikipedia.org/wiki/Aggravation_(board_game)). 

# How the game is played
- Goal is to get all 5 marbles to home holes.
- Roll 2 dice to determine how far to move your marbles.
- When moving your marble, you cannot hop over any of your own marbles.
- You may choose you split the numbers among your dice or use the total number rolled on one dice.
- You can bring out a marble with a 1 or a 6, by using that number to place the marble on your starting hole.
- You cannot capture your own marbles, and cannot go in other's bases.
- You may capture other player's marbles.
- If you roll doubles, you roll again.
- If you roll doubles 3 times consecutively, your lead marble that is not in your home base is killed.

# Overview
Currently, the game showcases the 4 ai's playing in its current state, but the source code can be easily altered to have a player play.
The back is rendered with MonoGame as a WindowsFormControl. Dice are rendered with the C# Physics Engine, Jitter. AI is alright, can only beat a random opponent 60-70% of the time. Algorithms implemented include, MaxN, Paranoid, MCTS, Offensive, and a MPMix. I implemented a stronger engine called Bach in C++ [here](https://github.com/ajosg/Bach).

# Screenshot
![alt text](https://i.imgur.com/gFieQVN.png)
