Module CommonUI
    Friend Const ViewColumns = 56
    Friend Const ScreenColumns = ViewColumns * 2
    Friend Const ViewRows = 28
    Friend Const ScreenRows = ViewRows + 2
    Friend Const OkText = "Ok"
    Friend Const NeverMindText = "Never Mind"
    Friend Sub OkPrompt()
        Dim prompt As New SelectionPrompt(Of String) With {.Title = ""}
        prompt.AddChoice(OkText)
        AnsiConsole.Prompt(prompt)
    End Sub
End Module
