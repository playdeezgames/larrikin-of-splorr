﻿Public Class PlayerCharacter
    Inherits Character
    Property State As PlayerState
        Get
            Return CType(PlayerData.ReadState().Value, PlayerState)
        End Get
        Set(value As PlayerState)
            PlayerData.WriteState(value)
        End Set
    End Property

    Public ReadOnly Property LeftDirection As Direction
        Get
            Return Direction.PreviousDirection()
        End Get
    End Property
    Public ReadOnly Property Direction As Direction
        Get
            Return CType(PlayerData.ReadDirection().Value, Direction)
        End Get
    End Property
    Public ReadOnly Property RightDirection As Direction
        Get
            Return Direction.NextDirection()
        End Get
    End Property

    Sub New()
        MyBase.New(PlayerData.ReadCharacter().Value)
    End Sub
End Class
