Module GroundInventoryMenu
    Sub Run(character As PlayerCharacter)
        AnsiConsole.Clear()
        Dim prompt As New SelectionPrompt(Of String) With {.Title = "[olive]On The Ground:[/]"}
        prompt.AddChoice(NeverMindText)
        For Each item In character.Location.Inventory.Items
            prompt.AddChoice(item.Name)
        Next
        Dim answer = AnsiConsole.Prompt(prompt)
        Select Case answer
            Case NeverMindText
                character.State = PlayerState.Exploration
            Case Else
                Dim item = character.Location.Inventory.Items.First(Function(x) x.Name = answer)
        End Select
    End Sub
End Module
