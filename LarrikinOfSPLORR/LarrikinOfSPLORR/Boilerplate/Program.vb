Module Program

    Private Sub Welcome()
        AnsiConsole.Clear()
        Dim figlet = New FigletText("Larrikin").Centered()
        figlet.Color = Color.Lime
        AnsiConsole.Write(figlet)
        figlet = New FigletText("of").Centered()
        figlet.Color = Color.Lime
        AnsiConsole.Write(figlet)
        figlet = New FigletText("SPLORR!!").Centered()
        figlet.Color = Color.Lime
        AnsiConsole.Write(figlet)
        AnsiConsole.MarkupLine("[gray]A Production of TheGrumpyGameDev[/]")
        AnsiConsole.MarkupLine("[gray]For Dungeon Crawler Jam 2022[/]")
        AnsiConsole.MarkupLine("[gray]...With ""help"" from his ""friends""[/]")
        SfxPlayer.Play(Sfx.Title)
        OkPrompt()
    End Sub
    Private Sub Bootstrap()
        Console.Title = "Larrikin of SPLORR!!"
#Disable Warning CA1416 ' Validate platform compatibility
        Console.WindowWidth = ScreenColumns
        Console.WindowHeight = ScreenRows
        Console.BufferWidth = ScreenColumns
        Console.BufferHeight = ScreenRows
#Enable Warning CA1416 ' Validate platform compatibility
        AddHandler SfxPlayer.PlaySfx, AddressOf SfxHandler.HandleSfx
    End Sub

    Sub Main(args As String())
        Bootstrap()
        Welcome()
        MainMenu.Run()
        Game.Finish()
    End Sub
End Module
