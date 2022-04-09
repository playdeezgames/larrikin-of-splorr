Public Module CharacterData
    Friend Const TableName = "Characters"
    Friend Const CharacterIdColumn = "CharacterId"
    Friend Const LocationIdColumn = LocationData.LocationIdColumn
    Friend Sub Initialize()
        ExecuteNonQuery(
            $"CREATE TABLE IF NOT EXISTS [{TableName}]
            (
                [{CharacterIdColumn}] INTEGER PRIMARY KEY AUTOINCREMENT,
                [{LocationIdColumn}] INT NOT NULL,
                FOREIGN KEY ([{LocationIdColumn}]) REFERENCES [{LocationData.TableName}]([{LocationData.LocationIdColumn}])
            );")
    End Sub

    Public Sub WriteLocation(characteId As Long, locationId As Long)
        WriteColumnValue(AddressOf Initialize, TableName, CharacterIdColumn, characteId, LocationIdColumn, locationId)
    End Sub

    Public Function ReadLocation(characterId As Long) As Long?
        Return ReadColumnValue(Of Long)(AddressOf Initialize, TableName, CharacterIdColumn, characterId, LocationIdColumn)
    End Function

    Public Function Create(locationId As Long) As Long
        Initialize()
        ExecuteNonQuery(
            $"INSERT INTO [{TableName}]
            (
                [{LocationIdColumn}]
            ) 
            VALUES
            (
                @{LocationIdColumn}
            );",
            MakeParameter($"@{LocationIdColumn}", locationId))
        Return LastInsertRowId
    End Function
End Module
