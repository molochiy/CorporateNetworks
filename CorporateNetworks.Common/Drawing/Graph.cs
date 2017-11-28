using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CorporateNetworks.Common.Models;

namespace CorporateNetworks.Common.Drawing
{
    public class Graph
    {
        const double NodeRadius = 7;

        const int NodeZIndex = 2;

        const int EdgeZIndex = 1;

        private readonly Canvas graphCanvas;

        private readonly List<Node> nodes = new List<Node>();

        public Graph(Canvas canvas)
        {
            this.graphCanvas = canvas;
        }

        public void Draw(List<Edge> edges)
        {
            this.CleanUp();
            var canvasVerticleMiddle = this.graphCanvas.ActualHeight / 2;
            var canvasHorizontalMiddle = this.graphCanvas.ActualWidth / 2;
            var circleRadius = Math.Min(this.graphCanvas.ActualWidth, this.graphCanvas.ActualHeight) / 2 - 30;

            var nodeIds = edges.Select(e => e.Parent).Union(edges.Select(e => e.Child)).ToList();
            nodeIds.Sort((e1, e2) => e1 - e2);

            var angle = 2 * Math.PI / nodeIds.Count;

            for (var i = 0; i < nodeIds.Count; i++)
            {
                var nodePos = new Node
                {
                    Id = nodeIds[i],
                    Position = new Position
                    {
                        Top = canvasVerticleMiddle - circleRadius * Math.Cos(angle * i),
                        Left = canvasHorizontalMiddle + circleRadius * Math.Sin(angle * i)
                    }
                };

                this.nodes.Add(nodePos);

                this.DrawNode(nodePos);

                this.DrawNodeText(nodePos, angle * i);
            }

            this.DrawEdges(edges);
        }

        private void DrawEdge(Edge edge, Color color, bool isTransposeEdge = false)
        {
            var parentNodePosition = this.nodes.Find(ep => ep.Id == edge.Parent).Position;
            var childNodePosition = this.nodes.Find(ep => ep.Id == edge.Child).Position;

            if (!parentNodePosition.Equals(childNodePosition))
            {
                var x1 = parentNodePosition.Left + NodeRadius / 2;
                var y1 = parentNodePosition.Top + NodeRadius / 2;
                var x2 = childNodePosition.Left + NodeRadius / 2;
                var y2 = childNodePosition.Top + NodeRadius / 2;

                if (isTransposeEdge)
                {
                    x2 = (x1 + x2) / 2;
                    y2 = (y1 + y2) / 2;
                }

                var edgeLine = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(color),
                    Fill = new SolidColorBrush(color)
                };

                Panel.SetZIndex(edgeLine, EdgeZIndex);

                this.graphCanvas.Children.Add(edgeLine);
            }
        }

        private void DrawNode(Node node)
        {
            Ellipse nodeEllipse = new Ellipse
            {
                Width = NodeRadius,
                Height = NodeRadius,
                Fill = new SolidColorBrush(Colors.Blue)
            };

            Canvas.SetTop(nodeEllipse, node.Position.Top);
            Canvas.SetLeft(nodeEllipse, node.Position.Left);
            Panel.SetZIndex(nodeEllipse, NodeZIndex);

            this.graphCanvas.Children.Add(nodeEllipse);
        }

        private void DrawNodeText(Node node, double angle)
        {
            TextBlock nodeText = new TextBlock
            {
                Text = node.Id.ToString()
            };

            var textTopPos = node.Position.Top - nodeText.FontSize * 2 * Math.Cos(angle);
            var textLeftPos = node.Position.Left + nodeText.FontSize * nodeText.Text.Length * Math.Sin(angle);

            Canvas.SetTop(nodeText, textTopPos);
            Canvas.SetLeft(nodeText, textLeftPos);

            this.graphCanvas.Children.Add(nodeText);
        }

        private void DrawEdges(List<Edge> edges)
        {
            var drawnEdges = new List<Edge>();
            edges.Sort((e1, e2) => e1.Parent != e2.Parent ? e1.Parent - e2.Parent : e1.Child - e2.Child);
            foreach (var edge in edges)
            {
                var transposeEdge = edges.FirstOrDefault(de => de.Child == edge.Parent && de.Parent == edge.Child);
                if (transposeEdge != null)
                {
                    var drawnEdge = drawnEdges.FirstOrDefault(de => de.Child == edge.Parent && de.Parent == edge.Child);
                    this.DrawEdge(edge, drawnEdge == null ? Colors.Red : Colors.Green, true);
                }
                else
                {
                    this.DrawEdge(edge, edge.Parent < edge.Child ? Colors.Red : Colors.Green);
                }
                
                drawnEdges.Add(edge);
            }
        }

        private void CleanUp()
        {
            this.graphCanvas.Children.RemoveRange(0, this.graphCanvas.Children.Count);
            this.nodes.Clear();
        }
    }
}
