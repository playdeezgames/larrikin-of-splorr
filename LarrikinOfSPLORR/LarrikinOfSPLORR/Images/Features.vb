Module Features
    Friend ReadOnly Chest As New List(Of String) From {
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
    Friend ReadOnly FeatureImages As New Dictionary(Of FeatureType, List(Of String)) From {
        {FeatureType.Chest, Chest}
        }
End Module
