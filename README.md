## ![alt text][logo] Piper ##
[logo]: https://avatars0.githubusercontent.com/u/11473656?s=50

**Piper** is a code feature to procedurally generate a pipe terrain for a game. It is not a game within itself, mainly a base for one.

### Customizations ###

#### _General_ ####

- Initial Sections : The system adds a new section when the player leaves one, this ensures there is always more terrain ahead. However you can set the size of the initial map on load.
- Segments Per Ring : You can set any number from 3 onwards here to create anti-aliasing. The higher the number, the more circular the rings appear. (Around 50 there is hardly any aliasing)
- Ring Radius : This is simply for size. How large do you want the tunnel to be?

#### _Only applicable to original Spiral generation_ ####

- Sections Per Circle : How many rings do you want to generate in one spiral loop?
- Circle Radius : How large should the spiral be?

#### _Video_ ####

- Take a look at the demonstration video [here](https://youtu.be/48yya4OsCa0 "Link to the Youtube video").

### Project ###

#### _Code_ ####

- This project was made in Unity 5.3 in C#.
- This is released under the MIT License so feel free to clone the repository and open the `Piper` folder in Unity to look or edit.

#### _Path_ ####

- Currently the terrain is mapped in a spiral shape, however any change to the `NextRingCenter()` method will modify the general shape, and the system will fill in the rest for you.

#### _Character_ ####

- Some basic character movement is also added in to show examples of the generation in action.

### FrenchDwarf ###

For all things FrenchDwarf, check out [FrenchDwarf.com](http://www.FrenchDwarf.com)