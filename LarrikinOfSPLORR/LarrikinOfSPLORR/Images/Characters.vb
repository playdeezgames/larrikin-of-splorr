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
    Friend ReadOnly Orc As List(Of String) = DoubleImage(New List(Of String) From {
        "            ",
        "            ",
        "            ",
        "    XXX X   ",
        "     X X    ",
        "    XXX     ",
        "       XX   ",
        "     XXX X  ",
        "   X XXX X  ",
        "     XXX    ",
        "     X X    ",
        "    XX X    "
        })
    Friend ReadOnly Skeleton As List(Of String) = DoubleImage(New List(Of String) From {
        "            ",
        "     XXX    ",
        "    XXXXX   ",
        "     X XX   ",
        "    XXXX    ",
        "    X X     ",
        "        X   ",
        "     XXX X  ",
        "   X  X  X  ",
        "     XXX    ",
        "     X X    ",
        "    XX X    "
        })
    Friend ReadOnly ChaosWarrior As List(Of String) = DoubleImage(New List(Of String) From {
        "            ",
        "    X  X    ",
        "   X    X   ",
        "    XXXXX   ",
        "       X    ",
        "    X X     ",
        "       XX   ",
        "     XXX X  ",
        "   X XXX X  ",
        "     XXX    ",
        "     X X    ",
        "    XX X    "
        })
    Friend Function GetImageForCharacterType(characterType As CharacterType) As List(Of String)
        Select Case characterType
            Case CharacterType.Goblin
                Return Goblin
            Case CharacterType.Orc
                Return Orc
            Case CharacterType.Skeleton
                Return Skeleton
            Case CharacterType.ChaosWarrior
                Return ChaosWarrior
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
    Friend Function GetColorForCharacterType(characterType As CharacterType) As Color
        Select Case characterType
            Case CharacterType.Goblin
                Return Color.Green
            Case CharacterType.Orc
                Return Color.DarkGreen
            Case CharacterType.Skeleton
                Return Color.LightSlateGrey
            Case CharacterType.ChaosWarrior
                Return Color.DarkSlateGray1
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
End Module
