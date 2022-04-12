Public Class Character
    ReadOnly Property Id As Long
    Sub New(characterId As Long)
        Id = characterId
    End Sub
    Property Location As Location
        Get
            Return New Location(CharacterData.ReadLocation(Id).Value)
        End Get
        Set(value As Location)
            CharacterData.WriteLocation(Id, value.Id)
        End Set
    End Property
    ReadOnly Property IsEnemy As Boolean
        Get
            Return CharacterType.IsEnemy
        End Get
    End Property
    ReadOnly Property Name As String
        Get
            Return CharacterType.Name
        End Get
    End Property
    ReadOnly Property Inventory As Inventory
        Get
            Dim inventoryId = CharacterInventoryData.ReadForCharacter(Id)
            If inventoryId Is Nothing Then
                inventoryId = InventoryData.Create()
                CharacterInventoryData.Write(Id, inventoryId.Value)
            End If
            Return New Inventory(inventoryId.Value)

        End Get
    End Property
    ReadOnly Property CharacterType As CharacterType
        Get
            Return CType(CharacterData.ReadCharacterType(Id).Value, CharacterType)
        End Get
    End Property
    ReadOnly Property MaximumHealth As Long
        Get
            Return CharacterType.MaximumHealth
        End Get
    End Property
    Property Health As Long
        Get
            Return MaximumHealth - CharacterData.ReadWounds(Id).Value
        End Get
        Set(value As Long)
            If value > MaximumHealth Then
                value = MaximumHealth
            ElseIf value < 0 Then
                value = 0
            End If
            CharacterData.WriteWounds(Id, MaximumHealth - value)
        End Set
    End Property

    Public Sub Destroy()
        For Each item In Inventory.Items
            Location.Inventory.Add(item)
        Next
        CharacterData.Destroy(Id)
    End Sub

    ReadOnly Property AttackDice As String
        Get
            Return CharacterType.AttackDice
        End Get
    End Property
    ReadOnly Property DefendDice As String
        Get
            Return CharacterType.DefendDice
        End Get
    End Property
    ReadOnly Property IsDead As Boolean
        Get
            Return Health = 0
        End Get
    End Property
    ReadOnly Property CanUseItem As Boolean
        Get
            Return Inventory.Items.Any(Function(x) x.CanUse)
        End Get
    End Property
End Class
