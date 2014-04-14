using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Grid manager class handles all the grid properties
public class GridManager : MonoBehaviour
{
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static GridManager s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static GridManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first GridManager object in the scene.
                s_Instance = FindObjectOfType(typeof(GridManager)) as GridManager;
                if (s_Instance == null)
                    Debug.Log("Could not locate an GridManager object. \n You have to have exactly one GridManager in the scene.");
            }
            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
        s_Instance = null;
    }

    #region Fields
    public int numOfRows;
    public int numOfColumns;
    public float gridCellSize;
    public bool showGrid = true;

    private Vector3 origin = new Vector3();
    public static List<GameObject> obstacleList = new List<GameObject>();
    public Node[,] nodes;
    public CrossNode[,] crossNode;
    public Dictionary<int, CrossNode> dict;
	public static Dictionary<int, Vector3> gridDict = new Dictionary<int, Vector3>();
	public static float [] valuesX = {1.47f, 1.87f, 2.27f, 2.67f, 3.07f, 3.47f};
	public static float [] valuesY = {1.04f, 1.44f, 1.84f, 2.24f, 2.64f, 3.04f};
    #endregion

    //Origin of the grid manager
    public Vector3 Origin
    {
        get { return origin; }
    }

    //Initialise the grid manager
    void Awake()
    {
        CreateGrid();
		CreateCrossNode();
        ResolveObstacles();
		MarkEdgeRoads();
		int x = 2;
		int z = 2;
		for(int i = 0; i < valuesX.Length ; i++)
		{
			for (int j = 0; j < valuesY.Length; j++)
			{ 
				int key = (int)(valuesX[i] * 1000f + valuesY[j]*10f); 
				int xTemp = x * (i + 1);
				int zTemp = z * (j + 1);
				Vector3 value = new Vector3(xTemp,1,zTemp);
				gridDict.Add (key,value);
			}
		}
    }

	public void ResolveObstacles()
    {
		DeleteObstacleFlags ();
		DeleteRoadFlags();
		MarkEdgeRoads();
		CalculateObstacles();
	}

	void DeleteObstacleFlags()
	{
		foreach(KeyValuePair<int, CrossNode> entry in dict)
		{
			Node[] cn = entry.Value.node;
			for(int j = 0; j < cn.Length; j++)
			{
				cn[j].bObstacle = false;
			}
		}
	}

	void MarkEdgeRoads(){
		for(int i = 0; i < numOfColumns; i++){
			nodes[i,0].isRoad = true;
		}

		for(int i = 0; i < numOfRows; i++){
			nodes[0,i].isRoad = true;
		}

		for(int i = 0; i < numOfColumns; i++){
			nodes[numOfColumns - 1,i].isRoad = true;
		}

		for(int i = 0; i < numOfRows; i++){
			nodes[i,numOfRows - 1].isRoad = true;
		}
	}

	void DeleteRoadFlags()
	{
		foreach(KeyValuePair<int, CrossNode> entry in dict)
		{
			Node[] cn = entry.Value.node;
			for(int j = 0; j < cn.Length; j++)
			{
				cn[j].isRoad = false;
			}
		}
	}

	public void ResetEstimatedCosts()
	{
		foreach(KeyValuePair<int, CrossNode> entry in dict)
		{
			Node[] cn = entry.Value.node;
			for(int j = 0; j < cn.Length; j++)
			{
				cn[j].nodeTotalCost = 1.0f;
			}
		}
	}


    void CreateCrossNode() 
    {
        dict = new Dictionary<int, CrossNode>();
        int col = numOfColumns - 1;
        int row = numOfRows - 1;
        crossNode = new CrossNode[col, row];
        for (int i = 0; i < col; i++)
        {
            for (int j = 0; j < row; j++)
            {
                Vector3 vec = new Vector3((float)(i + 1),0f,(float)(j + 1));
				CrossNode cn = new CrossNode(vec,nodes,dict);
				crossNode[i,j] = cn;
            }
        }
    }

    Node GetNode(Vector3 pos) 
    {
        int indexCell = GetGridIndex(pos);
        int col = GetColumn(indexCell);
        int row = GetRow(indexCell);
        return nodes[col, row];
    }

    /// <summary>
    /// Create the grid for squares
    /// </summary>
    void CreateGrid() 
    {
        //Initialise the nodes
        nodes = new Node[numOfColumns, numOfRows];

        int index = 0;
        for (int i = 0; i < numOfColumns; i++)
        {
            for (int j = 0; j < numOfRows; j++)
            {
                Vector3 cellPos = GetGridCellCenter(index);
                Node node = new Node(cellPos);
                nodes[i, j] = node;

                index++;
            }
        }
    }
    /// <summary>
    /// Calculate which cells in the grids are mark as obstacles
    /// </summary>
    void CalculateObstacles()
    {
		for (int i = 0; i < obstacleList.Count ; i++)
		{
			//Might throw a keynotfound -exception sometimes because transforms position might be momentarily off
			Vector3 vec = obstacleList[i].transform.position;
			int key = (int)vec.x * 100 + (int)vec.z;
			Node[] cn = dict[key].node;
			for(int j = 0; j < cn.Length; j++)
			{
				cn[j].bObstacle = true;
			}
		}
    }
    
    /// <summary>
    /// Returns position of the grid cell in world coordinates
    /// </summary>
    public Vector3 GetGridCellCenter(int index)
    {
        Vector3 cellPosition = GetGridCellPosition(index);
        cellPosition.x += (gridCellSize / 2.0f);
        cellPosition.z += (gridCellSize / 2.0f);
        return cellPosition;
    }

    /// <summary>
    /// Returns position of the grid cell in a given index
    /// </summary>
    public Vector3 GetGridCellPosition(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
        float xPosInGrid = col * gridCellSize;
        float zPosInGrid = row * gridCellSize;

        return Origin + new Vector3(xPosInGrid, 0.0f, zPosInGrid);
    }

    /// <summary>
    /// Get the grid cell index in the Astar grids with the position given
    /// </summary>
    public int GetGridIndex(Vector3 pos)
    {
        if (!IsInBounds(pos))
        {
            return -1;
        }

        pos -= Origin;

        int col = (int)(pos.x / gridCellSize);
        int row = (int)(pos.z / gridCellSize);

        return (row * numOfColumns + col);
    }

    /// <summary>
    /// Get the row number of the grid cell in a given index
    /// </summary>
    public int GetRow(int index)
    {
        int row = index / numOfColumns;
        return row;
    }

    /// <summary>
    /// Get the column number of the grid cell in a given index
    /// </summary>
    public int GetColumn(int index)
    {
        int col = index % numOfColumns;
        return col;
    }

    /// <summary>
    /// Check whether the current position is inside the grid or not
    /// </summary>
    public bool IsInBounds(Vector3 pos)
    {
        float width = numOfColumns * gridCellSize;
        float height = numOfRows* gridCellSize;

        return (pos.x >= Origin.x &&  pos.x <= Origin.x + width && pos.x <= Origin.z + height && pos.z >= Origin.z);
    }
		

    /// <summary>
    /// Get the neighour nodes in 4 different directions
    /// </summary>
    public void GetNeighbours(Node node, List<Node> neighbors)
    {
        Vector3 neighborPos = node.position;
        int neighborIndex = GetGridIndex(neighborPos);

        int row = GetRow(neighborIndex);
        int column = GetColumn(neighborIndex);

        //Bottom
        int leftNodeRow = row - 1;
        int leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //Top
        leftNodeRow = row + 1;
        leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //Right
        leftNodeRow = row;
        leftNodeColumn = column + 1;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //Left
        leftNodeRow = row;
        leftNodeColumn = column - 1;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
    }
	
	/// <summary>
	/// Check the neighbours inside the grid.
	/// </summary>
	/// <param name='row'>
	/// Row.
	/// </param>
	/// <param name='column'>
	/// Column.
	/// </param>
	/// <param name='neighbors'>
	/// Neighbors.
	/// </param>
    void AssignNeighbour(int row, int column, List<Node> neighbors)
    {
		if (row >= 0 && column >= 0 && row < numOfRows && column < numOfColumns)
        {
            Node nodeToAdd = nodes[row, column];

			neighbors.Add(nodeToAdd);
        } 
    }

    /// <summary>
    /// Show Debug Grids and obstacles inside the editor
    /// </summary>
    void OnDrawGizmos()
    {
        //Draw Grid
        if (showGrid)
        {
            DebugDrawGrid(transform.position, numOfRows, numOfColumns, gridCellSize, Color.blue);
        }
    }

    /// <summary>
    /// Draw the debug grid lines in the rows and columns order
    /// </summary>
    public void DebugDrawGrid(Vector3 origin, int numRows, int numCols, float cellSize, Color color)
    {
        float width = (numCols * cellSize);
        float height = (numRows * cellSize);

        // Draw the horizontal grid lines
        for (int i = 0; i < numRows + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 endPos = startPos + width * new Vector3(1.0f, 0.0f, 0.0f);
            Debug.DrawLine(startPos, endPos, color);
        }

        // Draw the vertial grid lines
        for (int i = 0; i < numCols + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 endPos = startPos + height * new Vector3(0.0f, 0.0f, 1.0f);
            Debug.DrawLine(startPos, endPos, color);
        }
    }
}
