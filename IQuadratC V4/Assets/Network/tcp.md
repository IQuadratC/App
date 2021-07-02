
# Message:
2 byte uint16 -> id
2 byte uint16 -> datalength
data

# ids:

1 = debug text
10 = last message not completly recived.

100 = disconnect to server
101 = disconnect recived

# standerts:
Allways BigLittleEndian
UTF8
