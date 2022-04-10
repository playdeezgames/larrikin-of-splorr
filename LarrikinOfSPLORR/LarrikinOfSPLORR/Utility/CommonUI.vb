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
    Friend Function DoubleImage(image As List(Of String)) As List(Of String)
        Dim result As New List(Of String)
        For Each line In image
            Dim doubledLine = ""
            For Each cell In line
                doubledLine &= cell
                doubledLine &= cell
            Next
            result.Add(doubledLine)
            result.Add(doubledLine)
        Next
        Return result
    End Function
End Module
