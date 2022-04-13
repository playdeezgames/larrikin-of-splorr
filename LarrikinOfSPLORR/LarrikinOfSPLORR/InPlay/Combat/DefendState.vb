Module DefendState
    Friend Sub Run(character As PlayerCharacter)
        Dim enemies = character.Location.Enemies
        For Each enemy In enemies
            AnsiConsole.MarkupLine("-----------------------------------")
            If Not character.IsDead Then
                Dim attack = RNG.RollDice(enemy.AttackDice)
                AnsiConsole.MarkupLine($"{enemy.Name} rolls an attack of {attack}!")
                Dim defend = RNG.RollDice(character.DefendDice)
                AnsiConsole.MarkupLine($"You roll a defense of {defend}!")
                Dim shield = character.Shield
                If shield IsNot Nothing Then
                    If shield.DecreaseDurability(attack) Then
                        AnsiConsole.MarkupLine($"[red]Yer {shield.Name} broke![/]")
                        shield.Destroy()
                    End If
                End If
                If attack > defend Then
                    Dim damage = attack - defend
                    AnsiConsole.MarkupLine($"{enemy.Name} hits you for {damage}!")
                    character.Health -= damage
                    If character.IsDead Then
                        AnsiConsole.MarkupLine("Yer dead!")
                    Else
                        SfxPlayer.Play(Sfx.EnemyHit)
                    End If
                Else
                    AnsiConsole.MarkupLine($"{enemy.Name} misses!")
                    SfxPlayer.Play(Sfx.EnemyMiss)
                End If
            End If
        Next
        OkPrompt()
        If character.IsDead Then
            character.State = PlayerState.Dead
        Else
            character.State = PlayerState.Fight
        End If
    End Sub
End Module
