using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls;

using System.Diagnostics;
using System.Text.Json;


namespace EvacuationWPF
{
    public partial class MainWindow : Window
    {
        const int GRID = 20;
        const int CELL = 30;

        int[,] map = new int[GRID, GRID]; // 0 free, 1 wall, 2 fire, 3 shelter
        (int x, int y) evacuee = (0, 0);
        (int x, int y) shelter = (19, 19);

        DispatcherTimer timer = new DispatcherTimer();
        Random rand = new Random();

        List<int> runtimeHistory = new();

        public MainWindow()
        {
            InitializeComponent();
            InitMap();

            timer.Interval = TimeSpan.FromMilliseconds(450);
            timer.Tick += (s, e) =>
            {
                SpreadFire();
                MoveEvacuee();
                Draw();
            };

            Draw();
        }

        void Start_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }

        // ================= INIT =================
        void InitMap()
        {
            for (int x = 0; x < GRID; x++)
                for (int y = 0; y < GRID; y++)
                    map[x, y] = 0;

            map[shelter.x, shelter.y] = 3;
        }

        // ================= DRAW =================
        void Draw()
        {
            MapCanvas.Children.Clear();

            for (int x = 0; x < GRID; x++)
            {
                for (int y = 0; y < GRID; y++)
                {
                    Brush b = Brushes.DimGray;
                    if (map[x, y] == 1) b = Brushes.Black;
                    if (map[x, y] == 2) b = Brushes.IndianRed;
                    if (map[x, y] == 3) b = Brushes.SteelBlue;

                    Rectangle cell = new Rectangle
                    {
                        Width = CELL - 2,
                        Height = CELL - 2,
                        Fill = b
                    };

                    Canvas.SetLeft(cell, x * CELL);
                    Canvas.SetTop(cell, y * CELL);
                    MapCanvas.Children.Add(cell);
                }
            }

            // 🚗 Car
            Rectangle car = new Rectangle
            {
                Width = CELL - 8,
                Height = CELL - 12,
                RadiusX = 8,
                RadiusY = 8,
                Fill = Brushes.Gold
            };

            Canvas.SetLeft(car, evacuee.x * CELL + 4);
            Canvas.SetTop(car, evacuee.y * CELL + 6);
            MapCanvas.Children.Add(car);
        }

        // ================= FIRE =================
        void SpreadFire()
        {
            for (int i = 0; i < 2; i++)
            {
                int x = rand.Next(GRID);
                int y = rand.Next(GRID);

                if (map[x, y] == 0 && (x, y) != shelter)
                    map[x, y] = 2;
            }
        }

        // ================= MOVE =================
        void MoveEvacuee()
        {
            Stopwatch sw = Stopwatch.StartNew();
            long memBefore = GC.GetTotalMemory(true);

            var path = AStar(evacuee, shelter);

            sw.Stop();
            long memAfter = GC.GetTotalMemory(false);

            runtimeHistory.Add((int)sw.ElapsedMilliseconds);

            RuntimeText.Text = $"Runtime: {sw.ElapsedMilliseconds} ms";
            MemoryText.Text = $"Memory: {(memAfter - memBefore) / 1024} KB";
            PathText.Text = $"Path Length: {path.Count}";

            DrawGraph();

            if (path.Count > 1)
                evacuee = path[1];

            if (evacuee == shelter)
            {
                timer.Stop();
                MessageBox.Show("✅ Evacuated Successfully!");
            }

            if (map[evacuee.x, evacuee.y] == 2)
            {
                timer.Stop();
                MessageBox.Show("💀 Evacuee caught by fire!");
            }
        }

        // ================= A* + FUZZY =================
        List<(int, int)> AStar((int x, int y) start, (int x, int y) goal)
        {
            var open = new PriorityQueue<(int, int), double>();
            var cameFrom = new Dictionary<(int, int), (int, int)>();
            var gScore = new Dictionary<(int, int), double>();

            open.Enqueue(start, 0);
            gScore[start] = 0;
            cameFrom[start] = start;

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            while (open.Count > 0)
            {
                var current = open.Dequeue();
                if (current == goal) break;

                for (int i = 0; i < 4; i++)
                {
                    int nx = current.Item1 + dx[i];
                    int ny = current.Item2 + dy[i];
                    var n = (nx, ny);

                    if (nx < 0 || ny < 0 || nx >= GRID || ny >= GRID)
                        continue;
                    if (map[nx, ny] == 1)
                        continue;

                    double firePenalty = map[nx, ny] == 2 ? 5 : NearbyFirePenalty(n);
                    double tentative = gScore[current] + 1 + firePenalty;

                    if (!gScore.ContainsKey(n) || tentative < gScore[n])
                    {
                        gScore[n] = tentative;
                        open.Enqueue(n, tentative + Heuristic(n, goal));
                        cameFrom[n] = current;
                    }
                }
            }

            var path = new List<(int, int)>();
            if (!cameFrom.ContainsKey(goal)) return path;

            var cur = goal;
            while (cur != start)
            {
                path.Add(cur);
                cur = cameFrom[cur];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }

        double Heuristic((int x, int y) a, (int x, int y) b)
            => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);

        double NearbyFirePenalty((int x, int y) p)
        {
            double penalty = 0;
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = p.x + dx;
                    int ny = p.y + dy;
                    if (nx >= 0 && ny >= 0 && nx < GRID && ny < GRID)
                        if (map[nx, ny] == 2)
                            penalty += 2;
                }
            return penalty;
        }

        // ================= GRAPH =================
        void DrawGraph()
        {
            GraphCanvas.Children.Clear();
            if (runtimeHistory.Count < 2) return;

            double w = GraphCanvas.ActualWidth;
            double h = GraphCanvas.ActualHeight;

            for (int i = 1; i < runtimeHistory.Count; i++)
            {
                Line line = new Line
                {
                    X1 = (i - 1) * (w / runtimeHistory.Count),
                    Y1 = h - runtimeHistory[i - 1],
                    X2 = i * (w / runtimeHistory.Count),
                    Y2 = h - runtimeHistory[i],
                    Stroke = Brushes.Lime,
                    StrokeThickness = 2
                };
                GraphCanvas.Children.Add(line);
            }
        }

        // ================= MOUSE =================
        private void MapCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(MapCanvas);
            int x = (int)(pos.X / CELL);
            int y = (int)(pos.Y / CELL);

            if (x >= 0 && y >= 0 && x < GRID && y < GRID)
            {
                if ((x, y) != evacuee && (x, y) != shelter)
                {
                    map[x, y] = map[x, y] == 1 ? 0 : 1;
                    Draw();
                }
            }
        }
      /*  List<(int, int)> CallPythonAStar()
{
    var payload = new
    {
        grid = map,
        fire = fireCells.Select(f => new[] { f.x, f.y }).ToList(),
        congestion = new List<int[]>(),
        start = new[] { evacuee.x, evacuee.y },
        goal = new[] { shelter.x, shelter.y },
        distance = 40,
        risk = 30,
        capacity = 80
    };

    string json = JsonSerializer.Serialize(payload);

    var psi = new ProcessStartInfo
    {
        FileName = "python",
       // Arguments = $"astar_fuzzy_runner.py \"{json}\"",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    var p = Process.Start(psi);
    string output = p.StandardOutput.ReadToEnd();
    p.WaitForExit();

    var result = JsonSerializer.Deserialize<PythonResult>(output);

    return result.path.Select(p => (p[0], p[1])).ToList();
}*/



class PythonResult
{
    public List<List<int>> path { get; set; }
    public double fuzzy { get; set; }
}

    }

    
}
