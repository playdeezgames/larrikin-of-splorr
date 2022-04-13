Public Module PlayerLocationData
    Friend Const TableName = "PlayerLocations"
    Friend Const LocationIdColumn = LocationData.LocationIdColumn
    Friend Sub Initialize()
        LocationData.Initialize()
        ExecuteNonQuery(
            $"CREATE TABLE IF NOT EXISTS [{TableName}]
            (
                [{LocationIdColumn}] INT NOT NULL UNIQUE,
                FOREIGN KEY ([{LocationIdColumn}]) REFERENCES [{LocationData.TableName}]([{LocationData.LocationIdColumn}])
            );")
    End Sub
    Public Function Read(locationId As Long) As Boolean
        Return ReadColumnValue(Of Long)(AddressOf Initialize, TableName, LocationIdColumn, locationId, LocationIdColumn).HasValue
    End Function
    Public Sub Write(locationId As Long)
        Initialize()
        ExecuteNonQuery($"REPLACE INTO [{TableName}]([{LocationIdColumn}]) VALUES(@{LocationIdColumn});", MakeParameter($"@{LocationIdColumn}", locationId))
    End Sub
End Module
