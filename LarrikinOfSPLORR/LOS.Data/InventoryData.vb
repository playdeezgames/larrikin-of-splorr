Public Module InventoryData
    Friend Const TableName = "Inventories"
    Friend Const InventoryIdColumn = "InventoryId"

    Friend Sub Initialize()
        ExecuteNonQuery($"CREATE TABLE IF NOT EXISTS [{TableName}]([{InventoryIdColumn}] INTEGER PRIMARY KEY AUTOINCREMENT);")
    End Sub
End Module
