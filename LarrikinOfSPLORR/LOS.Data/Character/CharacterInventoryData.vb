Public Module CharacterInventoryData
    Friend Const TableName = "CharacterInventories"
    Friend Const CharacterIdColumn = "CharacterId"
    Friend Const InventoryIdColumn = InventoryData.InventoryIdColumn
    Friend Sub Initialize()
        InventoryData.Initialize()
        CharacterData.Initialize()
        ExecuteNonQuery(
            $"CREATE TABLE IF NOT EXISTS [{TableName}]
            (
                [{CharacterIdColumn}] INT NOT NULL UNIQUE,
                [{InventoryIdColumn}] INT NOT NULL,
                FOREIGN KEY ([{CharacterIdColumn}]) REFERENCES [{CharacterData.TableName}]([{CharacterData.CharacterIdColumn}]),
                FOREIGN KEY ([{InventoryIdColumn}]) REFERENCES [{InventoryData.TableName}]([{InventoryData.InventoryIdColumn}])
            );")
    End Sub

    Public Sub Write(locationId As Long, inventoryId As Long)
        Initialize()
        ExecuteNonQuery(
            $"REPLACE INTO [{TableName}]
            (
                [{CharacterIdColumn}],
                [{InventoryIdColumn}]
            ) 
            VALUES
            (
                @{CharacterIdColumn},
                @{InventoryIdColumn}
            );",
            MakeParameter($"@{CharacterIdColumn}", locationId),
            MakeParameter($"@{InventoryIdColumn}", inventoryId))
    End Sub

    Public Function ReadForCharacter(locationId As Long) As Long?
        Return ReadColumnValue(Of Long)(AddressOf Initialize, TableName, CharacterIdColumn, locationId, InventoryIdColumn)
    End Function
End Module
