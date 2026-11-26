# Quirks of the original, and what the port did with them

The Odin port keeps the VB.NET original's rules, screens and text (LarrikinOfSPLORR/). Where the original had a quirk, it is listed here with what the port does. "Kept" means the port behaves as the 2022 game did.

## Kept

- **Overhealing is not prevented** (the itch page's own "Deliberate Design Decision"): a potion drunk at full health is used up and heals nothing. Health never goes above 5.
- **The dice are mostly misses.** The larrikin's attack is `1d3/3` (1 on a three, else 0) plus the weapon's `1D2/2` terms; enemies defend with `1d6/6` terms. Many rounds end "You roll an attack of 0!". This is the design.
- **Weapons and the shield wear by the attack roll**, including the roll they made in the same round: a dagger breaks once its attacks add up to 10, a shortsword 20, a longsword 30; the shield wears by each enemy attack roll and breaks at 10. The best weapon (highest maximum damage, the first carried among equals) is the one used.
- **After the enemy is killed** the screen waits for Ok, then the rest of the room attacks on the same screen, then Ok again. The separator line is printed for every enemy even after the larrikin is dead.
- **Run** turns to a random direction first: a door there means escape; a wall means every enemy in the room attacks. The facing stays changed either way.
- **Enemies and items can start in the first room**, so a run can open with a fight.
- **Only the first enemy in a room is drawn**; the status line gives the count.
- **The map shows only rooms visited**, and walls between a visited room and an unvisited one show as closed.
- **Lists show one line per item**, so fifty potions are fifty "potion" lines (Spectre's ten-line page with "(Move up and down to reveal more choices)" is kept).
- **Picking up the macguffin wins at once**, the moment it is in the inventory.
- **A loaded game opens on the game menu**, because the original saved from there.
- Tunes play one after another, as `Console.Beep` blocked, so a long fight queues its beeps.

## Changed

- **Saving**: the original asked for a file name and saved its SQLite database there. The port has one browser slot (localStorage `los:save`): **Save Game...** writes it and says "Saved in this browser."; **Load...** reads it ("Loading didn't work. I blame Azure." when there is none).
- **Quit** is gone from the main menu (a browser tab has nothing to quit to); **Full Screen** takes its place.
- **Escape in a list** picks Never Mind (or Continue, or No) where the list has one. The original's prompts ignored Escape.
- **The abandon confirmation** is a No/Yes list (Y and N keys still work), so it can be answered on a phone.
- **Choosing a duplicate** in a list takes the one highlighted. The original took the first with that name (or, in the fight list, the last enemy with the same name and health); the results are the same apart from which weapon's wear is carried.
- **The screen**: the original was a 112 by 30 console where a canvas pixel is two characters. The port draws a 448 by 240 picture: the view as 8 by 8 blocks and text in the DINOBYTE font (74 columns). The figlet titles are DINOBYTE at four times size. The "|" glyph in DINOBYTE is a broken bar.
- **Sounds**: `Console.Beep` is replaced by square-wave notes in the page at the same pitches and lengths.
- **About** gains two lines: the port credit and the DINOBYTE credit.
- The random number generator is the port's own (SplitMix64), so a seed does not reproduce a 2022 dungeon.
