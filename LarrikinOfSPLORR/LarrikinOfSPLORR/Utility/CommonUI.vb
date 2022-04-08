Module CommonUI
    Friend Const OkText = "Ok"
    Friend Sub OkPrompt()
        Dim prompt As New SelectionPrompt(Of String) With {.Title = ""}
        prompt.AddChoice(OkText)
        AnsiConsole.Prompt(prompt)
    End Sub
End Module
