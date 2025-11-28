using Newtonsoft.Json;
using OllamaClient.Dto;
using OllamaClient.Helpers;
using OllamaClient.OllamaUtils;
using System.IO;
using System.Reflection;
using System.Windows;

namespace OllamaClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

            this.Title = $"Ollama тестовый клиент {fvi.FileVersion}";

            SetStatusBar("Ready to work...", DateTime.Now, null);
        }

        private async void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            var model = cbOllamaModels.Text;

            var fileResult = FileUtils.GetFileContentBase64(tbFileName.Text.Trim());
            var requestTime = DateTime.UtcNow;
            SetStatusBar("Отправляем запрос", requestTime, null);
            BaseResultDto result = null!;
            if (fileResult.Item1)
                result = await OllamaApiClient.SendPromptAsync(cbOllamaModels.Text, OllamaRequest.Text, new string[] { fileResult.Item2 });
            else
                result = await OllamaApiClient.SendPromptAsync(cbOllamaModels.Text, OllamaRequest.Text, null);

            if (!result.IsValid)
                MessageBox.Show($"Модель: {model}{Environment.NewLine}Ошибка:{result.Result}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                this.OllamaResponseFull.Text = result.Result;
                if (result.Result is not null)
                {
                    var resultDto = JsonConvert.DeserializeObject<ResultDto>(result.Result);
                    this.OllamaResponsePayload.Text = string.IsNullOrEmpty(resultDto?.Response) ? resultDto?.Thinking : resultDto.Response;
                }
                var responseTime = DateTime.UtcNow;
                SetStatusBar("Получен ответ", requestTime, responseTime);

                this.OllamaResponsePayload.Text += Environment.NewLine;
                this.OllamaResponsePayload.Text += Environment.NewLine;
                this.OllamaResponsePayload.Text += Environment.NewLine;

                this.OllamaResponsePayload.Text += $"Длительность: {(responseTime - requestTime).ToString()}";
            }
        }

        private void SetStatusBar(string stText, DateTime requestTime, DateTime? responseTime)
        {
            lbState.Text = stText;
            lbRequestTime.Text = $"Время отправки: {requestTime.ToString()}";
            lbResponseTime.Text = $"Время получения ответа: {responseTime.ToString()}";
            lbDuration.Text = responseTime.HasValue ? $"Длительность: {(responseTime - requestTime).ToString()}" : lbDuration.Text = string.Empty;
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Очистить поля?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.OllamaRequest.Text = string.Empty;
                this.OllamaResponseFull.Text = string.Empty;
                //this.cbOllamaModels.Items.Clear();
                this.OllamaResponsePayload.Text = string.Empty;
            }
        }

        private async void Button_List_Click(object sender, RoutedEventArgs e)
        {
            var requestTime = DateTime.UtcNow;
            SetStatusBar("Запрос на получение списка моделей отправлен", requestTime, null);
            var modelsJson = await OllamaApiClient.GetLocalModelsListAsync();
            OllamaResponseFull.Text = modelsJson;
            LoadModels(modelsJson);
            SetStatusBar("Список моделей получен", requestTime, DateTime.UtcNow);
        }

        private void LoadModels(string modelsJson)
        {
            Models? models = JsonConvert.DeserializeObject<Models>(modelsJson);

            if (models is null || models.models.Length == 0)
                return;

            cbOllamaModels.Items.Clear();

            foreach (var model in models.models)
            {
                cbOllamaModels.Items.Add(model.name);
            }
        }

        private void Button_SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var imagesDir = $"{appDirectory}\\Images";
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|GIF Files (*.gif)|*.gif|All Files (*.*)|*.*";
            if (Path.Exists(imagesDir))
            {
                dlg.DefaultDirectory = imagesDir;
            }

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                tbFileName.Text = filename;
            }
        }
    }
}