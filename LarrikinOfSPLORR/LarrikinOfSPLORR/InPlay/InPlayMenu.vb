Module InPlayMenu
    Sub Run()
        Dim done = False
        While Not done
            Dim character As New PlayerCharacter()
            If character.HasWon Then
                WinState.Run()
                done = True
            Else
                Select Case character.State
                    Case PlayerState.Exploration
                        ExplorationState.Run(character)
                    Case PlayerState.GameMenu
                        done = GameMenu.Run()
                    Case PlayerState.GroundInventory
                        GroundInventoryMenu.Run(character)
                    Case PlayerState.Inventory
                        InventoryMenu.Run(character)
                    Case PlayerState.Fight
                        FightMenu.Run(character)
                    Case PlayerState.Defend
                        DefendState.Run(character)
                    Case PlayerState.Dead
                        DeadState.Run(character)
                        done = True
                    Case PlayerState.Map
                        MapState.Run(character)
                    Case Else
                        Throw New NotImplementedException
                End Select
            End If
        End While
    End Sub
End Module
