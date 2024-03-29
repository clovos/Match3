

Original notes:

# Match3

Start from the menu scene to play the game

.exe playable can be found in folder called Playable

I spent around 13 hours during 4 days.

I used an external package for the art called FruityFace.

Thoughts:
With expandability in mind I tried to take advantage of Unitys Scriptable Objects as much as I could to expose configurations in a more friendly environment then inside prefab components.

I decided after some research to try using a MVC pattern for the first time in Unity. I only have limited experience using a MVP pattern in Unity for testing purposes but the two are very similar. 
It was initially quite challenging to figure out how to integrate the mono behaviours in the pattern. I let the view class inherit from mono behaviour and let the controller listen to events from the view & model. This way the controller could recieve Unity specific callbacks from the mono behaviour.
I also choose to let the managers of the MVC factories inherit from mono behaviour so they could be attached to the scene.
The managers is also responsibly to provide the factories with the Scriptable Objects.

The implementation is not perfect by any means and I had to break a few rules of the pattern to get everything to work in the end but overall the responsibilites and the code structure was kept seperated and I believe it is quite easy to follow the flow and the dependencies. 

One challenge when decoupling the logic from the mono behaviours is asyncronous code. To solve this I've choosen to use coroutines with a time step index that can be used for indentifying at what time certain visual events should happen.

When starting to working on the session logic I quickly realized I needed some kind of reporting system from different parts if the game. I decided to implement a quick Pub/Sub messaging service that can be injected into the controllers so that the session logic could listen to events such as a successful match on the board. I made the messenging service into a singleton for simplicity reasons since I do not use a DI framework but I still inject it into the controller for dependency clearity and to align with the dependency inversion principle in the SOLID principles collection.

Thinks that could be improved in future iterations:
Add a DI framework to make code more modular.
The board model got a hard reference to the tile models. This is not ideal.
Board controller became quite heavy and would deserve some extractions.
The code has taking testing into account even though no tests was written but it would be very easy to add a DI framework and setup test coverage on the logic(Controllers).

Updated notes:

This is a older project of mine which is about 2 years old. I still think it shows the fundamentals of writing clean code in Unity. 
The key focus points here is to make the code modular, to follow the SOLID principles, to seperate the UI, logic and data and to utilize Unitys strengths.

Going through the project now does make me want to change a few things and I think this could be a good point of discussion for us.

I know that I was only suppose to provide a few hundred lines of code and this project is more than that so I will mention a few folders that we can focus on:

A way to structure a object from persistent data->UI:
Asset/Code/Level

A simple example of how to setup a modular service:
Asset/Code/Messaging

