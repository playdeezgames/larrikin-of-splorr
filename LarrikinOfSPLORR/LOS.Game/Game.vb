Public Module Game
    Sub NewGame()
        Store.Reset()
        CreateMaze()
        Dim characterId = CharacterData.Create()
        PlayerData.DoStuff(characterId, PlayerState.Exploration)
    End Sub

    ReadOnly Property Walkers As New Dictionary(Of Direction, MazeDirection(Of Direction)) From {
        {Direction.North, New MazeDirection(Of Direction)(Direction.South, 0, -1)},
        {Direction.East, New MazeDirection(Of Direction)(Direction.West, 1, 0)},
        {Direction.South, New MazeDirection(Of Direction)(Direction.North, 0, 1)},
        {Direction.West, New MazeDirection(Of Direction)(Direction.East, -1, 0)}
        }

    Private Sub CreateMaze()
        Dim maze As New Maze(Of Direction)(MazeColumns, MazeRows, Walkers)
        maze.Generate()
        For column = 0 To maze.Columns
            For row = 0 To maze.Rows
                Dim locationId = LocationData.Create(column, row)
            Next
        Next
    End Sub

    Sub Finish()
        Store.ShutDown()
    End Sub
End Module
