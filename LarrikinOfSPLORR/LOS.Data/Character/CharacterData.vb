﻿Public Module CharacterData
    Friend Const TableName = "Characters"
    Friend Const CharacterIdColumn = "CharacterId"
    Friend Const LocationIdColumn = LocationData.LocationIdColumn
    Friend Const CharacterTypeColumn = "CharacterType"
    Friend Const WoundsColumn = "Wounds"
    Friend Sub Initialize()
        ExecuteNonQuery(
            $"CREATE TABLE IF NOT EXISTS [{TableName}]
            (
                [{CharacterIdColumn}] INTEGER PRIMARY KEY AUTOINCREMENT,
                [{LocationIdColumn}] INT NOT NULL,
                [{CharacterTypeColumn}] INT NOT NULL,
                [{WoundsColumn}] INT NOT NULL,
                FOREIGN KEY ([{LocationIdColumn}]) REFERENCES [{LocationData.TableName}]([{LocationData.LocationIdColumn}])
            );")
    End Sub

    Public Function ReadForLocation(locationId As Long) As List(Of Long)
        Initialize()
        Return ExecuteReader(Of Long)(
            Function(reader) CLng(reader(CharacterIdColumn)),
            $"SELECT [{CharacterIdColumn}] FROM [{TableName}] WHERE [{LocationIdColumn}]=@{LocationIdColumn};",
            MakeParameter($"@{LocationIdColumn}", locationId))
    End Function

    Public Sub WriteLocation(characteId As Long, locationId As Long)
        WriteColumnValue(AddressOf Initialize, TableName, CharacterIdColumn, characteId, LocationIdColumn, locationId)
    End Sub

    Public Function ReadCharacterType(characterId As Long) As Long?
        Return ReadColumnValue(Of Long)(AddressOf Initialize, TableName, CharacterIdColumn, characterId, CharacterTypeColumn)
    End Function

    Public Function ReadLocation(characterId As Long) As Long?
        Return ReadColumnValue(Of Long)(AddressOf Initialize, TableName, CharacterIdColumn, characterId, LocationIdColumn)
    End Function

    Public Function ReadWounds(characterId As Long) As Long?
        Return ReadColumnValue(Of Long)(AddressOf Initialize, TableName, CharacterIdColumn, characterId, woundsColumn)
    End Function

    Public Sub Destroy(characterId As Long)
        CharacterInventoryData.Destroy(characterId)
        ExecuteNonQuery(
            $"DELETE FROM [{TableName}] WHERE [{CharacterIdColumn}]=@{CharacterIdColumn};",
            MakeParameter($"@{CharacterIdColumn}", characterId))
    End Sub

    Public Sub WriteWounds(characterId As Long, wounds As Long)
        WriteColumnValue(AddressOf Initialize, TableName, CharacterIdColumn, characterId, WoundsColumn, wounds)
    End Sub

    Public Function Create(locationId As Long, characterType As Long) As Long
        Initialize()
        ExecuteNonQuery(
            $"INSERT INTO [{TableName}]
            (
                [{LocationIdColumn}],
                [{CharacterTypeColumn}],
                [{WoundsColumn}]
            ) 
            VALUES
            (
                @{LocationIdColumn},
                @{CharacterTypeColumn},
                0
            );",
            MakeParameter($"@{LocationIdColumn}", locationId),
            MakeParameter($"@{CharacterTypeColumn}", characterType))
        Return LastInsertRowId
    End Function
End Module
