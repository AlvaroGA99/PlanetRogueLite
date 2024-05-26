import bpy
import math
#import threading
#import time


def GetImportedHeidghts(id):
    wedges = []
    centres = []
    return wedges[id] + centres[id]
#import logging
#logging.basicConfig(level=logging.DEBUG,format='(%(threadName)-10s) %(message)s',)

#bpy.ops.wm.read_factory_settings(use_empty=True)

mesh = bpy.data.meshes.new("EquilateralTriangle")
obj = bpy.data.objects.new("EquilateralTriangle", mesh)

scene = bpy.context.scene
scene.collection.objects.link(obj)
obj.select_set(True)

obj.location = (0, 0, 0)

#para las variaciones de rotacion cambiar la distribucion original de vertices

vertices = [
    
    (0.0, 1.0, 0.0),
    (math.sqrt(3) / 2, -0.5, 0.0),
    (-math.sqrt(3) / 2, -0.5, 0.0),
    
    
]

faces = [[0,1,2]]
repeatDict = {}
#verticesIdToCopy = [0,1,2]

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
#        verticesIdToCopy.append(new0id)
        
        if((face[1],face[2]) in repeatDict) :
            new1id = repeatDict[(face[1],face[2])]
        else :
            new1id = len(vertices)
            repeatDict[(face[1],face[2])] = new1id
            repeatDict[(face[2],face[1])] = new1id
            vertices.append(new1)
#        verticesIdToCopy.append(new1id)
        
        if((face[2],face[0]) in repeatDict ) :
            new2id = repeatDict[(face[2],face[0])]
        else :
            new2id = len(vertices)
            repeatDict[(face[2],face[0])] = new2id
            repeatDict[(face[0],face[2])] = new2id
            vertices.append(new2)
#        verticesIdToCopy.append(new2id)
        
        faces.append([new0id,face[1],new1id])
        faces.append([new2id,new1id,face[2]])
        faces.append([new0id,new1id,new2id])
        faces.append([face[0],new0id,new2id])

#despues de ejecutar el scriipt ejecutar por consola lo siguiente :
# modificar la mesh y ejecutar en consola

# for vert in bpy.context.selected_editable_objects[0].data.vertices.values() : print(vert.co[2],",")
#print("//////////////////////////////")
#heightsToAdd = [0.0 ,
#0.0 ,
#0.0 ,
#-1.0 ,
#-1.0 ,
#-1.0 ,
#0.0 ,
#0.0 ,
#-1.0 ,
#-1.0 ,
#0.0 ,
#0.0 ,
#-1.0 ,
#0.0 ,
#0.0 ,
#0.0 ,
#0.0 ,
#0.0 ,
#0.0 ,
#-1.0 ,
#-1.0 ,
#0.0 ,
#-1.0 ,
#-1.0 ,
#-1.0 ,
#-1.0 ,
#0.0 ,
#0.0 ,
#0.0 ,
#0.0 ,
#0.0 ,
#-1.0 ,
#-1.0 ,
#-1.0 ,
#-1.0 ,
#-1.0 ,
#-1.0 ,
#-1.0 ,
#-1.0 ,
#0.0 ,
#0.0 ,
#-1.0 ,
#0.0 ,
#0.0 ,
#0.0]

#for i in range(len(heightsToAdd)) :
#    vertices[i] = (vertices[i][0],vertices[i][1],vertices[i][2] )

#finalAddedHeights = []

#for id in verticesIdToCopy :
#    #print(str(id))
#    print(vertices[id][2])
#  #  vertices[id] = (0,0,0 + heightsToAdd[id])
    
    

mesh.from_pydata(vertices, [], faces)
mesh.update()
obj.select_set(False)