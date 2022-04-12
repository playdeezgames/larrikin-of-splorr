Imports System.Runtime.CompilerServices

Public Enum CharacterType As Long
    Larrikin
    Goblin
End Enum
Module CharacterTypeExtensions
    Friend ReadOnly AllEnemyTypes As New List(Of CharacterType) From {
        CharacterType.Goblin
        }
    <Extension()>
    Function MaximumHealth(characterType As CharacterType) As Long
        Select Case characterType
            Case CharacterType.Larrikin
                Return 5
            Case CharacterType.Goblin
                Return 1
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
    <Extension()>
    Function Name(characterType As CharacterType) As String
        Select Case characterType
            Case CharacterType.Larrikin
                Return "larrikin"
            Case CharacterType.Goblin
                Return "goblin"
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
    <Extension()>
    Function AttackDice(characterType As CharacterType) As String
        Select Case characterType
            Case CharacterType.Larrikin
                Return "1d3/3"
            Case CharacterType.Goblin
                Return "1d3/3"
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
    <Extension()>
    Function DefendDice(characterType As CharacterType) As String
        Select Case characterType
            Case CharacterType.Larrikin
                Return "1d3/3+1d3/3"
            Case CharacterType.Goblin
                Return "1d6/6"
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
    <Extension()>
    Function SpawnDice(characterType As CharacterType) As String
        Select Case characterType
            Case CharacterType.Goblin
                Return "2d6"
            Case Else
                Return "0d1"
        End Select
    End Function
    <Extension()>
    Function IsEnemy(characterType As CharacterType) As Boolean
        Return characterType <> CharacterType.Larrikin
    End Function
End Module