using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Numerics;
using System.IO.Compression;
using System.IO;
using System.Windows.Forms;
using TextBox = System.Windows.Controls.TextBox;
using ProgressBar = System.Windows.Controls.ProgressBar;
using System.Linq;

namespace Laboratory_11
{
    public partial class MainWindow : Window
    {

        private FibonacciCalculator fibonacciCalculator;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalculateTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
                return;

            BigInteger N = BigInteger.Parse(NTextBox.Text); 
            BigInteger K = BigInteger.Parse(KTextBox.Text); 

            Task.Run(() =>
            {
                try
                {
                    BigInteger result = CalculateBinomialCoefficient(N, K);
                    Dispatcher.Invoke(() => ShowResult("Zad1A -> Task result: " + result));
                }
                catch (ArgumentException ex)
                {
                    ShowErrorMessage("[INFO] " + ex.Message);
                }
            });
        }

        private void CalculateDelegateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
                return;

            BigInteger N = BigInteger.Parse(NTextBox.Text); 
            BigInteger K = BigInteger.Parse(KTextBox.Text); 

            Func<BigInteger, BigInteger, BigInteger> calculateDelegate = CalculateBinomialCoefficient;
            calculateDelegate.BeginInvoke(N, K, ar =>
            {
                try
                {
                    BigInteger result = calculateDelegate.EndInvoke(ar);
                    Dispatcher.Invoke(() => ShowResult("Zad1B -> Delegate result: " + result));
                }
                catch (ArgumentException ex)
                {
                    ShowErrorMessage("[INFO] " + ex.Message);
                }
            }, null);
        }

        private async void CalculateAsyncButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
                return;

            BigInteger N = BigInteger.Parse(NTextBox.Text); 
            BigInteger K = BigInteger.Parse(KTextBox.Text); 

            try
            {
                BigInteger result = await Task.Run(() => CalculateBinomialCoefficient(N, K));
                ShowResult("Zad1C -> Async/Await result: " + result);
            }
            catch (ArgumentException ex)
            {
                ShowErrorMessage("[INFO] " + ex.Message);
            }
        }

        private bool CheckInput()
        {
            if (string.IsNullOrWhiteSpace(NTextBox.Text) || string.IsNullOrWhiteSpace(KTextBox.Text))
            {
                ShowErrorMessage("[INFO] Please enter values for N and K.");
                return false;
            }

            if ((!BigInteger.TryParse(NTextBox.Text, out BigInteger N) || N < 0) || (!BigInteger.TryParse(KTextBox.Text, out BigInteger K) || K < 0))
            {
                ShowErrorMessage("[INFO] Invalid value for N or K. Please enter a valid integer greater than or equal to 0.");
                return false;
            }

            if (N < K)
            {
                ShowErrorMessage("[INFO] N must be greater than or equal to K.");
                return false;
            }

            return true;
        }

        private BigInteger CalculateBinomialCoefficient(BigInteger N, BigInteger K)
        {
            BigInteger numerator = CalculateFactorial(N);
            BigInteger denominator1 = CalculateFactorial(K);
            BigInteger denominator2 = CalculateFactorial(N - K);

            return numerator / (denominator1 * denominator2);
        }

        private BigInteger CalculateFactorial(BigInteger number)
        {
            BigInteger result = 1;
            for (BigInteger i = 2; i <= number; i++)
            {
                result *= i;
            }
            return result;
        }

        private void ShowResult(string message)
        {
            if (ResultTextBox.LineCount > 14)
            {
                int index = ResultTextBox.Text.IndexOf("\n", StringComparison.Ordinal);
                if (index != -1)
                {
                    ResultTextBox.Text = ResultTextBox.Text.Substring(index + 1);
                }
            }

            ResultTextBox.Text += message + "\n";
            ResultTextBox.ScrollToEnd();
        }

        private void ShowErrorMessage(string message)
        {
            if (ResultTextBox.LineCount > 14)
            {
                int index = ResultTextBox.Text.IndexOf("\n", StringComparison.Ordinal);
                if (index != -1)
                {
                    ResultTextBox.Text = ResultTextBox.Text.Substring(index + 1);
                }
            }

            ResultTextBox.Text += message + "\n";
            ResultTextBox.ScrollToEnd();
        }

        private void StartFibonacciCalculation_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFibonacciTermsInput())
                return;

            int fibonacciTerms = int.Parse(FibonacciTermsTextBox.Text);

            fibonacciCalculator = new FibonacciCalculator(ResultTextBox, ProgressBar, fibonacciTerms);
        }

        private bool CheckFibonacciTermsInput()
        {
            if (string.IsNullOrWhiteSpace(FibonacciTermsTextBox.Text))
            {
                ShowErrorMessage("[INFO] Please enter the number of Fibonacci terms.");
                return false;
            }

            if (!int.TryParse(FibonacciTermsTextBox.Text, out int terms) || terms < 0)
            {
                ShowErrorMessage("[INFO] Please enter a valid positive integer or zero for the number of Fibonacci terms.");
                return false;
            }

            return true;
        }

        private async void CompressFiles_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var dirInfo = new DirectoryInfo(dialog.SelectedPath);
            var files = dirInfo.EnumerateFiles().ToList();
            if (files.Count == 0)
            {
                ShowErrorMessage("[INFO] No files found for compression.");
                return;
            }

            foreach (var fileInfo in files)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        using (var fs = fileInfo.OpenRead())
                        using (var os = File.Open(fileInfo.FullName + ".gz", FileMode.Create))
                        using (var gs = new GZipStream(os, CompressionMode.Compress))
                        {
                            fs.CopyTo(gs);
                        }
                    });
                    Dispatcher.Invoke(() => ShowResult($"Zad3 -> Successfully compressed file: {fileInfo.FullName}"));
                }
                catch (IOException ex)
                {
                    Dispatcher.Invoke(() => ShowErrorMessage($"[INFO] Error compressing file {fileInfo.FullName}: {ex.Message}"));
                }
            }
        }

        private async void DecompressFiles_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var dirInfo = new DirectoryInfo(dialog.SelectedPath);
            var files = dirInfo.EnumerateFiles("*.gz").ToList();
            if (files.Count == 0)
            {
                ShowErrorMessage("[INFO] No .gz files found for decompression.");
                return;
            }

            foreach (var fileInfo in files)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        using (var fs = fileInfo.OpenRead())
                        using (var os = File.Open(fileInfo.FullName.Replace(".gz", ""), FileMode.Create))
                        using (var gs = new GZipStream(fs, CompressionMode.Decompress))
                        {
                            gs.CopyTo(os);
                        }
                    });
                    Dispatcher.Invoke(() => ShowResult($"Zad3 -> Successfully decompressed file: {fileInfo.FullName}"));
                }
                catch (IOException ex)
                {
                    Dispatcher.Invoke(() => ShowErrorMessage($"[INFO] -> Error decompressing file {fileInfo.FullName}: {ex.Message}"));
                }
            }
        }


    }

    public class FibonacciCalculator
    {
        private TextBox resultTextBox;
        private ProgressBar progressBar;
        private BackgroundWorker worker = new BackgroundWorker();

        public FibonacciCalculator(TextBox resultTextBox, ProgressBar progressBar, int numberOfTerms)
        {
            this.resultTextBox = resultTextBox;
            this.progressBar = progressBar;

            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            progressBar.Visibility = Visibility.Visible;
            worker.RunWorkerAsync(numberOfTerms);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int n = (int)e.Argument;
            BigInteger result = Fibonacci(n, (BackgroundWorker)sender, e);
            e.Result = result;
        }

        private BigInteger Fibonacci(int n, BackgroundWorker worker, DoWorkEventArgs e)
        {
            BigInteger a = 0;
            BigInteger b = 1;
            for (int i = 0; i < n; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return 0;
                }

                BigInteger temp = a;
                a = b;
                b = temp + b;
                Thread.Sleep(25);
                worker.ReportProgress(((i + 1) * 100) / n);
            }
            return a;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                resultTextBox.Dispatcher.Invoke(() => resultTextBox.AppendText("[INFO] Fibonacci sequence calculation has been cancelled.\n"));
            }
            else if (e.Error != null)
            {
                resultTextBox.Dispatcher.Invoke(() => resultTextBox.AppendText("[INFO] An error occurred while calculating the Fibonacci sequence: " + e.Error.Message + "\n"));
            }
            else
            {
                resultTextBox.Dispatcher.Invoke(() => resultTextBox.AppendText("Zad2 -> Fibonacci sequence result: " + e.Result.ToString() + "\n"));
            }
        }

    }
}
