Imports System.Runtime.CompilerServices

Public Enum CharacterType As Long
    Larrikin
End Enum
Module CharacterTypeExtensions
    <Extension()>
    Function MaximumHealth(characterType As CharacterType) As Long
        Select Case characterType
            Case CharacterType.Larrikin
                Return 20
            Case Else
                Throw New NotImplementedException
        End Select
    End Function
End Module