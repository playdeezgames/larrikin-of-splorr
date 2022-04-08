Module ExplorationState
    Sub Run()
        AnsiConsole.Clear()
        Dim character As New PlayerCharacter
        AnsiConsole.MarkupLine("TODO: draw location")
        AnsiConsole.MarkupLine("Esc: Game Menu")
        Select Case Console.ReadKey(True).Key
            Case ConsoleKey.Escape
                character.State = PlayerState.GameMenu
        End Select
    End Sub
End Module
