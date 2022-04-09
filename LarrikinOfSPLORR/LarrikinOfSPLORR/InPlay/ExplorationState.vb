﻿Imports System.Runtime.CompilerServices

Module ExplorationState
    Const Solid = "X"c
    Sub Run()
        AnsiConsole.Clear()
        Dim character As New PlayerCharacter
        DrawLocation(character)
        AnsiConsole.MarkupLine("[aqua]Arrows[/]-move/turn [gray]|[/] [aqua]<Esc>[/]-Game Menu")
        Select Case Console.ReadKey(True).Key
            Case ConsoleKey.LeftArrow
                HandleTurnLeft(character)
            Case ConsoleKey.RightArrow
                HandleTurnRight(character)
            Case ConsoleKey.UpArrow
                HandleMoveAhead(character)
            Case ConsoleKey.Escape
                character.State = PlayerState.GameMenu
        End Select
    End Sub

    Private Sub HandleMoveAhead(character As PlayerCharacter)
        Dim transition = character.Location.GetTransition(character.Direction)
        If transition.State = TransitionState.Open Then
            character.Location = transition.ToLocation
        Else
            Play("L500;F#2")
        End If
    End Sub

    Private Sub HandleTurnRight(character As PlayerCharacter)
        character.Direction = character.Direction.NextDirection()
    End Sub

    Private Sub HandleTurnLeft(character As PlayerCharacter)
        character.Direction = character.Direction.PreviousDirection()
    End Sub

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
        Dim location = character.Location
        Dim leftDirection = character.LeftDirection
        Dim transition = location.GetTransition(leftDirection)
        canvas.DrawImage(LeftColumn, 0, If(transition.State = TransitionState.Wall, LeftWall, LeftDoor), Color.White, Color.Black)

        Dim aheadDirection = character.Direction
        transition = location.GetTransition(aheadDirection)
        canvas.DrawImage(AheadColumn, 0, If(transition.State = TransitionState.Wall, AheadWall, AheadDoor), Color.White, Color.Black)

        Dim rightDirection = character.RightDirection
        transition = location.GetTransition(rightDirection)
        canvas.DrawImage(RightColumn, 0, If(transition.State = TransitionState.Wall, RightWall, RightDoor), Color.White, Color.Black)
        AnsiConsole.Clear()
        AnsiConsole.Write(canvas)
    End Sub
End Module
