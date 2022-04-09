Public Class Transition
    Private ReadOnly Property FromLocationId As Long
    ReadOnly Property FromLocation As Location
        Get
            Return New Location(FromLocationId)
        End Get
    End Property
    ReadOnly Property Direction As Direction
    Sub New(fromLocationId As Long, direction As Direction)
        Me.FromLocationId = fromLocationId
        Me.Direction = direction
    End Sub
    Property State As TransitionState
        Get
            Return CType(TransitionData.ReadState(FromLocationId, Direction).Value, TransitionState)
        End Get
        Set(value As TransitionState)
            TransitionData.WriteState(FromLocationId, Direction, value)
        End Set
    End Property
    ReadOnly Property ToLocation As Location
        Get
            Return New Location(TransitionData.ReadToLocation(FromLocationId, Direction).Value)
        End Get
    End Property
End Class
