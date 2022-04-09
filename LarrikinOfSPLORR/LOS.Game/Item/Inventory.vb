Public Class Inventory
    ReadOnly Property Id As Long
    Sub New(inventoryId As Long)
        Id = inventoryId
    End Sub
    ReadOnly Property IsEmpty As Boolean
        Get
            Return Not Items.Any
        End Get
    End Property
    ReadOnly Property Items As List(Of Item)
        Get
            Return InventoryItemData.ReadForInventory(Id).Select(Function(id) New Item(id)).ToList
        End Get
    End Property

    Friend Sub Add(item As Item)
        InventoryItemData.Write(Id, item.Id)
    End Sub
End Class
