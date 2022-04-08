Public Module Game
    Sub NewGame()
        Store.Reset()
        Dim characterId = CharacterData.Create()
        PlayerData.DoStuff(characterId, PlayerState.Exploration)
    End Sub
    Sub Finish()
        Store.ShutDown()
    End Sub
End Module
