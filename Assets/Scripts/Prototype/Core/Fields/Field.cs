using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Field : MonoBehaviour
{
    // Constant value of the field sizes
    // No idea why I divided into width and height even though it's a square
    public const int FIELD_WIDTH = 21;
    public const int FIELD_HEIGHT = 21;
    public const int FIELD_SIZE = 21;

    [Header("Dependencies")]
    [SerializeField] private Transform bottomLeftCornerTransform;
    [SerializeField] private Transform bottomRightCornerTransform;
    protected GameController gameController;
    private SquareSelectorCreator squareSelector;

    [Header("Property (read-only)")]
    [SerializeField] private float squareSize;
    
    // Grid and selected unit
    private Unit[,] grid;
    public Unit selectedUnit;

    // Abstract methods
    #region Abstract
    public abstract void SetSelectedUnit(Vector2 coords);
    public abstract void SelectedUnitMoved(Vector2 coords);
    public abstract void SelectedUnitAttacked(Vector2 coords);
    #endregion

    // Initialize methods
    // Also include OnGameRestarted()
    #region Initialize
    protected virtual void Awake()
    {
        SetSquareSize();
        CreateGrid();
    }

    private void SetSquareSize()
    {
        squareSize = Mathf.Abs(bottomLeftCornerTransform.position.x - bottomRightCornerTransform.position.x) / (FIELD_SIZE - 1);
    }

    private void CreateGrid()
    {
        grid = new Unit[FIELD_WIDTH, FIELD_HEIGHT];
    }

    public void SetDependencies(GameController gameController, SquareSelectorCreator squareSelector)
    {
        this.gameController = gameController;
        this.squareSelector = squareSelector;
    }

    public void OnGameRestarted()
    {
        selectedUnit = null;
        CreateGrid();
    }
    #endregion

    // Logic methods
    // Important for other methods to use
    #region Logic
    // Calculate Unity Position from 2D game coords
    public Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return bottomLeftCornerTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }

    // Calculate 2D game coords from Unity position
    private Vector2Int CalculateCoordsFromPosition(Vector3 inputPosition)
    {
        // Current version. Note: BAD SOLUTION. DO NOT FIX.
        int x = Mathf.FloorToInt(inputPosition.x / squareSize + 0.5f) + Mathf.FloorToInt((float)FIELD_WIDTH / 2);
        int y = Mathf.FloorToInt(inputPosition.z / squareSize + 0.5f) + Mathf.FloorToInt((float)FIELD_HEIGHT / 2);

        return new Vector2Int(x, y);
    }

    // Get Unit located on coords
    public Unit GetUnitOnSquare(Vector2Int coords)
    {
        if (CheckIfCoordsAreOnField(coords))
            return grid[coords.x, coords.y];
        return null;
    }

    // Check if coords provided are legal
    public bool CheckIfCoordsAreOnField(Vector2Int coords)
    {
        int x = coords.x;
        int y = coords.y;
        if (x < 0 || y < 0 || x >= FIELD_WIDTH || y >= FIELD_HEIGHT)
            return false;
        return true;
    }

    // Check if the Unit is on the field
    public bool HasUnit(Unit unit)
    {
        for (int x = 0; x < FIELD_WIDTH; x++)
        {
            for (int y = 0; y < FIELD_HEIGHT; y++)
            {
                if (grid[x, y] == unit)
                    return true;
            }
        }
        return false;
    }
    #endregion

    // Square selection and Unit move / attack system
    #region Selection and Action system
    // Square selection system. Directly take input from FieldInputHandler
    public void OnSquareSelected(Vector3 inputPosition)
    {
        // If there is no GameController or it is not the current player's turn, return
        if (!gameController || !gameController.CanPerformAction())
            return;

        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);
        Unit unit = GetUnitOnSquare(coords);

        // If a Unit is already selected ...
        if (selectedUnit)
        {
            // If the Unit is selected again, deselect
            if (unit != null && selectedUnit == unit)
                DeselectUnit();
            // If selecting another Unit on the same team
            else if (unit != null && selectedUnit != unit && gameController.IsTeamTurnActive(unit.Team))
            {
                DeselectUnit();
                SelectUnit(coords);
            }
            // Selecting a movement square
            else if (selectedUnit.CanMoveTo(coords))
                SelectedUnitMoved(coords);
            // Selecting an attack square
            else if (selectedUnit.CanAttackAt(coords))
                SelectedUnitAttacked(coords);
        }
        else
        // If there is no selected Unit ...
        {
            // Only select Units from the same team
            if (unit != null && gameController.IsTeamTurnActive(unit.Team))
                SelectUnit(coords);
        }
    }

    private void SelectUnit(Vector2Int coords)
    {
        SetSelectedUnit(coords);

        ShowSelectionSquare(selectedUnit.availableMoves, true);
        ShowSelectionSquare(selectedUnit.availableAttacks, false);

        // In-game UI:
        gameController.ShowUnitDetails(selectedUnit);
    }

    private void ShowSelectionSquare(HashSet<Vector2Int> selection, bool moveTrue)
    {
        Dictionary<Vector3, bool> selectionData = new Dictionary<Vector3, bool>();
        foreach (var selectedCoords in selection)
        {
            Vector3 position = CalculatePositionFromCoords(selectedCoords);
            // Manual y-reset due to elevated field
            position.y = 0;
            selectionData.Add(position, moveTrue);
        }
        squareSelector.ShowSelection(selectionData);
    }

    public void DeselectUnit()
    {
        selectedUnit = null;
        squareSelector.ClearSelection();

        // In-game UI:
        gameController.HideUnitDetails();
    }

    public void OnSetSelectedUnit(Vector2Int coords)
    {
        Unit unit = GetUnitOnSquare(coords);
        selectedUnit = unit;
    }

    public void OnSelectedUnitMove(Vector2Int coords)
    {
        UpdateFieldOnUnitMove(coords, selectedUnit.OccupiedSquare, selectedUnit, null);
        selectedUnit.MoveUnit(coords);
        selectedUnit.EndAction();
        DeselectUnit();
        EndAction();
    }

    public void OnSelectedUnitAttack(Vector2Int coords)
    {
        selectedUnit.AttackAt(coords);
        selectedUnit.EndAction();
        DeselectUnit();
        EndAction();
    }
    #endregion

    // Utility (or methods I don't know how to catagorize)
    #region Utility
    // Add a Unit
    public void SetUnitOnField(Vector2Int coords, Unit unit)
    {
        if (CheckIfCoordsAreOnField(coords))
            grid[coords.x, coords.y] = unit;
    }

    // Remove a Unit
    public void RemoveUnit(Unit unit)
    {
        if (unit)
        {
            grid[unit.OccupiedSquare.x, unit.OccupiedSquare.y] = null;
            gameController.OnUnitRemoved(unit);
        }
    }

    // End action of the selected piece
    private void EndAction()
    {
        gameController.EndAction();
    }
    
    // End turn of the current player
    public virtual void EndTurn()
    {
        gameController.EndTurn();
    }

    // Update the Grid on Unit movement
    public void UpdateFieldOnUnitMove(Vector2Int newCoords, Vector2Int oldCoords, Unit newUnit, Unit oldUnit)
    {
        grid[newCoords.x, newCoords.y] = newUnit;
        grid[oldCoords.x, oldCoords.y] = oldUnit;
    }

    // Animation 
    public void ShowExecutionAnimation(Unit attacker, Unit defender)
    {
        gameController.ShowExecutionAnimation(attacker, defender);
    }

    // UI
    public void UpdateUnitDetails(Unit unit)
    {
        gameController.UpdateUnitDetails(unit);
    }
    #endregion
}