import sys
import json
from astar import astar
from fuzzy import evaluate_exit

if __name__ == "__main__":
    data = json.loads(sys.argv[1])

    path, explored = astar(
        data["grid"],
        set(tuple(f) for f in data["fire"]),
        set(),
        tuple(data["start"]),
        tuple(data["goal"])
    )

    # Example fuzzy inputs
    fuzzy_score = evaluate_exit(
        distance=40,
        risk=30,
        capacity=80
    )

    print(json.dumps({
        "path": path,
        "runtime_ms": len(explored) * 1.5,
        "memory_kb": len(explored) * 0.8,
        "fuzzy": fuzzy_score
    }))
