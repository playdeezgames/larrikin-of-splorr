Module InPlayMenu
    Sub Run()
        Dim done = False
        While Not done
            Dim character As New PlayerCharacter()
            Select Case character.State
                Case PlayerState.Exploration
                    ExplorationState.Run(character)
                Case PlayerState.GameMenu
                    done = GameMenu.Run()
                Case PlayerState.GroundInventory
                    GroundInventoryMenu.Run(character)
                Case Else
                    Throw New NotImplementedException
            End Select
        End While
    End Sub
End Module
