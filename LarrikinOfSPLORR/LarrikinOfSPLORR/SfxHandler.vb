Module SfxHandler
    Sub HandleSfx(sfx As Sfx)
        Select Case sfx
            Case Sfx.Title
                PlayTitle()
            Case Else
                Throw New NotImplementedException
        End Select
    End Sub

    Private Sub PlayTitle()
        PlayStatement.Play("L500;C4;E4;G4;C5;G4;E4;C4")
    End Sub
End Module
