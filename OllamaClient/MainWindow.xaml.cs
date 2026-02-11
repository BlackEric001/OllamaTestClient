using Newtonsoft.Json;
using OllamaClient.Dto;
using OllamaClient.Helpers;
using OllamaClient.OllamaUtils;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xaml;

namespace OllamaClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal SettingsDto _settings;

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
            (DateTime requestTime, BaseResultDto rs) result = (default, null!);
            var fileName = tbFileName.Text.Trim();

            if (fileName != string.Empty)
            {
                var fileResult = FileUtils.GetFileContentBase64(fileName);
                if (fileResult.Item1.CheckResult && fileResult.Item2 != null)
                {
                    result = await SendMessageToOllamaAsync(result.rs, fileResult.Item2);
                }
                else
                    MessageBox.Show($"{fileResult.Item1.CheckMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                result = await SendMessageToOllamaAsync(result.rs, null);
            }
           
            
            if (!result.rs.IsValid)
                MessageBox.Show($"Модель: {model}{Environment.NewLine}Ошибка:{result.rs.Result}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                this.tbOllamaResponseFull.Text = result.rs.Result;
                if (result.rs.Result is not null)
                {
                    var resultDto = JsonConvert.DeserializeObject<ResultDto>(result.rs.Result);
                    this.tbOllamaResponsePayload.Text = string.IsNullOrEmpty(resultDto?.Response) ? resultDto?.Thinking : resultDto.Response;
                }
                var responseTime = DateTime.UtcNow;
                SetStatusBar("Получен ответ", result.requestTime, responseTime);

                this.tbOllamaResponsePayload.Text += Environment.NewLine;
                this.tbOllamaResponsePayload.Text += Environment.NewLine;
                this.tbOllamaResponsePayload.Text += Environment.NewLine;

                this.tbOllamaResponsePayload.Text += $"Длительность: {(responseTime - result.requestTime).ToString()}";
            }
        }

        private async Task<(DateTime, BaseResultDto)> SendMessageToOllamaAsync(BaseResultDto result, string? fileBase64)
        {
            var requestTime = DateTime.UtcNow;
            SetStatusBar("Отправляем запрос", requestTime, null);
            if (!string.IsNullOrEmpty(fileBase64))
                result = await OllamaApiClient.SendPromptAsync(_settings, cbOllamaModels.Text, tbOllamaRequest.Text, new string[] { fileBase64 });
            else
                result = await OllamaApiClient.SendPromptAsync(_settings, cbOllamaModels.Text, tbOllamaRequest.Text, null);
            return (requestTime, result);
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
            MessageBoxResult result = MessageBox.Show("Очистить поля?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.tbOllamaRequest.Text = string.Empty;
                this.tbOllamaResponseFull.Text = string.Empty;
                //this.cbOllamaModels.Items.Clear();
                this.tbOllamaResponsePayload.Text = string.Empty;
                this.tbFileName.Clear();
            }
        }

        private async void Button_List_Click(object sender, RoutedEventArgs e)
        {
            var requestTime = DateTime.UtcNow;
            SetStatusBar("Запрос на получение списка моделей отправлен", requestTime, null);
            var modelsJson = await OllamaApiClient.GetLocalModelsListAsync(_settings);
            tbOllamaResponseFull.Text = modelsJson;
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

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_settings);
            settingsWindow.Owner = this;
            settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? result = settingsWindow.ShowDialog();

            if (result == true)
            {
                _settings = settingsWindow.GetSettings();
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            var fileInfo = FileUtils.ReadSettings();
            if (!fileInfo.Item1.CheckResult || fileInfo.Item2 == null)
            {
                MessageBox.Show($"{fileInfo.Item1.CheckMessage}", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                _settings = new SettingsDto();
            }
            else
            {
                _settings = fileInfo.Item2;
                ApplySettings();
            }
        }

        private void ApplySettings()
        {
            tbOllamaRequest.Text = _settings.DefaultPrompt;
            cbOllamaModels.Items.Add(_settings.DefaultModel);
            cbOllamaModels.SelectedIndex = 0;
        }
    }
}