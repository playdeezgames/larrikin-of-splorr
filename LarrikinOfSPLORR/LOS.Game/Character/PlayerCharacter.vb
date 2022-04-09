Public Class PlayerCharacter
    Inherits Character
    Property State As PlayerState
        Get
            Return CType(PlayerData.ReadState().Value, PlayerState)
        End Get
        Set(value As PlayerState)
            PlayerData.WriteState(value)
        End Set
    End Property
    Sub New()
        MyBase.New(PlayerData.ReadCharacter().Value)
    End Sub
End Class
