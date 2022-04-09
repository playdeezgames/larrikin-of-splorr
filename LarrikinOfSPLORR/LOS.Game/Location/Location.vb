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
            Dim inventoryId = LocationInventoryData.ReadForLocation(Id)
            If inventoryId Is Nothing Then
                inventoryId = InventoryData.Create()
                LocationInventoryData.Write(Id, inventoryId.Value)
            End If
            Return New Inventory(inventoryId.Value)
        End Get
    End Property
End Class
