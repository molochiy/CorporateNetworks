using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using CorporateNetworks.Common.Algorithms;
using CorporateNetworks.Common.Drawing;
using CorporateNetworks.Common.Extensions;
using CorporateNetworks.Common.Generation;
using CorporateNetworks.Common.Models;
using CorporateNetworks.Common.Serialization;
using Microsoft.Win32;

namespace CorporateNetworks.Dijkstra
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Graph graphDrawing;
        private readonly List<WeightedEdge> edges = new List<WeightedEdge>();
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
                this.Draw(XmlSerialization.ReadFromXmlFile<List<WeightedEdge>>(openFileDialog.FileName));
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

        private void Draw(List<WeightedEdge> edgesToWrite)
        {
            this.edges.Clear();
            this.edges.AddRange(edgesToWrite);
            this.graphDrawing.Draw(this.edges.ToList());
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

            var result = DijkstraAlgorithm.Run(this.adjacencyMatrix, nodeToStart).ToList();

            timer.Stop();

            this.ResultGroupBox.IsEnabled = true;
            this.ResultDataGrid.ItemsSource = result;
            this.ResultDataGrid.Items.Refresh();
            this.ElapsedTimeText.Text = $"{timer.ElapsedMilliseconds} ms";
        }
    }
}
