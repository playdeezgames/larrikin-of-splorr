Public Module InventoryItemData
    Friend Const TableName = "InventoryItems"
    Friend Const InventoryIdColumn = InventoryData.InventoryIdColumn
    Friend Const ItemIdColumn = ItemData.ItemIdColumn
    Friend Sub Initialize()
        InventoryData.Initialize()
        ItemData.Initialize()
        ExecuteNonQuery(
            $"CREATE TABLE IF NOT EXISTS [{TableName}]
            (
                [{InventoryIdColumn}] INT NOT NULL,
                [{ItemIdColumn}] INT NOT NULL UNIQUE,
                FOREIGN KEY ([{InventoryIdColumn}]) REFERENCES [{InventoryData.TableName}]([{InventoryData.InventoryIdColumn}]),
                FOREIGN KEY ([{ItemIdColumn}]) REFERENCES [{ItemData.TableName}]([{ItemData.ItemIdColumn}])
            );")
    End Sub

    Public Sub Write(inventoryId As Long, itemId As Long)
        Initialize()
        ExecuteNonQuery(
            $"REPLACE INTO [{TableName}]
            (
                [{InventoryIdColumn}],
                [{ItemIdColumn}]
            ) 
            VALUES
            (
                @{InventoryIdColumn},
                @{ItemIdColumn}
            );",
            MakeParameter($"@{InventoryIdColumn}", inventoryId),
            MakeParameter($"@{ItemIdColumn}", itemId))
    End Sub

    Friend Sub ClearForItem(itemId As Long)
        Initialize()
        ExecuteNonQuery($"DELETE FROM [{TableName}] WHERE [{ItemIdColumn}]=@{ItemIdColumn};", MakeParameter($"@{ItemIdColumn}", itemId))
    End Sub

    Public Function ReadForInventory(inventoryId As Long) As List(Of Long)
        Initialize()
        Return ExecuteReader(
            Function(reader) CLng(reader(ItemIdColumn)),
            $"SELECT [{ItemIdColumn}] FROM [{TableName}] WHERE [{InventoryIdColumn}]=@{InventoryIdColumn};",
            MakeParameter($"@{InventoryIdColumn}", inventoryId))
    End Function
End Module
