Module WinState
    Friend Sub Run()
        AnsiConsole.Clear()
        Dim figlet = New FigletText("You Win!").Centered()
        figlet.Color = Color.Lime
        AnsiConsole.Write(figlet)
        SfxPlayer.Play(Sfx.Win)
        OkPrompt()
    End Sub
End Module
