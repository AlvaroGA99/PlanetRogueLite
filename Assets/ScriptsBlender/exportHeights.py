import bpy
import math

class NoPathException(Exception):
    pass
class InvalidNamingException(Exception):
    pass

EXPORTNAME = "BBB"
PATH = "D:\clase\Master\TFM\ScriptsBlender\\"

#try :
    
if(len(EXPORTNAME) !=3 or not(("B" in EXPORTNAME) or ("A" in EXPORTNAME) or ("C" in EXPORTNAME) )):
    raise InvalidNamingException("La nomenclatura introducida no es valida. Introduzca una cadena de tres caracteres con las letras A,B o C para represesentar las adyacencias deseadas.")
    
if(PATH == ""):
    raise NoPathException("No ha introducido ninguna ruta del sistema en la que guardar el archivo. Especifique una para poder exportar el archivo.")
    
vertices = [
    
    (0.0, 1.0, 0.0),
    (math.sqrt(3) / 2, -0.5, 0.0),
    (-math.sqrt(3) / 2, -0.5, 0.0),
    
    
]

faces = [[0,1,2]]
repeatDict = {}
verticesIdToCopy = [0,1,2]

res = 3
for i in range(res) :
    lastfaces = faces.copy()
    faces.clear()
    for face in lastfaces :
        new0 = ((vertices[face[0]][0] +  vertices[face[1]][0])/2,(vertices[face[0]][1] +  vertices[face[1]][1])/2,(vertices[face[0]][2] +  vertices[face[1]][2])/2)
        new1 = ((vertices[face[1]][0] +  vertices[face[2]][0])/2,(vertices[face[1]][1] +  vertices[face[2]][1])/2,(vertices[face[1]][2] +  vertices[face[2]][2])/2)
        new2 = ((vertices[face[2]][0] +  vertices[face[0]][0])/2,(vertices[face[2]][1] +  vertices[face[0]][1])/2,(vertices[face[2]][2] +  vertices[face[0]][2])/2)
        
        if((face[0],face[1]) in repeatDict) :
            new0id = repeatDict[(face[0],face[1])]
        else :
            new0id = len(vertices)
            repeatDict[(face[0],face[1])] = new0id
            repeatDict[(face[1],face[0])] = new0id
            vertices.append(new0)
        verticesIdToCopy.append(new0id)
        
        if((face[1],face[2]) in repeatDict) :
            new1id = repeatDict[(face[1],face[2])]
        else :
            new1id = len(vertices)
            repeatDict[(face[1],face[2])] = new1id
            repeatDict[(face[2],face[1])] = new1id
            vertices.append(new1)
        verticesIdToCopy.append(new1id)
        
        if((face[2],face[0]) in repeatDict ) :
            new2id = repeatDict[(face[2],face[0])]
        else :
            new2id = len(vertices)
            repeatDict[(face[2],face[0])] = new2id
            repeatDict[(face[0],face[2])] = new2id
            vertices.append(new2)
        verticesIdToCopy.append(new2id)
        
        faces.append([new0id,face[1],new1id])
        faces.append([new2id,new1id,face[2]])
        faces.append([new0id,new1id,new2id])
        faces.append([face[0],new0id,new2id])

outputvert = []

if(len(bpy.context.selected_editable_objects) != 1):
    raise IndexError("No ha seleccionado ninguna geometría o la geometría seleccionad no es válida.")

for vert in bpy.context.selected_editable_objects[0].data.vertices.values() : 
    outputvert.append(vert.co[2])

file = open(PATH+ EXPORTNAME +".txt", "w")

for id in verticesIdToCopy :

    if(id >= len(outputvert)):
        raise IndexError("No ha seleccionado ninguna geometría o la geometría seleccionad no es válida.")
    file.write(str(outputvert[id])+"\n")

file.close()
