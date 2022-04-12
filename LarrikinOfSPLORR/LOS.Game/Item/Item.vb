Public Class Item
    ReadOnly Property Id As Long
    Sub New(itemId As Long)
        Id = itemId
    End Sub
    ReadOnly Property ItemType As ItemType
        Get
            Return CType(ItemData.ReadItemType(Id).Value, ItemType)
        End Get
    End Property
    ReadOnly Property Name As String
        Get
            Return ItemType.Name
        End Get
    End Property
    ReadOnly Property CanDrink As Boolean
        Get
            Return ItemType.CanDrink
        End Get
    End Property
    ReadOnly Property CanUse As Boolean
        Get
            Return CanDrink
        End Get
    End Property
    ReadOnly Property HealDice As String
        Get
            Return ItemType.HealDice
        End Get
    End Property

    Public Sub Destroy()
        ItemData.Clear(Id)
    End Sub
End Class
