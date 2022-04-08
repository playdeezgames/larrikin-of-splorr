Module ExplorationState
    Const Solid = "X"c
    ReadOnly Walls As New List(Of String) From {
        "XX                                                    XX",
        "  XX                                                XX  ",
        "    XX                                            XX    ",
        "      XX                                        XX      ",
        "        XX                                    XX        ",
        "          XX                                XX          ",
        "            XX                            XX            ",
        "              XXXXXXXXXXXXXXXXXXXXXXXXXXXX              ",
        "              X                          X              ",
        "              X                          X              ",
        "              X                          X              ",
        "              X                          X              ",
        "              X                          X              ",
        "              X                          X              ",
        "              X                          X              ",
        "              X                          X              ",
        "              X                          X              ",
        "              X                          X              ",
        "              X                          X              ",
        "              X                          X              ",
        "              XXXXXXXXXXXXXXXXXXXXXXXXXXXX              ",
        "            XX                            XX            ",
        "          XX                                XX          ",
        "        XX                                    XX        ",
        "      XX                                        XX      ",
        "    XX                                            XX    ",
        "  XX                                                XX  ",
        "XX                                                    XX"
        }
    ReadOnly Doors As New List(Of String) From {
        "XX                                                    XX",
        "  XX                                                XX  ",
        "    XX                                            XX    ",
        "      XX                                        XX      ",
        "        XX                                    XX        ",
        "          XX                                XX          ",
        "            XX                            XX            ",
        "              XXXXXXXXXXXXXXXXXXXXXXXXXXXX              ",
        "    XXX       X                          X       XXX    ",
        "    X  XX     X                          X     XX  X    ",
        "    X    X    X                          X    X    X    ",
        "    X    X    X       XXXXXXXXXXXX       X    X    X    ",
        "    X    X    X       X          X       X    X    X    ",
        "    X    X    X       X          X       X    X    X    ",
        "    X    X    X       X          X       X    X    X    ",
        "    X    X    X       X          X       X    X    X    ",
        "    X    X    X       X          X       X    X    X    ",
        "    X    X    X       X          X       X    X    X    ",
        "    X    X    X       X          X       X    X    X    ",
        "    X    X    X       X          X       X    X    X    ",
        "    X    X    XXXXXXXXX          XXXXXXXXX    X    X    ",
        "    X    X  XX                            XX  X    X    ",
        "    X    XXX                                XXX    X    ",
        "    X                                              X    ",
        "    X                                              X    ",
        "    X                                              X    ",
        "  XX                                                XX  ",
        "XX                                                    XX"
        }
    Sub Run()
        AnsiConsole.Clear()
        Dim character As New PlayerCharacter
        DrawLocation(character)
        AnsiConsole.MarkupLine("Esc: Game Menu")
        Select Case Console.ReadKey(True).Key
            Case ConsoleKey.Escape
                character.State = PlayerState.GameMenu
        End Select
    End Sub
    Const ViewColumns = 56
    Const ViewRows = 28
    Private Sub DrawLocation(character As PlayerCharacter)
        Dim canvas As New Canvas(ViewColumns, ViewRows)
        For column = 0 To ViewColumns - 1
            For row = 0 To ViewRows - 1
                Dim cellColor = If(Doors(row)(column) = Solid, Color.White, Color.Black)
                canvas.SetPixel(column, row, cellColor)
            Next
        Next
        AnsiConsole.Clear()
        AnsiConsole.Write(canvas)
    End Sub
End Module
