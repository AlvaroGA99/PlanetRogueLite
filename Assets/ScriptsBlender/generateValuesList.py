data = ["A","B","C"]
list0 = "new List<string>() {"
list1 = "new List<string>() {"
list2 = "new List<string>() {"

restrictionType = "rocky"
restrictionCounter = 0;

for i in data :
    for j in data:
        for k in data:
            for l in data:
                for m in data:
                    for n in data:
                        
                        if restrictionType == "lakey":
                            if j=="C":
                                restrictionCounter += 1
                            if l=="C" :
                                restrictionCounter += 1
                            if n=="C" :
                                restrictionCounter += 1
                        elif restrictionType == "rocky":
                            if j=="A":
                                restrictionCounter += 1
                            if l=="A" :
                                restrictionCounter += 1
                            if n=="A" :
                                restrictionCounter += 1
                        else : 
                            if j=="B":
                                restrictionCounter += 1
                            if l=="B" :
                                restrictionCounter += 1
                            if n=="B" :
                                restrictionCounter += 1
                             
                        if(restrictionCounter != 1):
                            list0 += "\"" + i + j + k + "\","
                            list1 += "\"" +k + l + m + "\","
                            list2 += "\"" +m + n + i + "\","
                        restrictionCounter = 0
                        
                        
list0 += "}"
list1 += "}"
list2 += "}"

print(list2)