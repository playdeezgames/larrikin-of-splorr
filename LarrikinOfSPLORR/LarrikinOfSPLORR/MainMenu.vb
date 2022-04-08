Module MainMenu
    Private Const QuitText = "Quit"
    Private Function CreatePrompt() As SelectionPrompt(Of String)
        Dim prompt As New SelectionPrompt(Of String) With {.Title = "[olive]Main Menu:[/]"}
        prompt.AddChoice(QuitText)
        Return prompt
    End Function
    Sub Run()
        Dim done = False
        While Not done
            AnsiConsole.Clear()
            Select Case AnsiConsole.Prompt(CreatePrompt())
                Case QuitText
                    done = HandleQuit()
                Case Else
                    Throw New NotImplementedException
            End Select
        End While
    End Sub

    Private Function HandleQuit() As Boolean
        Return AnsiConsole.Confirm("[red]Are you sure you want to quit?[/]", False)
    End Function
End Module
