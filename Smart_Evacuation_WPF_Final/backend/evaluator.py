import time
import tracemalloc

def evaluate_algorithm(func, *args):
    """
    Evaluates an algorithm by measuring:
    - runtime (ms)
    - peak memory usage (KB)
    """

    tracemalloc.start()
    start_time = time.time()

    result = func(*args)

    end_time = time.time()
    current, peak = tracemalloc.get_traced_memory()
    tracemalloc.stop()

    return {
        "result": result,
        "runtime_ms": round((end_time - start_time) * 1000, 3),
        "memory_kb": round(peak / 1024, 3)
    }
