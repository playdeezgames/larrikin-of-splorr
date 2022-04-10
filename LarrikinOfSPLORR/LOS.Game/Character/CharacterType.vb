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
                Return 20
            Case CharacterType.Goblin
                Return 5
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
End Module