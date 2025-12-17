
WPF FRONTEND (EvacuationWPF)

1. Create WPF App (.NET 6 or later)
2. Replace MainWindow.xaml with this file
3. Add NuGet:
   - LiveCharts.Wpf
   - Newtonsoft.Json
4. Use HttpClient to call Python API at http://localhost:5000/astar
5. Animate path on Canvas using DispatcherTimer
