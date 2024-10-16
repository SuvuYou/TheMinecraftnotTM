length = 3
width = 3
height = 10
array_size = length * width * height

for index in range(array_size):
    x = index % length
    y = index // (length * width) 
    z = (index // length) % width
    print(f"Index: {index}, Coordinates: (x: {x}, y: {y}, z: {z})")