Imports System.Runtime.CompilerServices

Public Enum Direction
    North
    East
    South
    West
End Enum
Public Module DirectionExtensions
    Public ReadOnly AllDirections As New List(Of Direction) From {
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West
        }
    <Extension()>
    Function NextColumn(direction As Direction, column As Long) As Long
        Select Case direction
            Case Direction.East
                Return column + 1
            Case Direction.West
                Return column - 1
            Case Else
                Return column
        End Select
    End Function
    <Extension()>
    Function NextRow(direction As Direction, row As Long) As Long
        Select Case direction
            Case Direction.South
                Return row + 1
            Case Direction.North
                Return row - 1
            Case Else
                Return row
        End Select
    End Function
    <Extension()>
    Function NextDirection(direction As Direction) As Direction
        Select Case direction
            Case Direction.North
                Return Direction.East
            Case Direction.East
                Return Direction.South
            Case Direction.South
                Return Direction.West
            Case Direction.West
                Return Direction.North
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
    <Extension()>
    Function PreviousDirection(direction As Direction) As Direction
        Select Case direction
            Case Direction.North
                Return Direction.West
            Case Direction.East
                Return Direction.North
            Case Direction.South
                Return Direction.East
            Case Direction.West
                Return Direction.South
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
End Module
