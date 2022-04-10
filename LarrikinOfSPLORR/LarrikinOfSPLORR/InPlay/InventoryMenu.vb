Module InventoryMenu
    Sub Run(character As PlayerCharacter)
        AnsiConsole.Clear()
        Dim prompt As New SelectionPrompt(Of String) With {.Title = "[olive]In Yer Inventory:[/]"}
        prompt.AddChoice(NeverMindText)
        For Each item In character.Inventory.Items
            prompt.AddChoice(item.Name)
        Next
        Dim answer = AnsiConsole.Prompt(prompt)
        Select Case answer
            Case NeverMindText
                character.State = PlayerState.Exploration
            Case Else
                Dim item = character.Inventory.Items.First(Function(x) x.Name = answer)
                ItemMenu.Run(character, item)
                If character.Inventory.IsEmpty Then
                    character.State = PlayerState.Exploration
                End If
        End Select
    End Sub
End Module
