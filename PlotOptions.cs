//
// PlotOptions.cs
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
    using PythonPlotter;

    public class PlotOptions
    {
        public static PlotOptions Default => new PlotOptions
        {
            Dictionary = new BaseOptions
            {
                Show = false,
                NumToShow = 16,
                PlotType = PlotType.ErrorLine,
                Subplots = new Subplots
                {
                    Rows = 4,
                    Columns = 4,
                    ShareX = true,
                    ShareY = true
                }
            },
            Coefficients = new BaseOptions
            {
                Show = false,
                NumToShow = 4,
                PlotType = PlotType.ErrorBar,
                Subplots = new Subplots
                {
                    Rows = 4,
                    Columns = 1,
                    ShareX = true,
                    ShareY = true
                }
            },
            Reconstructions =
                new ReconstructionOptions
                {
                    Show = false,
                    NumToShow = 2,
                    Subplots = new Subplots {Rows = 2, Columns = 1, ShareX = true, ShareY = true}
                },
            ClassifierWeights = new BaseOptions {Show = true},
            Images = new BaseOptions
            {
                Show = false,
                NumToShow = 16,
                PlotType = PlotType.Hinton,
                Subplots = new Subplots
                {
                    Rows = 4,
                    Columns = 4,
                    ShareX = true,
                    ShareY = true
                }
            },
            Signals = new BaseOptions
            {
                Show = false,
                NumToShow = 16,
                PlotType = PlotType.Line,
                Subplots = new Subplots
                {
                    Rows = 4,
                    Columns = 4,
                    ShareX = true,
                    ShareY = true
                }
            }
        };

        public static PlotOptions ShowAll
        {
            get
            {
                var options = Default;
                options.Dictionary.Show = true;
                options.Coefficients.Show = true;
                options.Reconstructions.Show = true;
                options.ClassifierWeights.Show = true;
                options.Images.Show = true;
                options.Signals.Show = true;
                return options;
            }
        }

        public bool IsImage { get; set; }
        public BaseOptions Dictionary { get; set; } = new BaseOptions();
        public BaseOptions Coefficients { get; set; } = new BaseOptions();
        public ReconstructionOptions Reconstructions { get; set; } = new ReconstructionOptions();
        public BaseOptions ClassifierWeights { get; set; } = new BaseOptions();
        public BaseOptions Images { get; set; } = new BaseOptions();
        public BaseOptions Signals { get; set; } = new BaseOptions();
    }

    public class BaseOptions
    {
        public bool Show { get; set; }
        public int Skip { get; set; }
        public int NumToShow { get; set; }
        public Subplots Subplots { get; set; }
        public PlotType PlotType { get; set; } = PlotType.Line;
    }

    public class ReconstructionOptions : BaseOptions
    {
        public PlotType OriginalType { get; set; }
        public PlotType ReconstructedType { get; set; }
    }
}