# Realise 1.0.1
#### Bug fixes
* First block after if statement now place correctly.
* First block after "for" or "while" inside if statement now connecting correctly.
* PROGRAM and IN_OUT_PUT blocks now determined better.
* If statement (with "while" inside one of branches) end point now placeing correctly.

#### Encantments
* All primitive data types (and arrays of them) and "void" now removing from all code text.
* Short IN_OUT_PUT or PROCESS blocks texts is combined to one block.
* Main ("main()") method name will not be placed.
* Little user interface improvemens.


# Realise 1.0.2

### New
* Add "do-while" support.

#### Bug fixes
* Code comments now deleting from diagram correctly.
* "End" block now placing correctly.
* App will not stopeed if use array init (any demention) with '{' and '}'.
* Now Visio document closes correctly after finished building diagran.

#### Encantments
* Add spaces between equal sign and before method args.
* Modificate text in FOR block.
* Ìaribles initialization will be deleting.
* Namespaces before method call will be deleting.
* "||" and "&&" now replaceing by "or" and "and".
* Structures will be removed from diagram.
* Some UI updates.
* Don't need to restart program for each diagram building anymore.
* Global varibles now converting to PROCESS blocks;

### Important 
* [See updated code recomendations](https://github.com/DDmit04/DiagramConstructor#recomendation-for-better-app-work)

# Realise 1.0.3

### Bug fixes
* Fix converter bug wich delete part of "for" statement.
* Output only 'endl' block will not appear.

# Realise 1.0.4

### Bug fixes
* Too short statement branches now no longer crash programm.
* In IF and WHILE '==' now correctly replaceing by '='.
* Add spaces before and after 'or' and 'and'.
* Blocks after 'else-if' statement now placing correct.
* Do while after common process now placing correctly.

#### Encantments
* Add icon on .exe file

# Realise 1.1.0

### Program code
* Separate language config to other class.
* Improve code quality in DiagramBuilder class.

### Bug fixes
* Fix 'web effect' in large diagrams.
* 'do-while' statement as first block in 'if' or 'else' branch now placing correctly.

#### Encantments
* 'do-while' statement now placing closer to perv block.