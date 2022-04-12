﻿Module DeadState
    Friend Sub Run(character As PlayerCharacter)
        AnsiConsole.Clear()
        AnsiConsole.MarkupLine("                                                        
               .^7J5GB#&&&&&&&&&#GPJ7^.                 
         .~JB&@@@@@@@@@@@@@@@@@@@@@@@@@@#P!.            
        !@@@@@@@@@@@@&#GY?!^::...  ....:^~77^           
         G@@@@@@&G?^.         :P&P^                     
          P@@#!.            :#@@@@@#^                   
           J@B             5@@@@@@@@@5                  
            ^&@7         .#@@@@@@@@@@@#.                
              Y@&^      ^@@@@@@@@@@@@@@#                
               .P@#~  .G@@@@@@@@@@@@@G~.                
                 &@@&G@@@@@@@@@@@@@Y                    
                 @@@@@@@@@@@@@@@@@@@:                   
                .@@@@@@@@@@@@@@@@@@@@:                  
                 ?G&@@@@@@@@@@@@@@@@@@.                 
                    .~Y#@@@@@@@@@@@@@@@.                
                       ?@@@@@@@@@@@@@@@&                
                      .@@@@@@@@@@@@@@B!.                
                      G@@@@@@@@@@@@@@B.                 
                     ^@@@@@@@@@@@@@@?&@Y                
                     #@@@@@@@@@@@@@@G ?@&:              
                    ~@@@@@@@@@@@@@@@@: .&@7             
                    &@@@@@@@@@@@@@@@@B   G@P            
                   !@@@@@@@@@@@@@@@@@@:   Y@J           
                   &@@@@@@@@@@@@@@@@@@B    .            
                  ^&&&&&&&&&&&&&&&&&&&&.                ")
        SfxPlayer.Play(Sfx.Death)
        AnsiConsole.MarkupLine("[red]Yer dead.[/]")
        OkPrompt()
    End Sub
End Module
