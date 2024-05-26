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
countDict = {}
verticesIdToCopy = [0,1,2]
notBorderVertex = []

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
        
        if((face[1],face[2]) in repeatDict):
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

#despues de ejecutar el scriipt ejecutar por consola lo siguiente :
# modificar la mesh y ejecutar en consola
# for vert in bpy.context.selected_editable_objects[0].data.vertices.values() : print(vert.co[2],",")

border = []

for face in faces:
    if face[0] in countDict: 
        countDict[face[0]] += 1
    else:
        countDict[face[0]] = 1
    if face[1] in countDict: 
        countDict[face[1]] += 1
    else:
        countDict[face[1]] = 1
    if face[2] in countDict: 
        countDict[face[2]] += 1
    else:
        countDict[face[2]] = 1
    
topVertex = 0
topToRightVertices = []
rightVertex = 1
rightToLeftVertices = []
leftVertex = 2
leftToTopVertices = []

originalLength = len(vertices)

for i in range(originalLength) :
    if (not countDict[i] > 3):
        border.append(i)
    vertices.append((vertices[i][0],vertices[i][1],vertices[i][2] - 2))
    
for i in range(len(border)):
    if  vertices[border[i]][1] < 1 and  vertices[border[i]][1] > -0.5:
        if vertices[border[i]][0] > 0 :      
            topToRightVertices.append(border[i])
        elif vertices[border[i]][0] < 0 :
            leftToTopVertices.append(border[i])
    elif vertices[border[i]][1] == -0.5 and vertices[border[i]][0] > -math.sqrt(3) / 2 and vertices[border[i]][0] < math.sqrt(3) / 2 :
        rightToLeftVertices.append(border[i])
        
topToRightVertices.sort(key = lambda x : vertices[x][0])
rightToLeftVertices.sort(key = lambda x : vertices[x][0],reverse = True) 
leftToTopVertices.sort(key = lambda x : vertices[x][0])


facesToAdd = []
border = []

for face in faces:
    facesToAdd.append([face[0] + originalLength,face[2] + originalLength,face[1] + originalLength])
border.append(topVertex)

for v in topToRightVertices:
    border.append(v)
    
border.append(rightVertex)

for v in rightToLeftVertices:
    border.append(v)
    
border.append(leftVertex)

for v in leftToTopVertices:
    border.append(v)

for i in range(len(border)) :
   
    next = i + 1;
    if next == len(border):
        next = 0
    facesToAdd.append([border[i],border[i] + originalLength,border[next] + originalLength])
    facesToAdd.append([border[next],border[i],border[next] + originalLength])

#print("BORDER : ",border)

for face in facesToAdd :
    faces.append(face)
    
print("------------------------------------------------------------------------------------")
for i in facesToAdd:
    print(i[0],end=",")
    print(i[1],end=",")
    print(i[2],end=",")
mesh.from_pydata(vertices, [], faces)
mesh.update()
obj.select_set(False)