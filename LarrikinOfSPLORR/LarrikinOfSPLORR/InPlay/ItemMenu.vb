Module ItemMenu
    Private Const DropText = "Drop"
    Friend Sub Run(character As PlayerCharacter, item As Item)
        AnsiConsole.Clear()
        Dim prompt As New SelectionPrompt(Of String) With {.Title = $"[olive]What would you like to do with {item.Name}:[/]"}
        prompt.AddChoice(NeverMindText)
        prompt.AddChoice(DropText)
        Dim answer = AnsiConsole.Prompt(prompt)
        Select Case answer
            Case NeverMindText
                character.State = PlayerState.Inventory
            Case DropText
                character.Location.Inventory.Add(item)
            Case Else
                Throw New NotImplementedException
        End Select
    End Sub
End Module
