Public Class Location
    ReadOnly Property Id As Long
    Sub New(locationId As Long)
        Id = locationId
    End Sub

    Public Function GetTransition(direction As Direction) As Transition
        Return New Transition(Id, direction)
    End Function
    ReadOnly Property Inventory As Inventory
        Get
            Return New Inventory(LocationInventoryData.ReadForLocation(Id).Value)
        End Get
    End Property
End Class
