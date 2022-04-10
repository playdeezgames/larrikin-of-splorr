Module Characters
    Friend ReadOnly Goblin As List(Of String) = DoubleImage(New List(Of String) From {
        "            ",
        "            ",
        "            ",
        "            ",
        "    XXX     ",
        "     X X    ",
        "    XXX     ",
        "        X   ",
        "    X XX X  ",
        "   X  XX X  ",
        "     X X    ",
        "    XX  X   "
        })
    Friend Function GetImageForCharacterType(characterType As CharacterType) As List(Of String)
        Select Case characterType
            Case CharacterType.Goblin
                Return Goblin
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
    Friend Function GetColorForCharacterType(characterType As CharacterType) As Color
        Select Case characterType
            Case CharacterType.Goblin
                Return Color.Green
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
End Module
