Module MapState
    Friend Sub Run(character As PlayerCharacter)
        AnsiConsole.Clear()
        Dim canvas As New Canvas(MapColumns, MapRows)
        Dim column As Integer
        Dim row As Integer
        For column = 0 To MapColumns - 1
            For row = 0 To MapRows - 1
                canvas.SetPixel(column, row, Color.White)
            Next
        Next
        Dim characterLocationId = character.Location.Id
        For Each location In AllLocations
            column = CInt(location.Column) * 2 + 1
            row = CInt(location.Row) * 2 + 1
            canvas.SetPixel(column, row, If(location.Id = characterLocationId, Color.Green, Color.Black))
            Dim transition = location.GetTransition(Direction.East)
            If transition IsNot Nothing AndAlso transition.State = TransitionState.Open Then
                canvas.SetPixel(column + 1, row, Color.Black)
            End If
            transition = location.GetTransition(Direction.South)
            If transition IsNot Nothing AndAlso transition.State = TransitionState.Open Then
                canvas.SetPixel(column, row + 1, Color.Black)
            End If
        Next
        AnsiConsole.Write(canvas)
        Console.ReadKey(True)
        character.State = PlayerState.Exploration
    End Sub
End Module
