Module FightMenu
    Friend Sub Run(character As PlayerCharacter)
        AnsiConsole.Clear()
        Dim prompt As New SelectionPrompt(Of String) With {.Title = "[olive]Attack Target:[/]"}
        Dim enemyTable As New Dictionary(Of String, Character)
        For Each enemy In character.Location.Enemies
            Dim description = $"{enemy.Name}(H:[red]{enemy.Health}/{enemy.MaximumHealth}[/])"
            enemyTable(description) = enemy
            prompt.AddChoice(description)
        Next
        prompt.AddChoice(NeverMindText) 'TODO: run away?
        Dim answer = AnsiConsole.Prompt(prompt)
        Select Case answer
            Case NeverMindText
                character.State = PlayerState.Exploration
            Case Else
                HandleAttack(character, enemyTable(answer))
                If Not character.Location.Enemies.Any Then
                    character.State = PlayerState.Exploration
                End If
        End Select
    End Sub

    Private Sub HandleAttack(character As PlayerCharacter, enemy As Character)
        AnsiConsole.Clear()
        Dim attackRoll = RNG.RollDice(character.AttackDice)
        AnsiConsole.MarkupLine($"You roll an attack of {attackRoll}!")
        Dim defendRoll = RNG.RollDice(enemy.DefendDice)
        AnsiConsole.MarkupLine($"{enemy.Name} rolls a defense of {defendRoll}!")
        If attackRoll > defendRoll Then
            Dim damage = attackRoll - defendRoll
            AnsiConsole.MarkupLine($"You hit for {damage} damage!")
            enemy.Health -= damage
            If enemy.IsDead Then
                AnsiConsole.MarkupLine($"You kill {enemy.Name}!")
                enemy.Destroy()
                Play("L250;C4;C4;C4;L500;G4")
            Else
                Play("L500;F#4")
            End If
        Else
            AnsiConsole.MarkupLine($"You miss!")
            Play("L500;F#2")
        End If
        OkPrompt()
    End Sub
End Module
