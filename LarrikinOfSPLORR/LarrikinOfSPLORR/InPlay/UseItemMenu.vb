Module UseItemMenu
    Friend Sub Run(character As PlayerCharacter)
        AnsiConsole.Clear()
        Dim prompt As New SelectionPrompt(Of String) With {.Title = "[olive]Use What?[/]"}
        prompt.AddChoice(NeverMindText)
        For Each item In character.Inventory.Items.Where(Function(x) x.CanUse)
            prompt.AddChoice(item.Name)
        Next
        Dim answer = AnsiConsole.Prompt(prompt)
        Select Case answer
            Case NeverMindText
                'do nothing
            Case Else
                Dim item = character.Inventory.Items.First(Function(x) x.Name = answer)
                HandleUseItem(character, item)
        End Select
    End Sub

    Private Sub HandleUseItem(character As PlayerCharacter, item As Item)
        AnsiConsole.MarkupLine("You use the item!")
        character.UseItem(item)
        character.State = PlayerState.Defend
    End Sub
End Module
