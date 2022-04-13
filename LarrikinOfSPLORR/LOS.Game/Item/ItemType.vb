﻿Imports System.Runtime.CompilerServices

Public Enum ItemType
    Potion
    Compass
    Macguffin
    Dagger
    ShortSword
    LongSword
    Shield
End Enum
Public Module ItemTypeExtensions
    Public ReadOnly AllItemTypes As New List(Of ItemType) From {
        ItemType.Potion,
        ItemType.Compass,
        ItemType.Macguffin,
        ItemType.Dagger,
        ItemType.Shield,
        ItemType.ShortSword,
        ItemType.LongSword
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
            Case ItemType.Compass
                Return "compass"
            Case ItemType.Macguffin
                Return "macguffin"
            Case ItemType.Dagger
                Return "dagger"
            Case ItemType.ShortSword
                Return "shortsword"
            Case ItemType.LongSword
                Return "longsword"
            Case ItemType.Shield
                Return "shield"
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
    <Extension()>
    Function SpawnDice(itemType As ItemType) As String
        Select Case itemType
            Case ItemType.Potion
                Return "150D1"
            Case ItemType.Compass
                Return "1d1"
            Case ItemType.Macguffin
                Return "1d1"
            Case ItemType.Dagger
                Return "3d4"
            Case ItemType.ShortSword
                Return "2d4"
            Case ItemType.LongSword
                Return "1d4"
            Case ItemType.Shield
                Return "2d4"
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