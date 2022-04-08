Module MainMenu
    Private Const QuitText = "Quit"
    Private Const AboutText = "About..."
    Private Const EmbarkText = "Embark!"
    Private Function CreatePrompt() As SelectionPrompt(Of String)
        Dim prompt As New SelectionPrompt(Of String) With {.Title = "[olive]Main Menu:[/]"}
        prompt.AddChoice(EmbarkText)
        prompt.AddChoice(AboutText)
        prompt.AddChoice(QuitText)
        Return prompt
    End Function
    Sub Run()
        Dim done = False
        While Not done
            AnsiConsole.Clear()
            Select Case AnsiConsole.Prompt(CreatePrompt())
                Case EmbarkText
                    HandleEmbark()
                Case AboutText
                    HandleAbout()
                Case QuitText
                    done = HandleQuit()
                Case Else
                    Throw New NotImplementedException
            End Select
        End While
    End Sub

    Private Sub HandleEmbark()
        Game.NewGame()
        InPlayMenu.Run()
    End Sub

    Private Sub HandleAbout()
        AnsiConsole.Clear()
        AnsiConsole.MarkupLine("About Larrikin of SPLORR!!:")
        AnsiConsole.MarkupLine("Made by TheGrumpyGameDev for Dungeon Crawler Jam 2022")
        AnsiConsole.MarkupLine("With graphics adapted from https://vurmux.itch.io/urizen-onebit-tilesets")
        OkPrompt()
    End Sub

    Private Function HandleQuit() As Boolean
        Return AnsiConsole.Confirm("[red]Are you sure you want to quit?[/]", False)
    End Function
End Module
