Module FightMenu
    Friend Sub Run(character As PlayerCharacter)
        AnsiConsole.Clear()
        Dim prompt As New SelectionPrompt(Of String) With {.Title = "[olive]Attack Target:[/]"}
        prompt.AddChoice(NeverMindText)
        Dim answer = AnsiConsole.Prompt(prompt)
        Select Case answer
            Case NeverMindText
                character.State = PlayerState.Exploration
            Case Else
                Throw New NotImplementedException
        End Select
    End Sub
End Module
