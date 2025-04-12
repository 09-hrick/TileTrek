using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Window for Editing the 10*10 grid in the unity Editor 
/// </summary>
public class GridEditor : EditorWindow
{
    //reference to the asset contating grid data
    private GridSpawnData spawingDataAsset;
    
    // Current selected cell type from the pallet
    private CellType SelectedCellType = CellType.EmptyCell;
    
    // Asset path for loading and saving or creating grid data
    private const string dataAssetPath = "Assets/Resources/new GridSpawnData.asset";

    //size of the each cell  in pixel
    private int CelSize = 40;

    
    /// Adds new menu item under "Window" tab to open this editor
    [MenuItem("Window/Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<GridEditor>("Grid Editor");
    }

    private void OnEnable()
    {
        // Load and reference the existing grid data asset
        spawingDataAsset = AssetDatabase.LoadAssetAtPath<GridSpawnData>(dataAssetPath);
        
        // if asset does not exists then create new one
        if(spawingDataAsset == null)
        {
            Debug.LogWarning("Missing GridSpawnData Asset Please add it to the following path\n" + dataAssetPath);
        }
        
    }

    void OnGUI()
    {
        GUILayout.Label("Click button in the Grid to after selecting the CellType from the pallet",EditorStyles.largeLabel);
        GUILayout.Space(3);
        
        // Draw the main 10*10 grid
        DrawGrid();
        GUILayout.Space(5);

        // Draw the pallet for selecting different cell type 
        GUILayout.Label("Pallet", EditorStyles.largeLabel);
        GUILayout.Space(3);
        DrawPallet();
        GUILayout.Space(5);

        // Reset button for reseting the whole grid back to empty
        if (GUILayout.Button("Reset Grid", GUILayout.Width(100), GUILayout.Height(40)))
        {
            spawingDataAsset.ResetGrid();
        }
        
    }

    /// <summary>
    /// Draws buttons for each type of cell
    /// </summary>
    private void DrawPallet()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        DrawPalletButton(CellType.EmptyCell);
        GUILayout.Space(3);
        GUILayout.Label("Empty");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        DrawPalletButton(CellType.Obstruction);
        GUILayout.Space(3);
        GUILayout.Label("Obstruction");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        DrawPalletButton(CellType.Player);
        GUILayout.Space(3);
        GUILayout.Label("Player");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        DrawPalletButton(CellType.Enemy);
        GUILayout.Space(3);
        GUILayout.Label("Enemy");
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    /// <summary>
    /// Draw the a single of specified color from cellType pallet buttons
    /// </summary>
    /// <param name="Type"></param>
    private void DrawPalletButton(CellType Type)
    {
        Color originalColor = GUI.color;
        Color color = GetColorForCell(Type);

        // Check if this is a cell is the selected cell, then highlight it with a high alpha value 
        float apha = (SelectedCellType == Type) ? 1f : 0.09f;
        GUI.color = new Color(color.r,color.g,color.b,apha);
        GUIContent content = new GUIContent("", Type.ToString());
        
        // Change the selected cell from the pallet 
        if (GUILayout.Button(content, GUILayout.Width(CelSize), GUILayout.Height(CelSize)))
        {
            SelectedCellType = Type;
        }
        GUI.color = originalColor;
    }


    /// <summary>
    /// Draws empty cells and also handles color switching using the SelectedCellType
    /// And populates the GridSpawnData asset
    /// </summary>
    /// <param name="x">x position on the Grid</param>
    /// <param name="z">z position on the Grid</param>
    private void DrawGridCell(int x,int z)
    {
        // Get color current cell type
        Color cellColor = GetColorForCell(spawingDataAsset.grid[x + z*10]);
        Color originalBgColor = GUI.backgroundColor; //get the oeiginal/Default GUI Background color
        GUI.backgroundColor = cellColor;

        // Draw the grid cell with square buttons of cellsize width and height in pixels
        if (GUILayout.Button("", GUILayout.Width(CelSize), GUILayout.Height(CelSize))) 
        {
            // To ensure only one player and enemy can be set throughout the grid
            if(SelectedCellType == CellType.EmptyCell || SelectedCellType == CellType.Obstruction)
            {
                spawingDataAsset.grid[x + z * 10] = SelectedCellType;
            }
            else if (SelectedCellType == CellType.Player)
            {
                // Clear previous player if any
                for(int i = 0; i < 100; i++)
                {
                    if (spawingDataAsset.grid[i] == CellType.Player)
                    {
                        spawingDataAsset.grid[i] = CellType.EmptyCell;
                    }
                }
                spawingDataAsset.grid[x + z * 10] = SelectedCellType;
            }
            else if(SelectedCellType == CellType.Enemy)
            {
                // Clear previous enemy if any 
                for (int i = 0; i < 100; i++)
                {
                    if (spawingDataAsset.grid[i] == CellType.Enemy)
                    {
                        spawingDataAsset.grid[i] = CellType.EmptyCell;
                    }
                }
                spawingDataAsset.grid[x + z * 10] = SelectedCellType;
            }
           
        }

        // Reset background color to avoid affecting other UI
        GUI.backgroundColor = originalBgColor; 
    }

    /// <summary>
    /// Draws the 10*10 Grid on the Editor using Draw cell
    /// </summary>
    private void DrawGrid()
    {
        for (int x = 0; x < 10; x++)
        {
            GUILayout.BeginHorizontal();
            for (int z = 0; z < 10; z++)
            {
                DrawGridCell(x,z);
            }
            GUILayout.EndHorizontal();
        }
    }


    /// <summary>
    /// Returns the corresponding color for a given CellType
    /// </summary>
    private Color GetColorForCell(CellType Type)
    {
        switch(Type){
            case CellType.Enemy: return Color.blue;
            case CellType.Player: return Color.green;
            case CellType.Obstruction: return Color.red;
            default: return Color.white;
        }
    }
}
