Module FightMenu
    Private Const RunText = "RUN!"
    Friend Sub Run(character As PlayerCharacter)
        AnsiConsole.Clear()
        Dim prompt As New SelectionPrompt(Of String) With {.Title = "[olive]Attack Target:[/]"}
        Dim enemyTable As New Dictionary(Of String, Character)
        For Each enemy In character.Location.Enemies
            Dim description = $"{enemy.Name}(H:[red]{enemy.Health}/{enemy.MaximumHealth}[/])"
            enemyTable(description) = enemy
            prompt.AddChoice(description)
        Next
        prompt.AddChoice(RunText) 'TODO: run away?
        Dim answer = AnsiConsole.Prompt(prompt)
        Select Case answer
            Case RunText
                HandleRun(character)
            Case Else
                HandleAttack(character, enemyTable(answer))
                If Not character.Location.Enemies.Any Then
                    character.State = PlayerState.Exploration
                End If
        End Select
    End Sub

    Private Sub HandleRun(character As PlayerCharacter)
        AnsiConsole.Clear()
        character.Direction = RNG.FromList(AllDirections)
        If character.Move() Then
            AnsiConsole.MarkupLine("You manage to run away!")
            character.State = PlayerState.Exploration
            Play("L500;F#4")
        Else
            AnsiConsole.MarkupLine("You fail to run away!")
            Play("L500;F#2")
            character.State = PlayerState.Defend
        End If
        OkPrompt()
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
                Play("L500;B4")
            End If
        Else
            AnsiConsole.MarkupLine($"You miss!")
            Play("L500;B2")
        End If
        character.State = PlayerState.Defend
        OkPrompt()
    End Sub
End Module
