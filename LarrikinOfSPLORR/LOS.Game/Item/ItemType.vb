Imports System.Runtime.CompilerServices

Public Enum ItemType
    Potion
End Enum
Public Module ItemTypeExtensions
    Public ReadOnly AllItemTypes As New List(Of ItemType) From {
        ItemType.Potion
        }
    <Extension()>
    Function DrinkSfx(itemType As ItemType) As Sfx?
        Select Case itemType
            Case ItemType.Potion
                Return Sfx.HealthUp
            Case Else
                Return Nothing
        End Select
    End Function
    <Extension()>
    Function Name(itemType As ItemType) As String
        Select Case itemType
            Case ItemType.Potion
                Return "potion"
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
    <Extension()>
    Function SpawnDice(itemType As ItemType) As String
        Select Case itemType
            Case ItemType.Potion
                Return "4D1"
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
    <Extension()>
    Function HealDice(itemType As ItemType) As String
        Select Case itemType
            Case ItemType.Potion
                Return "2D3"
            Case Else
                Return "0D1"
        End Select
    End Function
    <Extension()>
    Function CanDrink(itemType As ItemType) As Boolean
        Select Case itemType
            Case ItemType.Potion
                Return True
            Case Else
                Return False
        End Select
    End Function
End Module