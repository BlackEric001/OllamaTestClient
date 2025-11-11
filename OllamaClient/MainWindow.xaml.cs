using Newtonsoft.Json;
using OllamaClient.Dto;
using OllamaClient.Helpers;
using OllamaClient.OllamaUtils;
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

            this.lbState.Text = "Ready";
            this.lbLastUpdateTime.Text = $"Время старта: {DateTime.Now.ToString()}";
        }

        private async void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            var model = cbOllamaModels.Text;

            var fileResult = FileUtils.GetFileContentBase64(tbFileName.Text.Trim());

            SetStatusBar("Отправляем запрос");
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
                    this.OllamaResponsePayload.Text = resultDto?.Response;
                }
            }
        }

        private void SetStatusBar(string stText)
        {
            this.lbState.Text = stText;
            this.lbLastUpdateTime.Text = $"Время отправки: {DateTime.UtcNow.ToString()}";
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
            SetStatusBar("Запрос на получение списка моделей отправлен");
            var modelsJson = await OllamaApiClient.GetLocalModelsListAsync();
            OllamaResponseFull.Text = modelsJson;
            LoadModels(modelsJson);
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
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

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