Imports System.Runtime.CompilerServices

Module ExplorationState
    Const Solid = "X"c
    Sub Run(character As PlayerCharacter)
        AnsiConsole.Clear()
        DrawLocation(character)
        AnsiConsole.Markup($"[aqua]H:[/][red]{character.Health}[/] [gray]|[/] [aqua]<Esc>[/]-menu [gray]|[/] [aqua]Arrows[/]-move/turn")
        If Not character.Location.Inventory.IsEmpty Then
            AnsiConsole.Markup(" [gray]|[/] [aqua]G[/]-ground")
        End If
        If Not character.Inventory.IsEmpty Then
            AnsiConsole.Markup(" [gray]|[/] [aqua]I[/]-inventory")
        End If
        Select Case Console.ReadKey(True).Key
            Case ConsoleKey.LeftArrow
                HandleTurnLeft(character)
            Case ConsoleKey.RightArrow
                HandleTurnRight(character)
            Case ConsoleKey.UpArrow
                HandleMoveAhead(character)
            Case ConsoleKey.G
                HandleGround(character)
            Case ConsoleKey.I
                HandleInventory(character)
            Case ConsoleKey.Escape
                character.State = PlayerState.GameMenu
        End Select
    End Sub

    Private Sub HandleGround(character As PlayerCharacter)
        If Not character.Location.Inventory.IsEmpty Then
            character.State = PlayerState.GroundInventory
        End If
    End Sub

    Private Sub HandleInventory(character As PlayerCharacter)
        If Not character.Inventory.IsEmpty Then
            character.State = PlayerState.Inventory
        End If
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
    Private ReadOnly LeftImages As New Dictionary(Of TransitionState, List(Of String)) From {
        {TransitionState.Wall, LeftWall},
        {TransitionState.Open, LeftDoor}
        }
    Private ReadOnly AheadImages As New Dictionary(Of TransitionState, List(Of String)) From {
        {TransitionState.Wall, AheadWall},
        {TransitionState.Open, AheadDoor}
        }
    Private ReadOnly RightImages As New Dictionary(Of TransitionState, List(Of String)) From {
        {TransitionState.Wall, RightWall},
        {TransitionState.Open, RightDoor}
        }

    Private Sub RenderLocationSection(canvas As Canvas, location As Location, column As Integer, direction As Direction, images As Dictionary(Of TransitionState, List(Of String)))
        Dim transition = location.GetTransition(direction)
        canvas.DrawImage(column, 0, images(transition.State), Color.White, Color.Black)
    End Sub
    Private Sub DrawLocation(character As PlayerCharacter)
        Dim canvas As New Canvas(ViewColumns, ViewRows)
        Dim location = character.Location
        RenderLocationSection(canvas, location, LeftColumn, character.LeftDirection, LeftImages)
        RenderLocationSection(canvas, location, AheadColumn, character.Direction, AheadImages)
        RenderLocationSection(canvas, location, RightColumn, character.RightDirection, RightImages)
        If Not location.Inventory.IsEmpty Then
            canvas.DrawImage(22, 14, ChestBackground, Color.SandyBrown, Nothing)
            canvas.DrawImage(22, 14, ChestForeground, Color.RosyBrown, Nothing)
        End If
        AnsiConsole.Clear()
        AnsiConsole.Write(canvas)
    End Sub
End Module
