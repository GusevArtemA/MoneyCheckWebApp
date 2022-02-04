using System;

namespace MoneyCheckWebApp.Predications.InflationPredicating.NeuralNetwork
{
    public class InflationNeuralNetworkEngine
    {
        private readonly double[] hBiases;
        private readonly double[] hNodes;

        private readonly double[][] hoWeights;
        private readonly double[][] ihWeights;

        private readonly double[] iNodes;
        private readonly int numHidden;
        private readonly int numInput;
        private readonly int numOutput;
        private readonly double[] oBiases;
        private readonly double[] oNodes;

        private readonly Random rnd;

        public InflationNeuralNetworkEngine(int numInput, int numHidden, int numOutput)
        {
            this.numInput = numInput;
            this.numHidden = numHidden;
            this.numOutput = numOutput;

            iNodes = new double[numInput];

            ihWeights = MakeMatrix(numInput, numHidden, 0.0);
            hBiases = new double[numHidden];
            hNodes = new double[numHidden];

            hoWeights = MakeMatrix(numHidden, numOutput, 0.0);
            oBiases = new double[numOutput];
            oNodes = new double[numOutput];

            rnd = new Random(0);
            InitializeWeights();
        }

        private static double[][] MakeMatrix(int rows, int cols, double v)
        {
            var result = new double[rows][];
            for (var r = 0; r < result.Length; ++r)
                result[r] = new double[cols];
            for (var i = 0; i < rows; ++i)
            for (var j = 0; j < cols; ++j)
                result[i][j] = v;
            return result;
        }

        private void InitializeWeights()
        {
            var numWeights = numInput * numHidden + numHidden * numOutput + numHidden + numOutput;
            var initialWeights = new double[numWeights];
            
            for (var i = 0; i < initialWeights.Length; ++i)
            {
                initialWeights[i] = (0.001 - 0.0001) * rnd.NextDouble() + 0.0001;
            }
            
            SetWeights(initialWeights);
        }

        public void SetWeights(double[] weights)
        {
            var numWeights = numInput * numHidden + numHidden * numOutput + numHidden + numOutput;
            if (weights.Length != numWeights)
                throw new Exception("Bad weights array in SetWeights");

            var k = 0;

            for (var i = 0; i < numInput; ++i)
            {
                for (var j = 0; j < numHidden; ++j)
                {
                    ihWeights[i][j] = weights[k++];
                }
            }

            for (var i = 0; i < numHidden; ++i)
            {
                hBiases[i] = weights[k++];
            }
                
            for (var i = 0; i < numHidden; ++i)
            {
                for (var j = 0; j < numOutput; ++j)
                {
                    hoWeights[i][j] = weights[k++];
                }
            }
            
            for (var i = 0; i < numOutput; ++i)
            {
                oBiases[i] = weights[k++];
            }
        }

        public double[] GetWeights()
        {
            var numWeights = numInput * numHidden + numHidden * numOutput + numHidden + numOutput;
            var result = new double[numWeights];
            var k = 0;
            
            foreach (var ihWeight in ihWeights)
            {
                for (var j = 0; j < ihWeights[0].Length; ++j)
                {
                    result[k++] = ihWeight[j];
                }
            }
            
            foreach (var hBias in hBiases)
            {
                result[k++] = hBias;
            }
            
            foreach (var hoWeight in hoWeights)
            {
                for (var j = 0; j < hoWeights[0].Length; ++j)
                {
                    result[k++] = hoWeight[j];
                }
            }
            
            foreach (var bias in oBiases)
            {
                result[k++] = bias;
            }
            
            return result;
        }

        public double[] ComputeOutputs(double[] xValues)
        {
            var hSums = new double[numHidden];
            var oSums = new double[numOutput];

            for (var i = 0; i < xValues.Length; ++i)
            {
                iNodes[i] = xValues[i];
            }

            for (var j = 0; j < numHidden; ++j)
            {
                for (var i = 0; i < numInput; ++i)
                {
                    hSums[j] += iNodes[i] * ihWeights[i][j];
                }
            }
            
            for (var i = 0; i < numHidden; ++i)
            {
                hSums[i] += hBiases[i];
            }

            for (var i = 0; i < numHidden; ++i)
            {
                hNodes[i] = HyperTan(hSums[i]);
            }

            for (var j = 0; j < numOutput; ++j)
            {
                for (var i = 0; i < numHidden; ++i)
                {
                    oSums[j] += hNodes[i] * hoWeights[i][j];
                }
            }

            for (var i = 0; i < numOutput; ++i)
            {
                oSums[i] += oBiases[i];
            }

            Array.Copy(oSums, oNodes, oSums.Length);

            var retResult = new double[numOutput]; 
            Array.Copy(oNodes, retResult, retResult.Length);
            return retResult;
        }

        private static double HyperTan(double x)
        {
            if (x < -20.0) return -1.0;
            if (x > 20.0) return 1.0;
            return Math.Tanh(x);
        }

        public double[] Train(double[][] trainData, int maxEpochs, double learnRate)
        {
            var hoGrads = MakeMatrix(numHidden, numOutput, 0.0); 
            var obGrads = new double[numOutput]; 

            var ihGrads = MakeMatrix(numInput, numHidden, 0.0);
            var hbGrads = new double[numHidden]; 

            var oSignals = new double[numOutput];
            var hSignals = new double[numHidden];

            var epoch = 0;
            var xValues = new double[numInput];
            var tValues = new double[numOutput];
            var derivative = 0.0;
            var errorSignal = 0.0;

            var sequence = new int[trainData.Length];
            for (var i = 0; i < sequence.Length; ++i)
                sequence[i] = i;

            var errInterval = maxEpochs / 5;
            while (epoch < maxEpochs)
            {
                ++epoch;

                if (epoch % errInterval == 0 && epoch < maxEpochs)
                {
                    var trainErr = Error(trainData);
                }

                Shuffle(sequence);
                for (var ii = 0; ii < trainData.Length; ++ii)
                {
                    var idx = sequence[ii];
                    Array.Copy(trainData[idx], xValues, numInput);
                    Array.Copy(trainData[idx], numInput, tValues, 0, numOutput);
                    ComputeOutputs(xValues); 

                    for (var k = 0; k < numOutput; ++k)
                    {
                        errorSignal = tValues[k] - oNodes[k];
                        derivative = 1.0;
                        oSignals[k] = errorSignal * derivative;
                    }
                    
                    for (var j = 0; j < numHidden; ++j)
                    {
                        for (var k = 0; k < numOutput; ++k)
                        {
                            hoGrads[j][k] = oSignals[k] * hNodes[j];
                        }
                    }
                    
                    for (var k = 0; k < numOutput; ++k)
                    {
                        obGrads[k] = oSignals[k] * 1.0;
                    }
                    
                    for (var j = 0; j < numHidden; ++j)
                    {
                        derivative = (1 + hNodes[j]) * (1 - hNodes[j]); 
                        var sum = 0.0;
                        
                        for (var k = 0; k < numOutput; ++k)
                        {
                            sum += oSignals[k] * hoWeights[j][k];
                        }
                        
                        hSignals[j] = derivative * sum;
                    }
                    
                    for (var i = 0; i < numInput; ++i)
                    {
                        for (var j = 0; j < numHidden; ++j)
                        {
                            ihGrads[i][j] = hSignals[j] * iNodes[i];
                        }
                    }
                    
                    for (var j = 0; j < numHidden; ++j)
                    {
                        hbGrads[j] = hSignals[j] * 1.0;
                    }
                    
                    for (var i = 0; i < numInput; ++i)
                    {
                        for (var j = 0; j < numHidden; ++j)
                        {
                            var delta = ihGrads[i][j] * learnRate;
                            ihWeights[i][j] += delta; // would be -= if (o-t)
                        }
                    }
                    
                    for (var j = 0; j < numHidden; ++j)
                    {
                        var delta = hbGrads[j] * learnRate;
                        hBiases[j] += delta;
                    }
                    
                    for (var j = 0; j < numHidden; ++j)
                    {
                        for (var k = 0; k < numOutput; ++k)
                        {
                            var delta = hoGrads[j][k] * learnRate;
                            hoWeights[j][k] += delta;
                        }
                    }
                    
                    for (var k = 0; k < numOutput; ++k)
                    {
                        var delta = obGrads[k] * learnRate;
                        oBiases[k] += delta;
                    }
                } 
            }

            var bestWts = GetWeights();
            return bestWts;
        } 

        private void Shuffle(int[] sequence)
        {
            for (var i = 0; i < sequence.Length; ++i)
            {
                var r = rnd.Next(i, sequence.Length);
                (sequence[r], sequence[i]) = (sequence[i], sequence[r]);
            }
        } 

        private double Error(double[][] trainData)
        {
            var sumSquaredError = 0.0;
            var xValues = new double[numInput];
            var tValues = new double[numOutput];
            
            foreach (var data in trainData)
            {
                Array.Copy(data, xValues, numInput);
                Array.Copy(data, numInput, tValues, 0, numOutput); // get target values
                var yValues = ComputeOutputs(xValues);
                for (var j = 0; j < numOutput; ++j)
                {
                    var err = tValues[j] - yValues[j];
                    sumSquaredError += err * err;
                }
            }

            return sumSquaredError / trainData.Length;
        }

        public double Accuracy(double[][] testData, double howClose)
        {
            var numCorrect = 0;
            var numWrong = 0;
            var xValues = new double[numInput];
            var tValues = new double[numOutput];
            double[] yValues;

            foreach (var data in testData)
            {
                Array.Copy(data, xValues, numInput); 
                Array.Copy(data, numInput, tValues, 0, numOutput);
                yValues = ComputeOutputs(xValues);

                if (Math.Abs(yValues[0] - tValues[0]) < howClose)
                {
                    ++numCorrect;
                }
                else
                {
                    ++numWrong;
                }
            }

            return numCorrect * 1.0 / (numCorrect + numWrong);
        }
    }
}