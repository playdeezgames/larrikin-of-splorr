Public Module Game
    Sub NewGame()
        Store.Reset()
        CreateMaze()
        PopulateMaze()
        Dim locationId = LocationData.ReadByColumnAndRow(0, 0).Value
        Dim characterId = CharacterData.Create(locationId, CharacterType.Larrikin) 'TODO: place character with more care!
        PlayerData.DoStuff(characterId, PlayerState.Exploration, Direction.North)
        PlayerLocationData.Write(locationId)
    End Sub

    Private Sub PopulateMaze()
        PopulateItems()
        PopulateEnemies()
    End Sub

    Private Sub PopulateEnemies()
        For Each characterType In AllEnemyTypes
            Dim spawnCount = RNG.RollDice(characterType.SpawnDice)
            While spawnCount > 0
                Dim location = RNG.FromList(AllLocations)
                CharacterData.Create(location.Id, characterType)
                spawnCount -= 1
            End While
        Next
    End Sub

    Private Sub PopulateItems()
        For Each itemType In AllItemTypes
            Dim spawnCount = RNG.RollDice(itemType.SpawnDice)
            While spawnCount > 0
                Dim location = RNG.FromList(AllLocations)
                location.Inventory.Add(New Item(ItemData.Create(itemType)))
                spawnCount -= 1
            End While
        Next
    End Sub

    ReadOnly Property Walkers As New Dictionary(Of Direction, MazeDirection(Of Direction)) From {
        {Direction.North, New MazeDirection(Of Direction)(Direction.South, 0, -1)},
        {Direction.East, New MazeDirection(Of Direction)(Direction.West, 1, 0)},
        {Direction.South, New MazeDirection(Of Direction)(Direction.North, 0, 1)},
        {Direction.West, New MazeDirection(Of Direction)(Direction.East, -1, 0)}
        }
    Private Sub CreateMazeLocations(maze As Maze(Of Direction))
        For column = 0 To maze.Columns - 1
            For row = 0 To maze.Rows - 1
                LocationData.Create(column, row)
            Next
        Next
    End Sub
    Private Sub CreateMaze()
        Dim maze As New Maze(Of Direction)(MazeColumns, MazeRows, Walkers)
        maze.Generate()
        CreateMazeLocations(maze)
        CreateMazeTransitions(maze)
    End Sub

    Private Sub CreateMazeTransitions(maze As Maze(Of Direction))
        For column = 0 To maze.Columns - 1
            For row = 0 To maze.Rows - 1
                Dim fromLocationId = LocationData.ReadByColumnAndRow(column, row)
                For Each direction In AllDirections
                    Dim cell = maze.GetCell(column, row)
                    Dim door = cell.GetDoor(direction)
                    Dim toLocationId = LocationData.ReadByColumnAndRow(direction.NextColumn(column), direction.NextRow(row))
                    If door IsNot Nothing AndAlso door.Open Then
                        TransitionData.Write(fromLocationId.Value, toLocationId.Value, direction, TransitionState.Open)
                    Else
                        If toLocationId.HasValue Then
                            TransitionData.Write(fromLocationId.Value, toLocationId.Value, direction, TransitionState.Wall)
                        Else
                            TransitionData.Write(fromLocationId.Value, fromLocationId.Value, direction, TransitionState.Wall)
                        End If
                    End If
                Next
            Next
        Next
    End Sub

    Sub Finish()
        Store.ShutDown()
    End Sub
End Module
