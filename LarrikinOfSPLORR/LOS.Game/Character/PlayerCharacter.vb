Public Class PlayerCharacter
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
    Public Property Direction As Direction
        Get
            Return CType(PlayerData.ReadDirection().Value, Direction)
        End Get
        Set(value As Direction)
            PlayerData.WriteDirection(value)
        End Set
    End Property

    Public Function HasBeenTo(location As Location) As Boolean
        Return PlayerLocationData.Read(location.Id)
    End Function

    Public ReadOnly Property RightDirection As Direction
        Get
            Return Direction.NextDirection()
        End Get
    End Property

    Sub New()
        MyBase.New(PlayerData.ReadCharacter().Value)
    End Sub
    Public Function Move() As Boolean
        Dim transition = Location.GetTransition(Direction)
        If transition.State = TransitionState.Open Then
            Location = transition.ToLocation
            MarkMap()
            Return True
        End If
        Return False
    End Function
    Sub MarkMap()
        PlayerLocationData.Write(Location.Id)
    End Sub
End Class
