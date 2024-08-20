# PlanetRogueLite 
#### Unity Version 2021.3.7f1

Deterministic procedural terrain planet generation system wrapped into a technical demo game, that features space travel and exploration through different biomes, fuel managment, and gravity mechanics. The system uses a Simple Tiled Wave Function Collapse based algorithm, that uses a spherical graph as the solution space, and get a solution using a set of terrain different portions or "modules" as the tile to reach succesful state. 

To design and export the modules to be used by the system, several Blender python scripts have been created, to create a basic module mesh with a certain polygonal resolution, edit it with the blender tools and finally export them as text file. 
