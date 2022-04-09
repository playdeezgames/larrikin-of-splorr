Imports System.Runtime.CompilerServices

Public Enum ItemType
    Potion
End Enum
Public Module ItemTypeExtensions
    Friend ReadOnly AllItemTypes As New List(Of ItemType) From {
        ItemType.Potion
        }
    <Extension()>
    Function SpawnDice(itemType As ItemType) As String
        Select Case itemType
            Case ItemType.Potion
                Return "4D1"
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
End Module