﻿using System;
using System.Collections.Generic;

namespace Trapzoid.Agent {

  public delegate void LoadPlayerPositionEventHandler(PlayerCell playerCell);
  public delegate void LoadOpponentPositionEventHandler(OpponentCell playerCell);

  /// <summary>
  /// Represents the world
  /// </summary>
  public class World {

    #region Constructor

    /// <summary>
    /// Default Constructor
    /// </summary>    
    public World() {
      Cells = new Dictionary<int, Dictionary<int, Cell>>();
    }

    #endregion

    #region Properties

    /// <summary> Represents the world </summary>
    public Dictionary<int, Dictionary<int, Cell>> Cells { get; set; }

    #endregion

    #region Events
    public event LoadOpponentPositionEventHandler LoadOpponentPositionEvent;
    public event LoadPlayerPositionEventHandler LoadPlayerPositionEvent;
    #endregion

    #region Public Methods

    /// <summary>
    /// Builds the world from the input given as a string
    /// </summary>
    /// <param name="newWorld"></param>
    /// <returns></returns>
    public bool BuildWorld(string[] newWorld) {
      bool builtWorld = false;
      if (newWorld.Length > 0) {
        OpponentCell opponent = null;
        PlayerCell player = null;
        for (var i = 0; i < newWorld.Length; i++) {
          string[] line = newWorld[i].Split(' ');
          int x = int.Parse(line[0]);
          int y = int.Parse(line[1]);
          if (!Cells.ContainsKey(x)) {
            Cells.Add(x, new Dictionary<int, Cell>());
          }
          if (!Cells[x].ContainsKey(y)) {
            CellContent content = Cell.GetWorldState(line[2]);
            switch (content) {
              case CellContent.Opponent:
                opponent = new OpponentCell() { X = x, Y = y, Content = content, Value = 0 };
                Cells[x].Add(y, opponent);
                break;
              case CellContent.You:
                player = new PlayerCell() { X = x, Y = y, Content = content, Value = 0 };
                Cells[x].Add(y, player);
                break;
              case CellContent.OpponentWall:
                Cells[x].Add(y, new Cell() { X = x, Y = y, Content = content, Value = 0 });
                break;
              case CellContent.YourWall:
                Cells[x].Add(y, new Cell() { X = x, Y = y, Content = content, Value = 0 });
                break;
              case CellContent.Clear:
                Cells[x].Add(y, new Cell() { X = x, Y = y, Content = content, Value = 1 });
                break;
            }
          }
          builtWorld = true;
        }
        if (LoadOpponentPositionEvent != null) {
          LoadPositions(opponent);
          LoadOpponentPositionEvent(opponent);
        }
        if (LoadOpponentPositionEvent != null) {
          LoadPositions(player);
          LoadPlayerPositionEvent(player);
        }
      }
      return builtWorld;
    }

    /// <summary>
    /// Displays the world
    /// </summary>    
    public void DisplayWorld() {
      for (int i = 0; i < Cells.Count; i++) {
        for (int j = 0; j < Cells.Count; j++) {
          switch (Cells[i][j].Content) {
            case CellContent.Opponent: Console.BackgroundColor = ConsoleColor.Red; break;
            case CellContent.OpponentWall: Console.BackgroundColor = ConsoleColor.DarkRed; break;
            case CellContent.You: Console.BackgroundColor = ConsoleColor.Blue; break;
            case CellContent.YourWall: Console.BackgroundColor = ConsoleColor.DarkBlue; break;
            case CellContent.Clear: Console.BackgroundColor = ConsoleColor.Gray; break;
          }
          Console.SetCursorPosition(i, j);
          Console.Write((int)Cells[i][j].Content);
        }
        Console.WriteLine();
      }
    }

    /// <summary>
    /// Gets the world as a string
    /// </summary>
    /// <returns></returns>
    public string GetWorld() {
      string result = "";
      for (int i = 0; i < Cells.Count; i++) {
        for (int j = 0; j < Cells.Count; j++) {
          result += string.Format("{0} {1} {2}\r\n", Cells[i][j].X, Cells[i][j].Y, Cells[i][j].Content);
        }
      }
      return result;
    }

    /// <summary>
    /// Loads the positions around the light cycle
    /// </summary>
    /// <param name="lightCycle">Light cycle to load positions for</param>
    public void LoadPositions(Cell cell) {
      cell.North = GetNorthPosition(cell);
      cell.South = GetSouthPosition(cell);
      cell.East = GetEastPosition(cell);
      cell.West = GetWestPosition(cell);
    }

    #endregion

    #region Private Methods    

    /// <summary>
    /// Retrieves the position north of the cell
    /// </summary>
    /// <param name="position">Position to check against</param>
    /// <returns>Cell north of the current cell</returns>
    private Cell GetNorthPosition(Cell position) {
      int y = (position.Y == 0) ? Cells.Count - 1 : position.Y - 1;
      return Cells[position.X][y];
    }

    /// <summary>
    /// Retrieves the position South of the cell
    /// </summary>
    /// <param name="position">Position to check against</param>
    /// <returns>Cell south of the current cell</returns>
    private Cell GetSouthPosition(Cell position) {
      int y = (position.Y == Cells.Count - 1) ? 0 : position.Y + 1;
      return Cells[position.X][y];
    }

    /// <summary>
    /// Retrieves the position east of the cell
    /// </summary>
    /// <param name="position">Position to check against</param>
    /// <returns>Cell east of the current cell</returns>
    private Cell GetEastPosition(Cell position) {
      int x = (position.X == Cells.Count - 1) ? 0 : position.X + 1;
      return Cells[x][position.Y];
    }

    /// <summary>
    /// Retrieves the position west of the cell
    /// </summary>
    /// <param name="position">Position to check against</param>
    /// <returns>Cell west of the current cell</returns>
    private Cell GetWestPosition(Cell position) {
      int x = (position.X == 0) ? Cells.Count - 1 : position.X - 1;
      return Cells[x][position.Y];
    }

    #endregion
  }
}
