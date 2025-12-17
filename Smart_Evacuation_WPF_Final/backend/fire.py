import random

def spread_fire(fire_cells, grid_size):
    new_fire = set(fire_cells)

    for (x, y) in fire_cells:
        for dx, dy in [(1,0),(-1,0),(0,1),(0,-1)]:
            nx, ny = x + dx, y + dy
            if 0 <= nx < grid_size and 0 <= ny < grid_size:
                if random.random() < 0.3:
                    new_fire.add((nx, ny))

    return new_fire
