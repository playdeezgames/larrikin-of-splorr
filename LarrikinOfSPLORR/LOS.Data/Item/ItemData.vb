﻿Public Module ItemData
    Friend Const TableName = "Items"
    Friend Const ItemIdColumn = "ItemId"
    Friend Const ItemTypeColumn = "ItemType"

    Friend Sub Initialize()
        ExecuteNonQuery(
            $"CREATE TABLE IF NOT EXISTS [{TableName}]
            (
                [{ItemIdColumn}] INTEGER PRIMARY KEY AUTOINCREMENT,
                [{ItemTypeColumn}] INT NOT NULL
            );")
    End Sub

    Public Function ReadItemType(itemId As Long) As Long?
        Return ReadColumnValue(Of Long)(AddressOf Initialize, TableName, ItemIdColumn, itemId, ItemTypeColumn)
    End Function

    Public Function Create(itemType As Long) As Long
        Initialize()
        ExecuteNonQuery(
            $"INSERT INTO [{TableName}]([{ItemTypeColumn}]) VALUES(@{ItemTypeColumn});",
            MakeParameter($"@{ItemTypeColumn}", itemType))
        Return LastInsertRowId
    End Function

    Public Sub Clear(itemId As Long)
        Initialize()
        InventoryItemData.ClearForItem(itemId)
        ItemDurabilityData.Clear(itemId)
        ExecuteNonQuery($"DELETE FROM [{TableName}] WHERE [{ItemIdColumn}]=@{ItemIdColumn};", MakeParameter($"@{ItemIdColumn}", itemId))
    End Sub
End Module
