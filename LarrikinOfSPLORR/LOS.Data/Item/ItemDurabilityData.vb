Public Module ItemDurabilityData
    Friend Const TableName = "ItemDurabilities"
    Friend Const ItemIdColumn = ItemData.ItemIdColumn
    Friend Const DepletionColumn = "Depletion"
    Friend Sub Initialize()
        ItemData.Initialize()
        ExecuteNonQuery(
            $"CREATE TABLE IF NOT EXISTS [{TableName}]
            (
                [{ItemIdColumn}] INT NOT NULL UNIQUE,
                [{DepletionColumn}] INT NOT NULL,
                FOREIGN KEY ([{ItemIdColumn}]) REFERENCES [{ItemData.TableName}]([{ItemData.ItemIdColumn}])
            );")
    End Sub
    Public Function Read(itemId As Long) As Long?
        Return ReadColumnValue(Of Long)(AddressOf Initialize, TableName, ItemIdColumn, itemId, DepletionColumn)
    End Function

    Public Sub Write(itemId As Long, durability As Long)
        Initialize()
        ExecuteNonQuery(
            $"REPLACE INTO [{TableName}]([{ItemIdColumn}],[{DepletionColumn}]) VALUES(@{ItemIdColumn},@{DepletionColumn});",
            MakeParameter($"@{ItemIdColumn}", itemId),
            MakeParameter($"@{DepletionColumn}", durability))
    End Sub

    Friend Sub Clear(itemId As Long)
        Initialize()
        ExecuteNonQuery($"DELETE FROM [{TableName}] WHERE [{ItemIdColumn}]=@{ItemIdColumn};", MakeParameter($"@{ItemIdColumn}", itemId))
    End Sub
End Module
