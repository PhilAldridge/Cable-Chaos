# Cable-Chaos
## Introduction
Inspired by the 'flow free' series of games for android and the 'three utilities problem' in mathematical topology, I wanted to make a version that worked in 3D! You are given a 3D shape with coloured circles on its surface and the object of the game is to connect the matching pairs of colours by adding cables across the surface of the shape without any cables crossing paths. This game dynamically creates its own levels, has a leaderboard and user profiles. I used Unity to create it. See the completed game here: https://play.google.com/store/apps/details?id=com.PhilAldridge.CableChaos

## Further Information
This project contained several significant difficulties that I needed to overcome:
### Learning Unity and C#
I had very limited experience in both before starting this project and had to build up my knowledge in order to complete it. I achieved this through online tutorials and googling help on specific problems.
### Creating a relational list
A few years ago, I had followed a tutorial on how to create shapes using minecraft-style voxels. However, in order for the game to create cables across the surface of the shape, it was necessary to create a relational list so that every side of every voxel knew which sides were adjacent to it. This was not a problem that I could find an answer to online so I had to create my own algorithm for creating this list.
### Creating a solvable level
I knew that I wanted the game to create its own levels dynamically. In order for this to work, I had to create an algorithm with the following properties:
- Possible to connect all matching pairs without crossing cables
- Paths are long and wind around each other
- Random generation so that 1000s of different levels can be created

Here is a rough outline of the algorithm I ended up settling on:
1. Duplicate the relational list showing which sides are adjacent to one another.
2. Pick two random sides of a random voxel.
3. Find the shortest paths between the two sides and randomly choose one of these paths.
4. Delete the sides used in this path from the relational list so that other paths cannot cross this one.
5. Repeat steps 2-4 until there are no adjacent sides left in the list.
### Creating an API
I created a global database storing users' profiles and the leaderboard using SQL and hosted it on a website. I then created an API for the app to communicate with the database using PHP. The API has three functions: creating a new user id, updating the user's score and finding the user's current position on the leaderboard. 

![This is an image](https://play-lh.googleusercontent.com/DqAd36vGuSZoVKZcMXHdbfiLPbGKP4MRM6bR20fvILCbGDFUhjq8pIibD9AMX-ngmog=w5120-h2880)
