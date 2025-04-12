# TileTrek

![tileTrek Demo](https://i.imgur.com/vMZ36lG.gif)

**Unity tool for building grid-based games with pathfinding and AI.**

## Features

- Custom editor with a palette for placing tiles:  
  - **Red**: Obstacle  
  - **Blue**: Enemy  
  - **Green**: Player  
  - **White**: Empty  
- Hover to view 3D grid coordinates of the GameObjects.  
- Click to move the Player (horizontal/vertical only, using A* pathfinding).  
- Player avoids obstacles/enemy; once arrived, the Enemy moves toward them.  
- The enemy stops adjacent to the Player.  
- Can only spawn one Player and one Enemy at a time.  
- Movement restricted to XZ plane.
