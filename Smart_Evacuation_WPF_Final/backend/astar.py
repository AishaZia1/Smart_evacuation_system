import heapq

def astar(grid, fire_cells, congestion, start, goal):
    rows, cols = len(grid), len(grid[0])

    def heuristic(a, b):
        return abs(a[0] - b[0]) + abs(a[1] - b[1])

    open_set = []
    heapq.heappush(open_set, (0, start))

    came_from = {}
    g_score = {start: 0}

    explored = []

    while open_set:
        _, current = heapq.heappop(open_set)
        explored.append(current)

        if current == goal:
            path = []
            while current in came_from:
                path.append(list(current))
                current = came_from[current]
            path.append(list(start))
            path.reverse()
            return path, explored

        x, y = current
        for dx, dy in [(1,0),(-1,0),(0,1),(0,-1)]:
            nx, ny = x + dx, y + dy
            neighbor = (nx, ny)

            if nx < 0 or ny < 0 or nx >= cols or ny >= rows:
                continue
            if grid[ny][nx] == 1:
                continue
            if neighbor in fire_cells:
                continue

            cost = 1
            if neighbor in congestion:
                cost = 3

            tentative_g = g_score[current] + cost

            if neighbor not in g_score or tentative_g < g_score[neighbor]:
                came_from[neighbor] = current
                g_score[neighbor] = tentative_g
                f = tentative_g + heuristic(neighbor, goal)
                heapq.heappush(open_set, (f, neighbor))

    return [], explored
