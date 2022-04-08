Imports System.Runtime.CompilerServices

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
    Const LeftColumn = 0
    Const AheadColumn = 14
    Const RightColumn = 42
    <Extension()>
    Public Sub DrawImage(canvas As Canvas, startColumn As Integer, startRow As Integer, image As List(Of String), solidColor As Color?, emptyColor As Color?)
        For row = 0 To image.Count - 1
            For column = 0 To image(row).Length - 1
                Dim cellColor = If(image(row)(column) = Solid, solidColor, emptyColor)
                If cellColor.HasValue Then
                    canvas.SetPixel(column + startColumn, row + startRow, cellColor.Value)
                End If
            Next
        Next
    End Sub
    Private Sub DrawLocation(character As PlayerCharacter)
        Dim canvas As New Canvas(ViewColumns, ViewRows)
        canvas.DrawImage(LeftColumn, 0, If(RNG.RollDice("1d2") = 1, LeftWall, LeftDoor), Color.White, Color.Black)
        canvas.DrawImage(AheadColumn, 0, If(RNG.RollDice("1d2") = 1, AheadWall, AheadDoor), Color.White, Color.Black)
        canvas.DrawImage(RightColumn, 0, If(RNG.RollDice("1d2") = 1, RightWall, RightDoor), Color.White, Color.Black)
        AnsiConsole.Clear()
        AnsiConsole.Write(canvas)
    End Sub
End Module
