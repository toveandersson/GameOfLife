# GameOfLife
My take on Conways game of life with frogs that I didn't paint but stole from another project (credit to Axel Björkman<3) 

![Movie_001](https://github.com/toveandersson/GameOfLife/assets/142992384/ee9eb6f0-54d0-473d-b844-3af40e6ca83a)

When the game had the right behaviour I tried to make some features in different areas to learn as much basics as possible, as for example about UI systems, basic code-structure/loops/functions, some about sound and saving settings between scenes, raycasting etc.

All of the features in the game are listed in the dropdown-list. There are options to clear the screen, regenerate, mute, and adjust speed. There are a few visual-preferences options like for example N for "no halfway down frogs" wich will make the "dying" cells dissappear (or be visible if you press again), and there are some different patterns which can be spawned by pressing the specific key for the pattern and then placing it by clicking the mousebutton. Pressing C will make any pressed button clear, otherwise you could spawn many patterns at once.

![Movie_016](https://github.com/toveandersson/GameOfLife/assets/142992384/2629ae51-553a-4d79-abdf-f9473f83083f)

A feature that doesn't show in the GIF:s is that the music changes pitch and tempo with the speed, and originally I wanted for the music to always match the tempo of the game but I found it hard to do and didn't have time to do it properly, but now it's mostly on beat unless you go too crazy with the speed. 

Some other ideas I had but didn't have time for was an option to go backwards in time for some generations, by saving data temporarily in an array of which cells has been alive for a number of generations back. When holding down the left arrow key the cells would get their alive or dead information from this array instead and do the simulation backwards for some time until the array is empty (or updated to the previous backwards stat's backwards stat's, wich will be the same stats which was saved and used backwards when holding the left arrow key, wich will result in the cells moving exactly as they did some time before the left-key was held down again).

A minor bug is that the pattern spawns at the mouseposition at the start of the new generation and not the place where you click, which becomes noticeable at slower frame-rates, especially when clicking at the beginning of a generation and moving the mouse before the next generation has spawned. While I believe I could solve this issue relatively easy with some googling, I didn't consider the bug noticeable enough to spend the time on it.
