using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CorporateNetworks.Common.Algorithms;
using CorporateNetworks.Common.Drawing;
using CorporateNetworks.Common.Extensions;
using CorporateNetworks.Common.Generation;
using CorporateNetworks.Common.Models;
using CorporateNetworks.Common.Serialization;
using Microsoft.Win32;

namespace CorporateNetwork.PairTransitions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Graph graphDrawing;
        private readonly List<Edge> edges = new List<Edge>();
        private double[][] adjacencyMatrix;

        public MainWindow()
        {
            this.InitializeComponent();
            this.graphDrawing = new Graph(this.GraphCanvas);
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            this.Draw(EdgeGeneration.GenerateWeightedEdges(Convert.ToInt32(this.NodesNumber.Value), true));
        }

        private void LoadEdges_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*",
                InitialDirectory = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName
            };

            if (openFileDialog.ShowDialog() == true)
            {
                this.Draw(XmlSerialization.ReadFromXmlFile<List<Edge>>(openFileDialog.FileName));
            }
        }

        private void SaveEdges_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Xml files (*.xml)|*.xml",
                InitialDirectory = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                XmlSerialization.WriteToXmlFile(saveFileDialog.FileName, this.edges);
            }
        }

        private void Draw(List<Edge> edgesToWrite)
        {
            this.edges.Clear();
            this.edges.AddRange(edgesToWrite);
            this.graphDrawing.Draw(this.edges);
            this.adjacencyMatrix = edgesToWrite.ToAdjacencyMatrix();
            this.NodeToStart.Minimum = 0;
            this.NodeToStart.Maximum = this.adjacencyMatrix.Length - 1;
            this.CalculationGroupBox.IsEnabled = true;
            this.EdgesDataGrid.ItemsSource = this.edges;
            this.EdgesDataGrid.Items.Refresh();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            var nodeToStart = Convert.ToInt32(this.NodeToStart.Value);

            var timer = Stopwatch.StartNew();

            var result = PairTransitionsAlgorithm.Run(this.adjacencyMatrix, nodeToStart).ToList();

            timer.Stop();

            this.ResultGroupBox.IsEnabled = true;
            this.ResultDataGrid.ItemsSource = result;
            this.ResultDataGrid.Items.Refresh();
        }

        private void ChangeWeight_Click(object sender, RoutedEventArgs e)
        {
            var a = this.EdgesDataGrid.SelectedCells;
            Edge edge = (a.Count > 0 ? a[0].Item : null) as Edge;
            if (edge != null)
            {
                var edgeToChange = this.edges.First(ed => ed.Parent == edge.Parent && ed.Child == edge.Child);
                edgeToChange.Weight = Convert.ToDouble(this.NewWeight.Text);
                this.graphDrawing.Draw(this.edges);
                this.adjacencyMatrix = this.edges.ToAdjacencyMatrix();
                this.EdgesDataGrid.ItemsSource = this.edges;
                this.EdgesDataGrid.Items.Refresh();
            }
        }
    }
}
