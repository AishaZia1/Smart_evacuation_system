🚨 Smart Evacuation Planning System

A* Search + Fuzzy Logic | Python Backend + WPF Frontend

📌 Project Summary

The Smart Evacuation Planning System is an AI-based application that computes safe evacuation routes during emergencies. It uses A* search for optimal pathfinding and fuzzy logic to evaluate risk under uncertainty, with real-time visualization and explainable AI decisions.

🎯 Objectives

Implement A* for optimal evacuation routing

Apply fuzzy logic for risk-based evaluation

Support dynamic hazards (fire spread)

Provide interactive visualization and explainability

Measure runtime, memory, and path efficiency

🧠 AI Techniques Used
A* Search

Finds shortest safe path

Avoids obstacles and fire

Replans when environment changes

Fuzzy Logic

Evaluates evacuation quality using distance, risk, and capacity

Produces a safety score for decision analysis

🏗 System Architecture
/docs        → Proposal, Design, Evaluation
/src
 ├── backend → Python (A*, Fuzzy Logic)
 └── frontend → WPF (C#)
/results     → Graphs & outputs


Frontend communicates with backend using JSON.

🖥 Frontend (WPF)

Interactive grid map

Car evacuation animation

Fire, trees, road visualization

Click to add/remove obstacles

Live charts for performance metrics

Explainability panel

⚙ Backend (Python)

A* pathfinding implementation

Fuzzy logic evaluation

Runtime & memory measurement

Decision explainability

📊 Evaluation Metrics

Runtime (ms)

Memory usage (KB)

Path length

Fuzzy safety score

Results are shown in tables and live graphs.

▶ How to Run
Backend
cd src/backend
python api.py

Frontend
cd src/frontend/EvacuationWPF
dotnet restore
dotnet run

Member 1	XXXX
Member 2	XXXX
