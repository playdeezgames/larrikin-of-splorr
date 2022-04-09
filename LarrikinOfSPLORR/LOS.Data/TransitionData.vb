Public Module TransitionData
    Friend Const TableName = "Transitions"
    Friend Const FromLocationIdColumn = "FromLocationId"
    Friend Const ToLocationIdColumn = "ToLocationId"
    Friend Const DirectionColumn = "Direction"
    Friend Const StateColumn = "State"
    Friend Sub Initialize()
        LocationData.Initialize()
        ExecuteNonQuery(
            $"CREATE TABLE IF NOT EXISTS [{TableName}]
            (
                [{FromLocationIdColumn}] INT NOT NULL,
                [{DirectionColumn}] INT NOT NULL,
                [{StateColumn}] INT NOT NULL,
                [{ToLocationIdColumn}] INT NOT NULL,
                UNIQUE([{FromLocationIdColumn}],[{DirectionColumn}]),
                FOREIGN KEY ([{FromLocationIdColumn}]) REFERENCES [{LocationData.TableName}]([{LocationData.LocationIdColumn}]),
                FOREIGN KEY ([{ToLocationIdColumn}]) REFERENCES [{LocationData.TableName}]([{LocationData.LocationIdColumn}])
            );")
    End Sub

    Public Sub WriteState(fromLocationId As Long, direction As Long, state As Long)
        Initialize()
        ExecuteNonQuery(
            $"UPDATE [{TableName}] SET [{StateColumn}]=@{StateColumn} WHERE [{FromLocationIdColumn}]=@{FromLocationIdColumn} AND [{DirectionColumn}]=@{DirectionColumn};",
            MakeParameter($"@{FromLocationIdColumn}", fromLocationId),
            MakeParameter($"@{DirectionColumn}", direction),
            MakeParameter($"@{StateColumn}", state))
    End Sub

    Public Function ReadState(fromLocationId As Long, direction As Long) As Long?
        Initialize()
        Return ExecuteScalar(Of Long)(
            $"SELECT [{StateColumn}] FROM [{TableName}] WHERE [{FromLocationIdColumn}]=@{FromLocationIdColumn} AND [{DirectionColumn}]=@{DirectionColumn};",
            MakeParameter($"@{FromLocationIdColumn}", fromLocationId),
            MakeParameter($"@{DirectionColumn}", direction))
    End Function

    Public Sub Write(fromLocationId As Long, toLocationId As Long, direction As Long, state As Long)
        Initialize()
        ExecuteNonQuery(
            $"REPLACE INTO [{TableName}]
            (
                [{FromLocationIdColumn}],
                [{ToLocationIdColumn}],
                [{DirectionColumn}],
                [{StateColumn}]
            ) 
            VALUES
            (
                @{FromLocationIdColumn},
                @{ToLocationIdColumn},
                @{DirectionColumn},
                @{StateColumn}
            );",
            MakeParameter($"@{FromLocationIdColumn}", fromLocationId),
            MakeParameter($"@{ToLocationIdColumn}", toLocationId),
            MakeParameter($"@{DirectionColumn}", direction),
            MakeParameter($"@{StateColumn}", state))
    End Sub
End Module
