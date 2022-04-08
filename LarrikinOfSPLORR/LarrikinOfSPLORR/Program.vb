Module Program
    Friend Const OkText = "Ok"
    Public Sub OkPrompt()
        Dim prompt As New SelectionPrompt(Of String) With {.Title = ""}
        prompt.AddChoice(OkText)
        AnsiConsole.Prompt(prompt)
    End Sub

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

    Sub Main(args As String())
        Console.Title = "Larrikin of SPLORR!!"
        AddHandler SfxPlayer.PlaySfx, AddressOf SfxHandler.HandleSfx
        Welcome()
    End Sub
End Module
