//
// PlottingHelpers.cs
//
// Author:
//       Tom Diethe <tom.diethe@bristol.ac.uk>
//
// Copyright (c) 2016 University of Bristol
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace BayesianDictionaryLearning
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using MicrosoftResearch.Infer.Distributions;
    using PythonPlotter;
    using InferHelpers;

    using Vector = MathNet.Numerics.LinearAlgebra.Vector<double>;
    using Matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>;


    public static class PlottingHelper
    {
        public static string PythonPath { get; set; }
        public static string ScriptPath { get; set; }
        public static string FigurePath { get; set; }

        /// <summary>
        /// Plots the results.
        /// </summary>
        /// <param name="x">The x values.</param>
        /// <param name="y">The y values.</param>
        /// <param name="title">The plot title.</param>
        /// <param name="subTitle">Sub title.</param>
        /// <param name="xlabel">x-axis label.</param>
        /// <param name="ylabel">y-axis label.</param>
        /// <param name="show">Whether to show the plot.</param>
        public static void Plot(IEnumerable<double> x, IEnumerable<double> y, string title, string subTitle,
            string xlabel, string ylabel, bool show = false)
        {
            var series = (ISeries) (new LineSeries { X = x, Y = y });

            var plotter = new Plotter
            {
                Title = title + (string.IsNullOrEmpty(subTitle) ? string.Empty : " " + subTitle),
                XLabel = xlabel,
                YLabel = ylabel,
                Series = new[] { series },
                ScriptName = Path.Combine(ScriptPath, title.Replace(" ", "_") + ".py"),
                FigureName = Path.Combine(FigurePath, title.Replace(" ", "_") + ".pdf"),
                Python = PythonPath,
                Show = show,
                Tight = true
            };

            plotter.Plot();
        }

        public static void TwinTwinPlot(
            Dictionary<string, IEnumerable<double>> y1,
            Dictionary<string, IEnumerable<double>> y2,
            string title,
            string xlabel,
            string y1Label,
            string y2Label,
            bool show = false)
        {
		    var series1 = y1.Select(ia => (ISeries) (new LineSeries {Label = ia.Key, X = ia.Value})).ToArray();
            var series2 = y2.Select(ia => (ISeries) (new LineSeries {Label = ia.Key, X = ia.Value})).ToArray();

            // Turn on color cycling for both series
			series1[0].Color = "next(palette)";
			series2[0].Color = "next(palette)";

			// Here we build the plotting script for the second plot (without the pre/postamble),
			// so we can append it to the script for the first plot
            var plotter2 = new Plotter { XLabel = xlabel, YLabel = y2Label, Series = series2, TwinX = true };
            plotter2.BuildScript();

            // TODO: http://matplotlib.org/examples/api/two_scales.html

            var plotter1 = new Plotter
            {
                Title = title,
                XLabel = xlabel,
                YLabel = y1Label,
                Series = series1,
                Python = PythonPath,
                Show = show,
                Tight = true
            };
            plotter1.Plot(plotter2.Script);
        }

        /// <summary>
        /// Plots the results.
        /// </summary>
        /// <param name="y">The values.</param>
        /// <param name="filename">The file name.</param>
        /// <param name="xlabel">x-axis label.</param>
        /// <param name="ylabel">y-axis label.</param>
        /// <param name="show">Whether to show the plot.</param>
        public static void Plot(Dictionary<string, IEnumerable<double>> y, string filename, string xlabel,
            string ylabel, bool show = false)
        {
            var series = y.Select(ia => (ISeries) (new LineSeries {Label = ia.Key, X = ia.Value})).ToArray();

            var plotter = new Plotter
            {
                XLabel = xlabel,
                YLabel = ylabel,
                Series = series,
                ScriptName = Path.Combine(ScriptPath, filename.Replace(" ", "_") + ".py"),
                FigureName = Path.Combine(FigurePath, filename.Replace(" ", "_") + ".pdf"),
                Python = PythonPath,
                Show = show,
                Tight = true
            };

            plotter.Plot();
        }

        /// <summary>
        /// Plots the results.
        /// </summary>
        /// <param name="y">The values.</param>
        /// <param name="title">The plot title.</param>
        /// <param name="subTitle">Sub title.</param>
        /// <param name="xlabel">x-axis label.</param>
        /// <param name="ylabel">y-axis label.</param>
        /// <param name="show">Whether to show the plot.</param>
        public static void Plot(Dictionary<string, IEnumerable<double>> y, string title, string subTitle, string xlabel,
            string ylabel, bool show = false)
        {
            var series = y.Select(ia => (ISeries) (new LineSeries {Label = ia.Key, X = ia.Value})).ToArray();

            var plotter = new Plotter
            {
                Title = title + (string.IsNullOrEmpty(subTitle) ? string.Empty : " " + subTitle),
                XLabel = xlabel,
                YLabel = ylabel,
                Series = series,
                ScriptName = Path.Combine(ScriptPath, title.Replace(" ", "_") + ".py"),
                FigureName = Path.Combine(FigurePath, title.Replace(" ", "_") + ".pdf"),
                Python = PythonPath,
                Show = show,
                Tight = true
            };

            plotter.Plot();
        }

        public static void SparsityPlot(Gaussian[][] coefficients, string title, string filename, bool show = false)
        {
            var values = coefficients.GetMeans<Gaussian>(); // .To2D().Transpose().ToJagged();
	        var plotter = new Plotter
				{
					// Title = title,
					XLabel = "bases",
					YLabel = "signals",
                    ScriptName = Path.Combine(ScriptPath, $"{filename.Replace(" ", "_")}.py"),
                    FigureName = Path.Combine(FigurePath, $"{filename.Replace(" ", "_")}.pdf"),
                    Python = PythonPath,
                    Series = new ISeries[] { new HintonSeries { Values = values } },
					Grid = false,
				    Show = show,
				    Tight = true
				};
			plotter.Plot();
        }

        /// <summary>
        /// Plots the functions.
        /// </summary>
        /// <param name="functions">The functions to plot.</param>
        /// <param name="title">The plot title..</param>
        /// <param name="subTitle">Sub title.</param>
        /// <param name="options">The plotting options.</param>
        public static void PlotFunctions(Matrix functions, string title, string subTitle, PlotOptions options)
        {
            if (options.IsImage)
            {
                PlotImages(functions, title, options.Images);
                return;
            }

            var series = CreateSeries(functions.ToRowArrays(), null, options.Signals);
            var plotter = new Plotter
            { 
                Title = title + (string.IsNullOrEmpty(subTitle) ? string.Empty : " " + subTitle),
                XLabel = "x", YLabel = "y", 
                Series = series, Subplots = options.Signals.Subplots,
                ScriptName = Path.Combine(ScriptPath, title + ".py"),
                FigureName = Path.Combine(FigurePath, title + ".pdf"),
                Python = PythonPath
            };

            plotter.Plot();            
        }

        public static void ScatterPlot(Matrix data, int[] labels, string title, string subTitle)
        {
            var series = (ISeries) (new ScatterSeries
                {
                    X = data.Column(0).ToArray(),
                    Y = data.Column(1).ToArray(),
                    Color = "[" + string.Join(", ", labels.Select(ia => $"{ia}")) + "]"
                });

            var plotter = new Plotter
            {
                Title = title + (string.IsNullOrEmpty(subTitle) ? string.Empty : " " + subTitle),
                XLabel = "x",
                YLabel = "y",
                Series = new[] { series },
                ScriptName = Path.Combine(ScriptPath, title + ".py"),
                FigureName = Path.Combine(FigurePath, title + ".pdf"),
                Python = PythonPath
            };

            plotter.Plot();
        }

        /// <summary>
        /// Plots the results.
        /// </summary>
        /// <returns>The results.</returns>
        /// <param name="numBases">Number of bases.</param>
        /// <param name="signalWidth">The signal width.</param>
        /// <param name="dictionary">Dictionary.</param>
        /// <param name="coefficients">Coefficients.</param>
        /// <param name="st">Sub title.</param>
        /// <param name="plotOptions">The plotting options.</param>
        public static void PlotResults(
            int numBases, int signalWidth,
            Gaussian[][] dictionary, Gaussian[][] coefficients,
            string st, PlotOptions plotOptions)
        {
            var subplots = new Subplots
            {
                ShareX = true,
                ShareY = true,
                Rows = numBases < 16 ? numBases : 4,
                Columns = numBases < 16 ? 1 : 4
            };

            plotOptions.Dictionary.Subplots = subplots;
            plotOptions.Dictionary.NumToShow = plotOptions.IsImage ? 1 : Math.Min(numBases, 16);

            if (plotOptions.IsImage)
            {
                PlotImages(dictionary, signalWidth, "Dictionary", plotOptions.Dictionary);
            }
            else
            {
                PlotPosteriors(dictionary, "Dictionary", st, plotOptions.Dictionary);
            }

            if (coefficients == null)
                return;

            subplots.Rows = 3;
            subplots.Columns = 2;
            plotOptions.Coefficients.NumToShow = 6;
            plotOptions.Coefficients.Subplots = subplots;
            PlotPosteriors(coefficients, "Coefficients", st, plotOptions.Coefficients);
        }

        /// <summary>
        /// Plots the results.
        /// </summary>
        /// <returns>The results.</returns>
        /// <param name="numBases">Number of bases.</param>
        /// <param name="signalWidth">The signal width.</param>
        /// <param name="dictionary">Dictionary.</param>
        /// <param name="coefficients">Coefficients.</param>
        /// <param name="st">Sub title.</param>
        /// <param name="plotOptions">The plotting options.</param>
        /// <param name="labels">Class labels.</param>
        public static void PlotResults(
            int numBases, int signalWidth,
            Gaussian[][][] dictionary, Gaussian[][][] coefficients,
            string st, PlotOptions plotOptions, IList<string> labels)
        {
            var subplots = new Subplots
            {
                ShareX = true,
                ShareY = true,
                Rows = numBases < 16 ? numBases : 4,
                Columns = numBases < 16 ? 1 : 4,
            };

            plotOptions.Dictionary.Subplots = subplots;
            plotOptions.Dictionary.NumToShow = plotOptions.IsImage ? 1 : Math.Min(numBases, 16);

            for (var i = 0; i < dictionary.Length; i++)
            {
                string label = "Dictionary_class=" + (labels == null ? $"{i}" : labels[i]);
                if (plotOptions.IsImage)
                {
                    PlotImages(dictionary[i], signalWidth, label, plotOptions.Dictionary);
                }
                else
                {
                    PlotPosteriors(dictionary[i], label, st, plotOptions.Dictionary);
                }
            }

            if (coefficients == null)
                return;

            subplots.Rows = 3;
            subplots.Columns = 2;
            plotOptions.Coefficients.NumToShow = 6;
            plotOptions.Coefficients.Subplots = subplots;

            for (var i = 0; i < coefficients.Length; i++)
            {
                var coefficient = coefficients[i];
                string label = "Coefficients_class={label}" + (labels == null ? $"{i}" : labels[i]);
                PlotPosteriors(coefficient, label, st, plotOptions.Coefficients);
            }
        }

        /// <summary>
        /// Plots the results.
        /// </summary>
        /// <returns>The results.</returns>
        /// <param name="numBases">Number of bases.</param>
        /// <param name="signalWidth">The signal width.</param>
        /// <param name="dictionary">Dictionary.</param>
        /// <param name="coefficients">Coefficients.</param>
        /// <param name="st">Sub title.</param>
        /// <param name="plotOptions">Plotting options.</param>
        public static void PlotResults(
            int numBases, int signalWidth,
            VectorGaussian[] dictionary, VectorGaussian[] coefficients,
            string st, PlotOptions plotOptions)
        {
            // Note the dictionary is transposed in this case
            var dict = dictionary.Select(DistributionHelpers.IndependentApproximation).ToArray().Transpose();
            var coef = coefficients.Select(DistributionHelpers.IndependentApproximation).ToArray();

            PlotResults(numBases, signalWidth, dict, coef, st, plotOptions);
        }

        /// <summary>
        /// Plots the results.
        /// </summary>
        /// <returns>The results.</returns>
        /// <param name="numBases">Number of bases.</param>
        /// <param name="signalWidth">The signal width.</param>
        /// <param name="dictionary">Dictionary.</param>
        /// <param name="coefficients">Coefficients.</param>
        /// <param name="st">Sub title.</param>
        /// <param name="plotOptions">Plotting options.</param>
        public static void PlotResults(
            int numBases, int signalWidth,
            Gaussian[][] dictionary, VectorGaussian[] coefficients,
            string st, PlotOptions plotOptions)
        {
            var coef = coefficients.Select(DistributionHelpers.IndependentApproximation).ToArray();
            PlotResults(numBases, signalWidth, dictionary, coef, st, plotOptions);
        }

        /// <summary>
        /// Plots the reconstructions.
        /// </summary>
        /// <returns>The reconstructions.</returns>
        /// <param name="reconstructions">Reconstructions.</param>
        /// <param name="averageError">The average reconstruction error.</param>
        /// <param name="subTitle">Sub title.</param>
        /// <param name="normalised">Whether these are normalised reconstructions.</param>
        /// <param name="plotOptions">Plotting options.</param>
        public static void PlotReconstructions(Reconstruction[] reconstructions, double averageError, string subTitle,
            bool normalised, PlotOptions plotOptions)
        {
            if (plotOptions.IsImage)
            {
                PlotImageReconstructions(reconstructions, averageError, subTitle, normalised, plotOptions);
                return;
            }

            var series1 = reconstructions.Take(plotOptions.Reconstructions.NumToShow).Select(
                (ia, i) => (ISeries)(new LineSeries
                {
                    Label = "signal",
                    X = ia.Signal,
                    Row = i/plotOptions.Reconstructions.Subplots.Columns,
                    Column = i%plotOptions.Reconstructions.Subplots.Columns
                })).ToArray();
            var series2 = reconstructions.Take(plotOptions.Reconstructions.NumToShow).Select(
                (ia, i) => (ISeries)(new ErrorLineSeries
                {
                    Label = "reconstruction",
                    ErrorLabel = "$\\pm$s.d.",
                    X = ia.Estimate.GetMeans(),
                    ErrorValues = ia.Estimate.GetStandardDeviations(),
                    Row = i/plotOptions.Reconstructions.Subplots.Columns,
                    Column = i%plotOptions.Reconstructions.Subplots.Columns
                })).ToArray();

            IList<ISeries> series = series1.Concat(series2).ToArray();

            string n = normalised ? " (normalised)" : string.Empty;

            // var series = new[] { new LineSeries { X = x1, Row = 0 }, new LineSeries { X = x2, Row = 1 } };
            string sub = string.IsNullOrEmpty(subTitle) ? string.Empty : $"_{subTitle.Replace(" ", "_")}";
            var plotter = new Plotter
            {
                Title = $"Reconstructions{n}, RMSE={averageError:N4}",
                XLabel = "x",
                YLabel = "y",
                Series = series,
                Subplots = plotOptions.Reconstructions.Subplots,
                ScriptName = Path.Combine(ScriptPath, $"Reconstructions_{n}{sub}.py"),
                FigureName = Path.Combine(FigurePath, $"Reconstructions_{n}{sub}.pdf"),
                Python = PythonPath,
                Show = plotOptions.Reconstructions.Show
            };
            plotter.Plot();
        }

        /// <summary>
        /// Plots the image reconstructions. Note that we assume the images are square
        /// </summary>
        /// <returns>The reconstructions.</returns>
        /// <param name="reconstructions">Reconstructions.</param>
        /// <param name="averageError">The average reconstruction error.</param>
        /// <param name="subTitle">Sub title.</param>
        /// <param name="normalised">Whether these are normalised reconstructions.</param>
        /// <param name="plotOptions">Plotting options.</param>
        public static void PlotImageReconstructions(Reconstruction[] reconstructions, double averageError,
            string subTitle, bool normalised, PlotOptions plotOptions)
        {
            var series1 = reconstructions.Take(plotOptions.Reconstructions.NumToShow).Select(
                (ia, i) => (ISeries) new HintonSeries
                {
                    Label = "signal",
                    Values = Reshape(Vector.Build.Dense(ia.Signal)),
                    Row = i,
                    Column = 0
                }).ToArray();
            var series2 = reconstructions.Take(plotOptions.Reconstructions.NumToShow).Select(
                (ia, i) => (ISeries) new HintonSeries
                {
                    Label = "reconstruction",
                    // ErrorLabel = "$\\pm$s.d.",
                    Values = Reshape(Vector.Build.Dense(ia.Estimate.GetMeans())),
                    // ErrorValues = ia.Estimate.GetStandardDeviations(),
                    Row = i,
                    Column = 1
                }).ToArray();

            IList<ISeries> series = series1.Concat(series2).ToArray();

            string n = normalised ? "(normalised)" : string.Empty;

            var subplots = new Subplots {Rows = plotOptions.Reconstructions.NumToShow, Columns = 2, ShareX = false, ShareY = false};
            string sub1 = string.IsNullOrEmpty(subTitle) ? string.Empty : $" {subTitle.Replace("_", " ")}";
            string sub2 = string.IsNullOrEmpty(subTitle) ? string.Empty : $"_{subTitle.Replace(" ", "_")}";
            string message = $"Reconstructions {n}{sub1}, avg. error={averageError:N4}";
            Console.WriteLine(message);
            var plotter = new Plotter
            {
                Title = message,
                XLabel = "x",
                YLabel = "y",
                Grid = false,
                Series = series,
                Subplots = subplots,
                ScriptName = Path.Combine(ScriptPath, $"Reconstructions_{n}{sub2}.py"),
                FigureName = Path.Combine(FigurePath, $"Reconstructions_{n}{sub2}.pdf"),
                Python = PythonPath,
                Show = plotOptions.Reconstructions.Show
            };
            plotter.Plot();
        }

        // /// <summary>
        // /// Plots the reconstructions.
        // /// </summary>
        // /// <returns>The reconstructions.</returns>
        // /// <param name="signals">Signals.</param>
        // /// <param name="reconstructions">Reconstructions.</param>
        // /// <param name="title">Title.</param>
        // /// <param name="numToShow">Number to show.</param>
        // /// <param name="rows">Rows.</param>
        // /// <param name="cols">Cols.</param>
        // /// <param name="subTitle">Sub title.</param>
        // public static void PlotReconstructions(double[][] signals, double[][] reconstructions, string title, int numToShow, int rows, int cols, string subTitle = null)
        // {
        //     var r = signals.Zip(reconstructions, (s, e) => new Reconstruction { Signal = s, Estimate = e.Select(Gaussian.PointMass).ToArray() }).ToArray();
        //     PlotReconstructions(r, averageError, title, numToShow, rows, cols, subTitle);
        // }

        /// <summary>
        /// Plots the posteriors.
        /// </summary>
        /// <returns>The posteriors.</returns>
        /// <param name="posteriors">Posteriors.</param>
        /// <param name="title">Title.</param>
        /// <param name="subTitle">Sub title.</param>
        /// <param name="options">Plot options.</param>
        public static void PlotPosteriors<T>(
            T[][] posteriors,
            string title,
            string subTitle,
            BaseOptions options)
            where T : IDistribution<double>, CanGetMean<double>, CanGetVariance<double>
        {
            var series = CreateSeries(posteriors, null, options);

            // var series = new[] { new LineSeries { X = x1, Row = 0 }, new LineSeries { X = x2, Row = 1 } };
            string sub = string.IsNullOrEmpty(subTitle) ? string.Empty : $"_{subTitle.Replace(" ", "_")}";
            string sel = $"{options.Skip}-{options.Skip + options.NumToShow}";
            var plotter = new Plotter 
            { 
                Title = $"{title} {sel}",
                XLabel = "x", YLabel = "y", Series = series, Subplots = options.Subplots,
                Python = PythonPath,
                ScriptName = Path.Combine(ScriptPath, $"{title}{sub}_{sel}.py"),
                FigureName = Path.Combine(FigurePath, $"{title}{sub}_{sel}.pdf"),
                Show = options.Show
            };
            
            plotter.Plot();
        }
        
        /// <summary>
        /// Plot errors with evidence on twinx
        /// </summary>
        public static void PlotErrorsWithEvidence(IList<double> bases, IList<double> errors, IList<double> evidence, bool show = false)
        {
            // Here we're going to customise the Plotter.TwinPlot function
            const string title = "Effect of number of bases";
            const string xlabel = "#bases";
            const string y1Label = "Reconstruction error";
            const string y2Label = "Log Evidence";
            
            var series1 = new ISeries[] { new LineSeries { X = bases, Y = errors, Color = "next(palette)", Label = "Reconstruction error" } };
            var series2 = new ISeries[] { new LineSeries { X = bases, Y = evidence, Color = "next(palette)", Label = "Evidence" } };

			// Here we build the plotting script for the second plot (without the pre/postamble), 
			// so we can append it to the script for the first plot
            var plotter2 = new Plotter { XLabel = xlabel, YLabel = y2Label, Series = series2, TwinX = true };
            plotter2.BuildScript();

            // TODO: http://matplotlib.org/examples/api/two_scales.html

            var plotter1 = new Plotter 
            { 
                Title = title, 
                XLabel = xlabel, 
                YLabel = y1Label, 
                Series = series1, 
                Python = PythonPath,
                ScriptName = Path.Combine(ScriptPath, "EffectOfBases"),
                FigureName = Path.Combine(FigurePath, "EffectOfBases"),
                Show = show
            };
            
            plotter1.Plot(plotter2.Script);
            
        }

        /// <summary>
        /// Plots the image.
        /// </summary>
        /// <param name="imageFlat">Image flat.</param>
        /// <param name="show">Whether to show the plot.</param>
        public static void PlotImage(Vector imageFlat, bool show = false)
		{
			
			// DenseOfColumnMajor(rows, columns, m.Row(0));
			// Plotter.Hinton(image);
			var plotter = new Plotter 
            { 
                Series = new ISeries[] { new MatrixSeries { Values = Reshape(imageFlat) } }, 
                Grid = false,
                Python = PythonPath,
                ScriptName = Path.Combine(ScriptPath, "EffectOfBases"),
                FigureName = Path.Combine(FigurePath, "EffectOfBases"),
                Show = show
            };
			plotter.Plot();
		}

        /// <summary>
        /// Plots the images.
        /// </summary>
        /// <param name="imagesFlat">Images.</param>
        /// <param name="title">The title.</param>
        /// <param name="options">Plotting options.</param>
        public static void PlotImages(Matrix imagesFlat, string title, BaseOptions options)
        {
            var series =
                (from t in imagesFlat.EnumerateRowsIndexed()
                    let index = t.Item1
                    let image = Reshape(t.Item2)
                    select
                        new MatrixSeries
                        {
                            Values = image,
                            Row = index/options.Subplots.Columns,
                            Column = index%options.Subplots.Columns
                        }
                    ).Cast<ISeries>().ToList();

            var plotter = new Plotter
            {
                Title = title,
                Series = series, 
                Grid = false, 
                Subplots = options.Subplots,
                Python = PythonPath,
                ScriptName = Path.Combine(ScriptPath, "EffectOfBases"),
                FigureName = Path.Combine(FigurePath, "EffectOfBases"),
                Show = options.Show
            };

			plotter.Plot();
		}
        
        public static void PlotImages(Gaussian[][] images, int imageWidth, string title, BaseOptions options)
        {
            var dictionary = Matrix.Build.DenseOfRowArrays(images.GetMeans<Gaussian>());
            PlotImages(dictionary.SubMatrix(0, options.Subplots.Rows * options.Subplots.Columns, 0, imageWidth), title, options);
        }

        public static ISeries[] CreateSeries(double[][] data, string[] labels, BaseOptions options)
        {
            ISeries[] series;

            switch (options.PlotType)
            {
                case PlotType.Line:
                    series = data.Skip(options.Skip).Take(options.NumToShow).Select(
                        (ia, i) => (ISeries) (new ErrorLineSeries
                        {
                            Label = labels?[i],
                            X = ia,
                            Row = i/options.Subplots.Columns,
                            Column = i%options.Subplots.Columns
                        })).ToArray();
                    break;
                case PlotType.Bar:
                    series = data.Skip(options.Skip).Take(options.NumToShow).Select(
                        (ia, i) => (ISeries) (new BarSeries<string>
                        {
                            Label = labels?[i],
                            DependentValues = ia,
                            Row = i/options.Subplots.Columns,
                            Column = i%options.Subplots.Columns
                        })).ToArray();
                    break;
                default:
                    throw new ArgumentException("Unknonw plot type", nameof(options.PlotType));
            }

            return series;
        }

        public static
            ISeries[] CreateSeries<T>(T[][] data, string[] labels, BaseOptions options)
            where T : IDistribution<double>, CanGetMean<double>, CanGetVariance<double>
        {
            ISeries[] series;

            switch (options.PlotType)
            {
                case PlotType.ErrorLine:
                    series = data.Skip(options.Skip).Take(options.NumToShow).Select(
                        (ia, i) => (ISeries)(new ErrorLineSeries
                        {
                            Label = labels?[i],
                            X = ia.Select(x => x.GetMean()).ToArray(),
                            ErrorValues = ia.GetStandardDeviations(),
                            Row = i / options.Subplots.Columns,
                            Column = i % options.Subplots.Columns
                        })).ToArray();
                    break;
                case PlotType.Bar:
                    series = data.Skip(options.Skip).Take(options.NumToShow).Select(
                        (ia, i) => (ISeries)(new BarSeries<string>
                        {
                            Label = labels?[i],
                            DependentValues = ia.GetMeans(),
                            Row = i / options.Subplots.Columns,
                            Column = i % options.Subplots.Columns
                        })).ToArray();
                    break;
                case PlotType.ErrorBar:
                    series = data.Skip(options.Skip).Take(options.NumToShow).Select(
                        (ia, i) => (ISeries)(new BarSeries<string>
                        {
                            Label = labels?[i],
                            DependentValues = ia.GetMeans(),
                            ErrorValues = ia.GetStandardDeviations(),
                            Row = i / options.Subplots.Columns,
                            Column = i % options.Subplots.Columns
                        })).ToArray();
                    break;
                case PlotType.Line:
                    series = data.Skip(options.Skip).Take(options.NumToShow).Select(
                        (ia, i) => (ISeries)(new LineSeries
                        {
                            Label = labels?[i],
                            X = ia.GetMeans(),
                            Row = i / options.Subplots.Columns,
                            Column = i % options.Subplots.Columns
                        })).ToArray();
                    break;
                default:
                    throw new ArgumentException("Unknonw plot type", nameof(options.PlotType));
            }

            return series;
        }

        /// <summary>
        /// Reshape the specified flat image.
        /// </summary>
        /// <param name="imageFlat">Image flat.</param>
        public static double[][] Reshape(Vector imageFlat)
        {
            var sz = (int)Math.Sqrt(imageFlat.Count);
            var image = new double[sz][];
            for (int i = 0; i < sz; i++)
            {
                image[i] = imageFlat.SubVector(i * sz, sz).ToArray();
            }

            return image;
        }
    }
}