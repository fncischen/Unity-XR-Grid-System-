# Grid.cs #

### Summary: ### 

Base class to handle all grid verticies, dimensions, snapping/unsnapping, and grid manipulation. 

#### Public Variables ####

  * ###### Non-Inspector Parameters ######
  
    ```
    public Vector3 To { get { return to; } set { to = value; } }
    public Vector3 From { get { return from; } set { from = value; } }
    public Vector3 GridOrigin { get { return gridOrigin} set {gridOrigin = value }
    ```
    To & From and GridOrigin determine the width, height, and depth of the grid at the origin GridOrigin. 

 * ###### Inspector Parameters ######

    ```
    public GridRenderer gridRenderer;
    public float CellSize;
    ```
    
    A separate Game Component gridRenderer handles the Rendering of the Grid, both using the Unity Gizmos API, and a custom
    script for grid rendering.
    
    Cell size determines the dimension of each Grid Cell. 
    
#### Private Variables ####

    
    ```
    private Vector3[,] xStart;
    private Vector3[,] xEnd;

    private Vector3[,] yStart;
    private Vector3[,] yEnd;

    private Vector3[,] zStart;
    private Vector3[,] zEnd;
    ```

    ```
    private int xStartLength;
    private int yStartLength;
    private int zStartLength;

    private int xEndLength;
    private int yEndLength;
    private int zEndLength;
    ```

    private int xDimension;
    private int yDimension;
    private int zDimension;
    
    ```
    private GridVerticiesPayload g;
    ```
