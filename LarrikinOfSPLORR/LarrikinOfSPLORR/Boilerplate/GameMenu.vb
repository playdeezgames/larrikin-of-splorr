Module GameMenu
    Private Const ContinueText = "Continue"
    Private Const AbandonGameText = "Abandon Game"
    Private Const SaveGameText = "Save Game..."
    Function Run() As Boolean
        Dim done = False
        Dim character As New PlayerCharacter
        While Not done
            AnsiConsole.Clear()
            Dim prompt As New SelectionPrompt(Of String) With {.Title = "[olive]Game Menu:[/]"}
            prompt.AddChoices(ContinueText, SaveGameText, AbandonGameText)
            Select Case AnsiConsole.Prompt(prompt)
                Case SaveGameText
                    HandleSaveGame()
                Case ContinueText
                    done = True
                    character.State = PlayerState.Exploration
                Case AbandonGameText
                    done = HandleAbandonGame()
                    If done Then
                        Return True
                    End If
                Case Else
                    Throw New NotImplementedException
            End Select
        End While
        Return False
    End Function

    Private Sub HandleSaveGame()
        AnsiConsole.Clear()
        Dim fileName = AnsiConsole.Ask(Of String)("[olive]Filename:[/]", "")
        If Not String.IsNullOrWhiteSpace(fileName) Then
            Store.Save(fileName)
        End If
    End Sub

    Private Function HandleAbandonGame() As Boolean
        Dim abandon = AnsiConsole.Confirm("[red]Are you sure you want to abandon the game?[/]", False)
        If abandon Then
            Game.Finish()
        End If
        Return abandon
    End Function
End Module
