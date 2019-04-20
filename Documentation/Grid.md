# Grid.cs #

### Summary: ### 

Base class to handle all grid verticies, dimensions, snapping/unsnapping, and grid manipulation. 

### Public Variables ###

##### Non-Inspector Parameters #####
  
```
public Vector3 To { get { return to; } set { to = value; } }
public Vector3 From { get { return from; } set { from = value; } }
public Vector3 GridOrigin { get { return gridOrigin} set {gridOrigin = value }
```
To & From and GridOrigin determine the width, height, and depth of the grid at the origin GridOrigin. 

##### Inspector Parameters #####

```
public GridRenderer gridRenderer;
public float CellSize;
```
    
A separate Game Component gridRenderer handles the Rendering of the Grid, both using the Unity Gizmos API, and a custom
script for grid rendering.
    
Cell size determines the dimension of each Grid Cell. 
    
### Private Variables ####

```
private Vector3[,] xStart;
private Vector3[,] xEnd;

private Vector3[,] yStart;
private Vector3[,] yEnd;

private Vector3[,] zStart;
private Vector3[,] zEnd;
```

These represent all verticies that each line in the grid will connect to, respectively, on the x, y, and z axis. 

```
private int xStartLength;
private int yStartLength;
private int zStartLength;

private int xEndLength;
private int yEndLength;
private int zEndLength;
```
These represent the lengths of each x length, y length, and z length, starting from, respectively, the XY plane, the YZ plane, and XZ plane.

```
private int xDimension;
private int yDimension;
private int zDimension;
```

These represent the numbers of cells on each x, y, and z direction of the grid. 

```
private GridVerticiesPayload g;
```
The Grid Verticies Payload represents a set of grid data which is sent to the GridRenderer Game Component for rendering.

### Public Events ###


### Public Configuration Methods ###

**ConfigureGrid()** : Initializes the grid dimensions, cell size, verticies, and prepares the Grid Payload to send to the Grid Renderer.

**RenderGrid()** : Makes a call to the Grid Renderer Game Component renderer method, UpdateRender() to render the Grid, based on data sent by the Grid Payload argument. 

### Pubblic Collider Methods ###

### Public Grid Interaction Methods ###

### Private Helper Methods ### 

**calculateGridLengths()** : Calculates the start and end lengths of each grid line, based on the given public variables Vector3 from, Vector3 to, and Vector3 origin. 

**calculateGridDimensions()** : Set the grid unit width, height, and depth of the grid, based on the values of the start and end lengths of each of the grid lines.

**generateGridVerticies()** : Populate the start and ends of each of the grid lines with vertice coordinaties, based on the start and end lengths of the grid. 
