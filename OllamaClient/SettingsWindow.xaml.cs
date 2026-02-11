using OllamaClient.Dto;
using OllamaClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OllamaClient
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private SettingsDto _settings;

        public SettingsWindow(SettingsDto settings)
        {
            InitializeComponent();
            this._settings = settings;
            FillControls();
        }

        private void FillControls()
        {
            tbOllamaUrl.Text = _settings.OllamaUrl;
            tbOllamaTimeout.Text = _settings.OllamaTimeout.ToString();
            tbDefaultModel.Text = _settings.DefaultModel;
            tbDefaultPrompt.Text = _settings.DefaultPrompt;
            tbTemperature.Text = _settings.Temperature.ToString();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Сохранить изменения?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _settings.OllamaUrl = tbOllamaUrl.Text;
                    _settings.OllamaTimeout = Int32.Parse(tbOllamaTimeout.Text);
                    _settings.DefaultModel = tbDefaultModel.Text;
                    _settings.DefaultPrompt = tbDefaultPrompt.Text;
                    _settings.Temperature = Double.Parse(tbTemperature.Text);

                    FileUtils.SaveSettings(_settings);
                    this.DialogResult = true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        internal SettingsDto GetSettings()
        {
            return _settings;
        }
    }
}
