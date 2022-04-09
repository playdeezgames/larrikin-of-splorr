Module Features
    Friend ReadOnly ChestBackground As New List(Of String) From {
        "            ",
        "            ",
        "            ",
        "   X     X  ",
        "   X     X  ",
        "   X     X  ",
        "            ",
        " XXXXXXXXXXX",
        "            ",
        "     XXX    ",
        "      X     ",
        "            "
        }
    Friend ReadOnly ChestForeground As New List(Of String) From {
        "            ",
        "            ",
        "            ",
        "  X XXXXX X ",
        " XX XXXXX XX",
        " XX XXXXX XX",
        " XXXXXXXXXXX",
        "            ",
        " XXXXXXXXXXX",
        " XXXX   XXXX",
        " XXXXX XXXXX",
        " XXXXXXXXXXX"
        }
    Friend ReadOnly ForegroundImages As New Dictionary(Of FeatureType, List(Of String)) From {
        {FeatureType.Chest, ChestForeground}
        }
End Module
