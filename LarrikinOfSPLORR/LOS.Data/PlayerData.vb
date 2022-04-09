Public Module PlayerData
    Friend Const TableName = "Players"
    Friend Const PlayerIdColumn = "PlayerId"
    Friend Const CharacterIdColumn = CharacterData.CharacterIdColumn
    Friend Const StateColumn = "State"
    Friend Const DirectionColumn = "Direction"

    Friend Sub Initialize()
        CharacterData.Initialize()
        ExecuteNonQuery(
            $"CREATE TABLE IF NOT EXISTS [{TableName}]
            (
                [{PlayerIdColumn}],
                [{CharacterIdColumn}],
                [{StateColumn}],
                [{DirectionColumn}],
                CHECK([{PlayerIdColumn}]=1),
                FOREIGN KEY ([{CharacterIdColumn}]) REFERENCES [{CharacterData.TableName}]([{CharacterData.CharacterIdColumn}])
            );")
    End Sub

    Function ReadState() As Long?
        Return ReadColumnValue(Of Long)(AddressOf Initialize, TableName, PlayerIdColumn, 1, StateColumn)
    End Function

    Sub WriteState(state As Long)
        WriteColumnValue(AddressOf Initialize, TableName, PlayerIdColumn, 1, StateColumn, state)
    End Sub
    Public Sub DoStuff(characterId As Long, state As Long, direction As Long)
        Initialize()
        ExecuteNonQuery(
            $"REPLACE INTO [{TableName}]
            (
                [{PlayerIdColumn}],
                [{CharacterIdColumn}],
                [{StateColumn}],
                [{DirectionColumn}]
            ) 
            VALUES
            (
                1,
                @{CharacterIdColumn},
                @{StateColumn},
                @{DirectionColumn}
            );",
            MakeParameter($"@{CharacterIdColumn}", characterId),
            MakeParameter($"@{StateColumn}", state),
            MakeParameter($"@{DirectionColumn}", direction))
    End Sub

    Public Function ReadCharacter() As Long?
        Return ReadColumnValue(Of Long)(AddressOf Initialize, TableName, PlayerIdColumn, 1, CharacterIdColumn)
    End Function
End Module
