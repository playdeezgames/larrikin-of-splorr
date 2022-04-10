Public Module CharacterData
    Friend Const TableName = "Characters"
    Friend Const CharacterIdColumn = "CharacterId"
    Friend Const LocationIdColumn = LocationData.LocationIdColumn
    Friend Const CharacterTypeColumn = "CharacterType"
    Friend Sub Initialize()
        ExecuteNonQuery(
            $"CREATE TABLE IF NOT EXISTS [{TableName}]
            (
                [{CharacterIdColumn}] INTEGER PRIMARY KEY AUTOINCREMENT,
                [{LocationIdColumn}] INT NOT NULL,
                [{CharacterTypeColumn}] INT NOT NULL,
                FOREIGN KEY ([{LocationIdColumn}]) REFERENCES [{LocationData.TableName}]([{LocationData.LocationIdColumn}])
            );")
    End Sub

    Public Sub WriteLocation(characteId As Long, locationId As Long)
        WriteColumnValue(AddressOf Initialize, TableName, CharacterIdColumn, characteId, LocationIdColumn, locationId)
    End Sub

    Public Function ReadLocation(characterId As Long) As Long?
        Return ReadColumnValue(Of Long)(AddressOf Initialize, TableName, CharacterIdColumn, characterId, LocationIdColumn)
    End Function

    Public Function Create(locationId As Long, characterType As Long) As Long
        Initialize()
        ExecuteNonQuery(
            $"INSERT INTO [{TableName}]
            (
                [{LocationIdColumn}],
                [{CharacterTypeColumn}]
            ) 
            VALUES
            (
                @{LocationIdColumn},
                @{CharacterTypeColumn}
            );",
            MakeParameter($"@{LocationIdColumn}", locationId),
            MakeParameter($"@{CharacterTypeColumn}", characterType))
        Return LastInsertRowId
    End Function
End Module
