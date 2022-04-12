Module ItemMenu
    Private Const DropText = "Drop"
    Private Const DrinkText = "Drink"
    Friend Sub Run(character As PlayerCharacter, item As Item)
        AnsiConsole.Clear()
        Dim prompt As New SelectionPrompt(Of String) With {.Title = $"[olive]What would you like to do with {item.Name}:[/]"}
        prompt.AddChoice(NeverMindText)
        prompt.AddChoice(DropText)
        If item.CanDrink Then
            prompt.AddChoice(DrinkText)
        End If
        Dim answer = AnsiConsole.Prompt(prompt)
        Select Case answer
            Case NeverMindText
                character.State = PlayerState.Inventory
            Case DropText
                character.Location.Inventory.Add(item)
            Case DrinkText
                HandleDrink(character, item)
            Case Else
                Throw New NotImplementedException
        End Select
    End Sub

    Private Sub HandleDrink(character As PlayerCharacter, item As Item)
        character.Drink(item)
    End Sub
End Module
