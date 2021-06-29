baseType = "object"
types = {
    "bool",
    "int",
    "int2",
    "int3",
    "int4",
    "float",
    "float2",
    "float3",
    "float4",
    "string",
    "int[]",
    "string[]"
}


def convert(fileprefix, content):
    for type in types:
        typeForName = (type[0].upper() + type[1:]).replace("[]", "Array")
        
        newContent = content.replace("Master", typeForName)
        newContent = newContent.replace(baseType, type)
        newPublicEventFile = open(fileprefix + typeForName + ".cs", "w")
        newPublicEventFile.write(newContent)
        newPublicEventFile.close()


publicEventFile = open("PublicEventMaster.cs", "r")
convert("PublicEvent", publicEventFile.read())
publicEventFile.close()

publicFile = open("PublicMaster.cs", "r")
convert("Public", publicFile.read())
publicFile.close()



