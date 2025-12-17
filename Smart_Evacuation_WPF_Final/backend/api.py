from flask import Flask, request, jsonify
from backend.astar import astar
from backend.evaluator import evaluate_algorithm
from backend.fire import spread_fire

app = Flask(__name__)

fire_cells = {(4,4)}
congestion = {(2,2),(2,3),(3,3)}

@app.route("/astar", methods=["POST"])
def astar_api():
    global fire_cells

    data = request.json
    grid = data["grid"]
    start = tuple(data["start"])
    goal = tuple(data["goal"])

    fire_cells = spread_fire(fire_cells, len(grid))

    eval_result = evaluate_algorithm(
        astar, grid, fire_cells, congestion, start, goal
    )

    path, explored = eval_result["result"]

    return jsonify({
        "path": path,
        "explored": explored,
        "fire": list(map(list, fire_cells)),
        "congestion": list(map(list, congestion)),
        "runtime_ms": eval_result["runtime_ms"],
        "memory_kb": eval_result["memory_kb"]
    })

if __name__ == "__main__":
    app.run(port=5000, debug=True)
