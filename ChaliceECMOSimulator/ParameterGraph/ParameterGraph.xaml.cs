using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SkiaSharp;
using SkiaSharp.Views.UWP;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ChaliceECMOSimulator.ParameterGraph
{
    public sealed partial class ParameterGraph : UserControl, INotifyPropertyChanged
    {
        string _title = "graph";
        public string Title { get {return _title; } set { _title = value; OnPropertyChanged(); } }
        
        
            public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        float _graphXOffset = 50;
        public float GraphXOffset { get { return _graphXOffset; } set { _graphXOffset = value; OnPropertyChanged(); } }

        float _graphYOffset = 50;
        public float GraphYOffset { get { return _graphYOffset; } set { _graphYOffset = value; OnPropertyChanged(); } }

        float _graphMaxY = 100;
        public float GraphMaxY { get { return _graphMaxY; } set { _graphMaxY = value; OnPropertyChanged(); } }

        float _graphMinY = 0;
        public float GraphMinY { get { return _graphMinY; } set { _graphMinY = value; OnPropertyChanged(); } }

        float _graphMaxX = 100;
        public float GraphMaxX { get { return _graphMaxX; } set { _graphMaxX = value; OnPropertyChanged(); } }

        float _graphMinX = 0;
        public float GraphMinX { get { return _graphMinX; } set { _graphMinX = value; OnPropertyChanged(); } }

        public bool Graph1Enabled { get; set; } = true;
        SKPoint point1;
        Queue<SKPoint> queue1;
        SKPoint[] displayArray1;
        public SKPointMode PointMode1 { get; set; } = SKPointMode.Polygon;
        public SKPaint GraphPaint1 { get; set; }

        public bool Graph2Enabled { get; set; } = false;
        SKPoint point2;
        Queue<SKPoint> queue2;
        SKPoint[] displayArray2;
        public SKPointMode PointMode2 { get; set; } = SKPointMode.Polygon;
        public SKPaint GraphPaint2 { get; set; }

        public bool Graph3Enabled { get; set; } = false;
        SKPoint point3;
        Queue<SKPoint> queue3;
        SKPoint[] displayArray3;
        public SKPointMode PointMode3 { get; set; } = SKPointMode.Polygon;
        public SKPaint GraphPaint3 { get; set; }

        public bool Graph4Enabled { get; set; } = false;
        SKPoint point4;
        Queue<SKPoint> queue4;
        SKPoint[] displayArray4;
        public SKPointMode PointMode4 { get; set; } = SKPointMode.Polygon;
        public SKPaint GraphPaint4 { get; set; }

        public bool Graph5Enabled { get; set; } = false;
        SKPoint point5;
        Queue<SKPoint> queue5;
        SKPoint[] displayArray5;
        public SKPointMode PointMode5 { get; set; } = SKPointMode.Polygon;
        public SKPaint GraphPaint5 { get; set; }

        public SKColor BackgroundColor { get; set; } = SKColors.Black;
        public SKPaint GridLineAxisPaint;
        public SKPaint GridLinePaint;
        public SKPaint GridAxisLabelsPaint;
        public SKPaint GridTitlePaint;
        public SKPaint LegendTextPaint;

        public float GridYAxisStep { get; set; } = 10;
        public float GridXAxisStep { get; set; } = 10;

        public bool HideXAxisLabels { get; set; } = false;
        public bool HideYAxisLabels { get; set; } = false;

        public string XAxisTitle { get; set; } = "x - axis";
        public string YAxisTitle { get; set; } = "y - axis";

        public string Legend1 { get; set; } = "Test";
        public string Legend2 { get; set; } = "White";
        public string Legend3 { get; set; } = "";
        public string Legend4 { get; set; } = "";
        public string Legend5 { get; set; } = "";

        public int PixelPerDataPoint { get; set; } = 2;                  // number of pixels between per datapoints
        public bool IsSideScrolling { get; set; } = true;       
        public int ScrollDirection { get; set; } = 1;

        public int DataRefreshRate { get; set; } = 15;          // in ms so every 15 ms a datapoint is added to the arrays
        public int GraphicsUpdateRate { get; set; } = 15;       // in ms so every 15 ms the graph is data-arrays are drawn
        public int GraphicsClearanceRate { get; set; } = 0;     // in ms so every 0 seconds the complete canvas is rebuild, set 0 for side scrolling graphs
        public float GraphicsFrameDuration { get; set; } = 0;     // in ms so the duration of one complete graphics frame in ms.
        int refreshCounter = 0;

        private readonly DispatcherTimer updateTimer = new Windows.UI.Xaml.DispatcherTimer();

        float h_data = 0;
        float w_data = 0;

        float h_grid = 0;
        float w_grid = 0;

        public ParameterGraph()
        {
            this.InitializeComponent();

            queue1 = new Queue<SKPoint>();
            point1 = new SKPoint();
            GraphPaint1 = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = 2,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true
            };

            queue2 = new Queue<SKPoint>();
            point2 = new SKPoint();
            GraphPaint2 = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.White,
                StrokeWidth = 2,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true
            };

            queue3 = new Queue<SKPoint>();
            point3 = new SKPoint();
            GraphPaint3 = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.LimeGreen,
                StrokeWidth = 2,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true
            };

            queue4 = new Queue<SKPoint>();
            point4 = new SKPoint();
            GraphPaint4 = new SKPaint

            {

                Style = SKPaintStyle.Stroke,

                Color = SKColors.Orange,

                StrokeWidth = 2,

                StrokeCap = SKStrokeCap.Round,

                IsAntialias = true

            };

            queue5 = new Queue<SKPoint>();
            point5 = new SKPoint();
            GraphPaint5 = new SKPaint

            {

                Style = SKPaintStyle.Stroke,

                Color = SKColors.Blue,

                StrokeWidth = 2,

                StrokeCap = SKStrokeCap.Round,

                IsAntialias = true

            };

            GridLineAxisPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.White,
                StrokeWidth = 1,
                IsAntialias = true,
            };

            float[] dashed = { 2, 2 };
            GridLinePaint = new SKPaint

            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.White,
                StrokeWidth = 1,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(dashed, 20)
            };
            GridAxisLabelsPaint = new SKPaint
            {
                Typeface = SKTypeface.FromFamilyName("Arial Bold"),
                FakeBoldText = false,
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                TextAlign = SKTextAlign.Right,
                Color = SKColors.White,
                IsStroke = false,
                TextSize = 16f

            };
            LegendTextPaint = new SKPaint
            {
                Typeface = SKTypeface.FromFamilyName("Arial Bold"),
                FakeBoldText = false,
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                TextAlign = SKTextAlign.Left,
                Color = SKColors.White,
                IsStroke = false,
                TextSize = 16f
            };

            // start the graph timer
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, GraphicsUpdateRate);
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

            // find the size
            h_data = (float)myGraphCanvas.CanvasSize.Height;
            w_data = (float)myGraphCanvas.CanvasSize.Width;

            // draw the grid
            myGraphCanvasGrid.Invalidate();

        }

        private void UpdateTimer_Tick(object sender, object e)
        {
            myGraphCanvas.Invalidate();
        }

        public void UpdateGraphData(double _x1, double _y1, double _x2 = 0, double _y2 = 0, double _x3 = 0, double _y3 = 0, double _x4 = 0, double _y4 = 0, double _x5 = 0, double _y5 = 0)
        {
            // calculate the number of needed datapoints for the graph as determined by the x - axis and the pixels between datapoints
            int noDataPoints = (int)((w_data - 2 * GraphXOffset) / PixelPerDataPoint);

            // calculate the vertical scaling
            float yScaling = (h_data - 2 * GraphYOffset) / (GraphMaxY - GraphMinY);

            // calculate the horizontal scaling
            float xScaling = (w_data - 2 * GraphXOffset) / (GraphMaxX - GraphMinX);

            // calculate the coordinates
            point1.Y = (float)(h_data - ((_y1 - GraphMinY) * yScaling) - GraphYOffset);
            point1.X = (float)(GraphXOffset + ((_x1 - GraphMinX) * xScaling));
            if (point1.X < GraphXOffset) point1.X = GraphXOffset;
            if (point1.X > w_data - GraphXOffset) point1.X = w_data - GraphXOffset;

            point2.Y = (float)(h_data - ((_y2 - GraphMinY) * yScaling) - GraphYOffset);
            point2.X = (float)(GraphXOffset + ((_x2 - GraphMinX) * xScaling));
            if (point2.X < GraphXOffset) point2.X = GraphXOffset;
            if (point2.X > w_data - GraphXOffset) point2.X = w_data - GraphXOffset;

            point3.Y = (float)(h_data - ((_y3 - GraphMinY) * yScaling) - GraphYOffset);
            point3.X = (float)(GraphXOffset + ((_x3 - GraphMinX) * xScaling));
            if (point3.X < GraphXOffset) point3.X = GraphXOffset;
            if (point3.X > w_data - GraphXOffset) point3.X = w_data - GraphXOffset;

            point4.Y = (float)(h_data - ((_y4 - GraphMinY) * yScaling) - GraphYOffset);
            point4.X = (float)(GraphXOffset + ((_x4 - GraphMinX) * xScaling));
            if (point4.X < GraphXOffset) point4.X = GraphXOffset;
            if (point4.X > w_data - GraphXOffset) point4.X = w_data - GraphXOffset;

            point5.Y = (float)(h_data - ((_y5 - GraphMinY) * yScaling) - GraphYOffset);
            point5.X = (float)(GraphXOffset + ((_x5 - GraphMinX) * xScaling));
            if (point5.X < GraphXOffset) point5.X = GraphXOffset;
            if (point5.X > w_data - GraphXOffset) point5.X = w_data - GraphXOffset;

            lock (queue1)
            {
                // put the new point on the queue
                queue1.Enqueue(point1);

                // if the queue count exceeds the number of datapoint than remove the first point from the queue
                if (queue1.Count >= noDataPoints) queue1.Dequeue();

                // copy the current queue to the display array for display purposes
                if (queue1.Count > 0) displayArray1 = queue1.ToArray();
            }

            lock (queue2)
            {
                // put the new point on the queue
                queue2.Enqueue(point2);

                // if the queue count exceeds the number of datapoint than remove the first point from the queue
                if (queue2.Count >= noDataPoints) queue2.Dequeue();

                // copy the current queue to the display array for display purposes
                if (queue2.Count > 0) displayArray2 = queue2.ToArray();
            }

            lock (queue3)
            {
                // put the new point on the queue
                queue3.Enqueue(point3);

                // if the queue count exceeds the number of datapoint than remove the first point from the queue
                if (queue3.Count >= noDataPoints) queue3.Dequeue();

                // copy the current queue to the display array for display purposes
                if (queue3.Count > 0) displayArray3 = queue3.ToArray();
            }

            lock (queue4)
            {
                // put the new point on the queue
                queue4.Enqueue(point4);

                // if the queue count exceeds the number of datapoint than remove the first point from the queue
                if (queue4.Count >= noDataPoints) queue4.Dequeue();

                // copy the current queue to the display array for display purposes
                if (queue4.Count > 0) displayArray4 = queue4.ToArray();
            }

            lock (queue5)
            {
                // put the new point on the queue
                queue5.Enqueue(point5);

                // if the queue count exceeds the number of datapoint than remove the first point from the queue
                if (queue5.Count >= noDataPoints) queue5.Dequeue();

                // copy the current queue to the display array for display purposes
                if (queue5.Count > 0) displayArray5 = queue5.ToArray();
            }

            // if the graph is a scrolling graph we have to determine the x value
            if (IsSideScrolling)
            {
                float x = GraphXOffset;
                if (ScrollDirection == -1)
                {
                    x = w_data - GraphXOffset;
                }

                if (displayArray5 != null)
                {
                    for (int pos = 0; pos < displayArray5.Count(); pos++)
                    {
                        displayArray1[pos].X = x;
                        displayArray2[pos].X = x;
                        displayArray3[pos].X = x;
                        displayArray4[pos].X = x;
                        displayArray5[pos].X = x;
                        x += PixelPerDataPoint * ScrollDirection;
                    }
                }
            }
        }

        private void MyGraphCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            h_data = (float) myGraphCanvas.CanvasSize.Height;
            w_data = (float)myGraphCanvas.CanvasSize.Width;
            // graphics frame duration = Every 15 ms the data is updated and this data point takes 4 pixels.  So width / 4 = number of datapoints * 15 ms = duration of 1 frame
            GraphicsFrameDuration = ((w_data - 2 * GraphXOffset) / PixelPerDataPoint) * DataRefreshRate;
            DrawGraph(canvas);
        }

        private void MyGraphCanvasGrid_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            h_grid = (float)myGraphCanvasGrid.CanvasSize.Height;
            w_grid = (float)myGraphCanvasGrid.CanvasSize.Width;
            DrawGrid(canvas);
        }

        public void ClearQueues()

        {
            queue1.Clear();
            queue2.Clear();
            queue3.Clear();
            queue4.Clear();
            queue5.Clear();
        }

        public void DrawGraph(SKCanvas _canvasGraph)
        {
            if (refreshCounter >= GraphicsClearanceRate)
            {
                _canvasGraph.Clear(SKColors.Transparent);
                refreshCounter = 0;
            }
            refreshCounter += GraphicsUpdateRate;

            if (Graph1Enabled && displayArray1 != null) _canvasGraph.DrawPoints(PointMode1, displayArray1, GraphPaint1);
            if (Graph2Enabled && displayArray2 != null) _canvasGraph.DrawPoints(PointMode2, displayArray2, GraphPaint2);
            if (Graph3Enabled && displayArray3 != null) _canvasGraph.DrawPoints(PointMode3, displayArray3, GraphPaint3);
            if (Graph4Enabled && displayArray4 != null) _canvasGraph.DrawPoints(PointMode4, displayArray4, GraphPaint4);
            if (Graph5Enabled && displayArray5 != null) _canvasGraph.DrawPoints(PointMode5, displayArray5, GraphPaint5);
        }

        public void RedrawGrid()
        {
            // draw the grid
            myGraphCanvasGrid.Invalidate();
        }
        public void DrawGrid(SKCanvas _canvasGrid)
        {
            // clear the grid graph
            _canvasGrid.Clear(BackgroundColor);

            // draw the x - axis
            _canvasGrid.DrawLine(GraphXOffset, h_grid - GraphYOffset, w_grid - GraphXOffset, h_grid - GraphYOffset, GridLineAxisPaint);
            _canvasGrid.DrawLine(GraphXOffset, 0 + GraphYOffset, w_grid - GraphXOffset, 0 + GraphYOffset, GridLineAxisPaint);

            // draw the y - axis
            _canvasGrid.DrawLine(GraphXOffset, h_grid - GraphYOffset, GraphXOffset, 0 + GraphYOffset, GridLineAxisPaint);
            _canvasGrid.DrawLine(w_grid - GraphXOffset, h_grid - GraphYOffset, w_grid - GraphXOffset, 0 + GraphYOffset, GridLineAxisPaint);

            // draw the legend
            float pos = w_grid / 6;
            if (Graph1Enabled)
            {
                _canvasGrid.DrawLine(pos, 15, pos + 15f, 15, GraphPaint1);
                _canvasGrid.DrawText(Legend1, pos + 20f, 18, LegendTextPaint);
            }
            if (Graph2Enabled)
            {
                _canvasGrid.DrawLine(pos * 2f, 15, pos * 2f + 15f, 15, GraphPaint2);
                _canvasGrid.DrawText(Legend2, pos * 2f + 20f, 18, LegendTextPaint);
            }
            if (Graph3Enabled)
            {
                _canvasGrid.DrawLine(pos * 3f, 15, pos * 3f + 15f, 15, GraphPaint3);
                _canvasGrid.DrawText(Legend3, pos * 3f + 20f, 18, LegendTextPaint);
            }
            if (Graph4Enabled)
            {
                _canvasGrid.DrawLine(pos * 4f, 15, pos * 4f + 15f, 15, GraphPaint4);
                _canvasGrid.DrawText(Legend4, pos * 4f + 20f, 18, LegendTextPaint);
            }
            if (Graph5Enabled)
            {
                _canvasGrid.DrawLine(pos * 5f, 15, pos * 5f + 15f, 15, GraphPaint5);
                _canvasGrid.DrawText(Legend5, pos * 5f + 20f, 18, LegendTextPaint);
            }

            // draw the horizontal grid lines
            float yStepSize = ((h_grid - 2 * GraphYOffset) / (GraphMaxY - GraphMinY)) * GridYAxisStep;
            float yLabel = GraphMinY;

            for (float yLine = GraphYOffset; yLine <= h_grid - GraphYOffset + 1; yLine += yStepSize)
            {
                _canvasGrid.DrawLine(GraphXOffset, h_grid - yLine, w_grid - GraphXOffset, h_grid - yLine, GridLinePaint);
                _canvasGrid.DrawText(yLabel.ToString(), GraphXOffset - 8, h_grid - yLine + 5, GridAxisLabelsPaint);
                yLabel += GridYAxisStep;
            }

            // draw the vertical grid lines
            if (IsSideScrolling)
            {
                // as the graph is side scrolling the vertical grid lines are timed datapoints and stored in an array where every point is dependent on the datarefresh rate
                // so a datarefresh rate of every 15 ms creates a datapoint every 15 ms.
                // so the labels should be if a screen width of 1000 takes a dataframe of 1000 points which should by 1000 * 15 ms = 15000 ms = 15 seconds
                // if i want a scroll line for every second than the stepsize would be 15 sec = 1000 points -> 1 sec = 1000/15 = 66,67 points and thats the stepsize
                // so the width * datarefreshrate / 1000 = number of seconds in this frame, then 

                float xStepSize = ((w_data - 2 * GraphXOffset) / (GraphicsFrameDuration / 1000));      // pixels / s
                float xLabel = GraphMinX;
                string xLabelText = "";
                for (float xLine = GraphXOffset; xLine <= w_grid - GraphXOffset; xLine += xStepSize)
                {
                    xLabelText = xLabel.ToString();
                    if (true)
                    {
                        xLabelText = Math.Round(xLabel, 0).ToString();
                    }
                    _canvasGrid.DrawLine(xLine, h_grid - GraphYOffset, xLine, 0 + GraphYOffset, GridLinePaint);
                    if (HideXAxisLabels == false)
                    {
                        _canvasGrid.DrawText(xLabelText.ToString(), xLine + 8, h_grid - GraphYOffset + 18, GridAxisLabelsPaint);
                    }
                    else
                    {
                        _canvasGrid.DrawText(XAxisTitle, w_grid / 2f, h_grid - GraphYOffset + 18, GridAxisLabelsPaint);
                    }
                    xLabel += GridXAxisStep;
                }
            } else
            {
                // draw the vertical grid lines
                float xStepSize = ((w_grid - 2 * GraphXOffset) / (GraphMaxX - GraphMinX)) * GridXAxisStep;
                float xLabel = GraphMinX;
                for (float xLine = GraphXOffset; xLine <= w_grid - GraphXOffset + 1; xLine += xStepSize)
                {
                    _canvasGrid.DrawLine(xLine, h_grid - GraphYOffset, xLine, 0 + GraphYOffset, GridLinePaint);
                    _canvasGrid.DrawText(xLabel.ToString(), xLine + 8, h_grid - GraphYOffset + 18, GridAxisLabelsPaint);
                    xLabel += GridXAxisStep;
                }
            }
            ClearQueues();
        }

      
    }
}

