using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelectorCreator))]
public abstract class Field : MonoBehaviour
{
    public const int FIELD_WIDTH = 21;
    public const int FIELD_HEIGHT = 21;
    public const int FIELD_SIZE = 21;

    [SerializeField] private Transform bottomLeftCornerTransform;
    [SerializeField] private Transform bottomRightCornerTransform;
    [SerializeField] private float squareSize;
    private GameController gameController;
    private SquareSelectorCreator squareSelector;

    private Unit[,] grid;
    private Unit selectedUnit;

    public abstract void SelectedUnitMoved(Vector2 coords);
    public abstract void SetSelectedUnit(Vector2 coords);

    protected virtual void Awake()
    {
        squareSelector = GetComponent<SquareSelectorCreator>();
        SetSquareSize();
        CreateGrid();
    }

    private void SetSquareSize()
    {
        squareSize = Mathf.Abs(bottomLeftCornerTransform.position.x - bottomRightCornerTransform.position.x) / (FIELD_SIZE - 1);
    }

    public void SetDependencies(GameController gameController)
    {
        this.gameController = gameController;
    }

    private void CreateGrid()
    {
        grid = new Unit[FIELD_WIDTH, FIELD_HEIGHT];
    }

    public Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return bottomLeftCornerTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }

    private Vector2Int CalculateCoordsFromPosition(Vector3 inputPosition)
    {
        // Current version. Note: BAD SOLUTION. DO NOT FIX.
        int x = Mathf.FloorToInt(inputPosition.x / squareSize + 0.5f) + Mathf.FloorToInt((float)FIELD_WIDTH / 2);
        int y = Mathf.FloorToInt(inputPosition.z / squareSize + 0.5f) + Mathf.FloorToInt((float)FIELD_HEIGHT / 2);

        return new Vector2Int(x, y);
    }

    public void OnSquareSelected(Vector3 inputPosition)
    {
        if (!gameController || !gameController.CanPerformMove())
            return;

        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);
        Unit unit = GetUnitOnSquare(coords);

        if (selectedUnit)
        {
            if (unit != null && selectedUnit == unit)
                DeselectUnit();
            else if (unit != null && selectedUnit != unit && gameController.IsTeamTurnActive(unit.team))
                SelectUnit(coords);
            else if (selectedUnit.CanMoveTo(coords))
                SelectedUnitMoved(coords);
        }
        else
        {
            if (unit != null && gameController.IsTeamTurnActive(unit.team))
                SelectUnit(coords);
        }
    }

    public void OnGameRestarted()
    {
        selectedUnit = null;
        CreateGrid();
    }

    private void SelectUnit(Vector2Int coords)
    {
        Unit unit = GetUnitOnSquare(coords);

        SetSelectedUnit(coords);
        List<Vector2Int> selection = selectedUnit.availableMoves;
        ShowSelectionSquare(selection);
    }

    private void ShowSelectionSquare(List<Vector2Int> selection)
    {
        Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();
        foreach (var selectedCoords in selection)
        {
            Vector3 position = CalculatePositionFromCoords(selectedCoords);
            // Manual y-reset due to elevated field
            position.y = 0;
            bool isSquareFree = (GetUnitOnSquare(selectedCoords) == null);
            squaresData.Add(position, isSquareFree);
        }
        squareSelector.ShowSelection(squaresData);
    }

    private void DeselectUnit()
    {
        selectedUnit = null;
        squareSelector.ClearSelection();
    }

    public void OnSelectedUnitMove(Vector2Int coords)
    {
        TryToTakeOppositeUnit(coords);

        UpdateFieldOnUnitMove(coords, selectedUnit.occupiedSquare, selectedUnit, null);
        selectedUnit.MoveUnit(coords);
        DeselectUnit();
        EndTurn();
    }

    public void OnSetSelectedUnit(Vector2Int coords)
    {
        Unit unit = GetUnitOnSquare(coords);
        selectedUnit = unit;
    }

    private void TryToTakeOppositeUnit(Vector2Int coords)
    {
        Unit unit = GetUnitOnSquare(coords);
        if (unit != null && !selectedUnit.IsFromSameTeam(unit))
            TakeUnit(unit);
    }

    private void TakeUnit(Unit unit)
    {
        if (unit)
        {
            grid[unit.occupiedSquare.x, unit.occupiedSquare.y] = null;
            gameController.OnUnitRemoved(unit);
        }
    }

    private void EndTurn()
    {
        gameController.EndTurn();
    }

    public void UpdateFieldOnUnitMove(Vector2Int newCoords, Vector2Int oldCoords, Unit newUnit, Unit oldUnit)
    {
        grid[newCoords.x, newCoords.y] = newUnit;
        grid[oldCoords.x, oldCoords.y] = oldUnit;
    }

    public Unit GetUnitOnSquare(Vector2Int coords)
    {
        if (CheckIfCoordsAreOnField(coords))
            return grid[coords.x, coords.y];
        return null;
    }

    public bool CheckIfCoordsAreOnField(Vector2Int coords)
    {
        int x = coords.x;
        int y = coords.y;
        if (x < 0 || y < 0 || x >= FIELD_WIDTH || y >= FIELD_HEIGHT)
            return false;
        return true;
    }

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

    public void SetUnitOnField(Vector2Int coords, Unit unit)
    {
        if (CheckIfCoordsAreOnField(coords))
            grid[coords.x, coords.y] = unit;
    }
}