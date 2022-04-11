Module DeadState
    Friend Sub Run(character As PlayerCharacter)
        AnsiConsole.Clear()
        AnsiConsole.MarkupLine("Yer dead.")
        Play("L500")
        OkPrompt()
    End Sub
End Module
