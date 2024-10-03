# Planet Generation
#### Unity Version 2021.3.7f1

Deterministic procedural terrain planet generation system wrapped into a technical demo game, that features space travel and exploration through different biomes, fuel managment, and gravity mechanics. The system uses a Simple Tiled Wave Function Collapse based algorithm, that uses a spherical graph as the solution space, and get a solution using a set of terrain different portions or "modules" as the tile to reach succesful state. 

To design and export the modules to be used by the system, several Blender python scripts have been created, to create a basic module mesh with a certain polygonal resolution, edit it with the blender tools and finally export them as text file. The exported file name must use a naming determined and used by the module system. In general the naming is based on the height of the 3 sections in wich each side of the module is subdivided. The following image shows one example of naming : 

![nomenclaturas](https://github.com/user-attachments/assets/9b57c51c-4a64-4aff-a178-3559775d032a)

Finally the following link shows a video of the Blender scripts being used : 

https://drive.google.com/file/d/1XAgG_UtNIjnhw5-wRu1jRBuCYF7rA0Kc/view?usp=drive_link

