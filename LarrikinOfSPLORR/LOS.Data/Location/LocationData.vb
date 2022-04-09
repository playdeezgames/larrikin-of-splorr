Public Module LocationData
    Friend Const TableName = "Locations"
    Friend Const LocationIdColumn = "LocationId"
    Friend Const ColumnColumn = "Column"
    Friend Const RowColumn = "Row"
    Public ReadOnly Property All As List(Of Long)
        Get
            Return ReadAllIds(AddressOf Initialize, TableName, LocationIdColumn)
        End Get
    End Property

    Friend Sub Initialize()
        ExecuteNonQuery(
            $"CREATE TABLE IF NOT EXISTS [{TableName}]
            (
                [{LocationIdColumn}] INTEGER PRIMARY KEY AUTOINCREMENT,
                [{ColumnColumn}] INT NOT NULL,
                [{RowColumn}] INT NOT NULL,
                UNIQUE([{ColumnColumn}],[{RowColumn}])
            );")
    End Sub
    Public Function Create(column As Long, row As Long) As Long
        Initialize()
        ExecuteNonQuery(
            $"INSERT INTO [{TableName}]([{ColumnColumn}],[{RowColumn}]) VALUES(@{ColumnColumn},@{RowColumn});",
            MakeParameter($"@{ColumnColumn}", column),
            MakeParameter($"@{RowColumn}", row))
        Return LastInsertRowId
    End Function

    Public Function ReadByColumnAndRow(column As Long, row As Long) As Long?
        Initialize()
        Return ExecuteScalar(Of Long)(
            $"SELECT [{LocationIdColumn}] FROM [{TableName}] WHERE [{ColumnColumn}]=@{ColumnColumn} AND [{RowColumn}]=@{RowColumn};",
            MakeParameter($"@{ColumnColumn}", column),
            MakeParameter($"@{RowColumn}", row))
    End Function
End Module
