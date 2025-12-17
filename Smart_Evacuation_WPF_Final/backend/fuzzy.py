import numpy as np
import skfuzzy as fuzz

def evaluate_exit(distance, risk, capacity):
    """
    Fuzzy logic system to evaluate evacuation exit priority.
    Inputs range from 0 to 100.
    Output: priority score (0–1)
    """

    x = np.arange(0, 101, 1)

    # Fuzzy sets
    distance_near = fuzz.trimf(x, [0, 0, 50])
    distance_far = fuzz.trimf(x, [30, 100, 100])

    risk_low = fuzz.trimf(x, [0, 0, 50])
    risk_high = fuzz.trimf(x, [30, 100, 100])

    capacity_low = fuzz.trimf(x, [0, 0, 50])
    capacity_high = fuzz.trimf(x, [30, 100, 100])

    # Membership values
    d = fuzz.interp_membership(x, distance_near, distance)
    r = fuzz.interp_membership(x, risk_low, risk)
    c = fuzz.interp_membership(x, capacity_high, capacity)

    # Simple fuzzy rule aggregation
    score = d * r * c

    return round(float(score), 3)
