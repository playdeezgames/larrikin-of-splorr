Public Class Transition
    ReadOnly Property FromLocationId As Long
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
End Class
