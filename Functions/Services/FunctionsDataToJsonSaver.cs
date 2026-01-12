using Functions.Models;
using Functions.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Functions.Services
{
    public class FunctionsDataToJsonSaver : IFunctionsDataToJsonSaver
    {
        public IEnumerable<FunctionPointViewModel>? GetFunctionsDataFromFile()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "JSON files (*.json)|*.json",
                FilterIndex = 1,
                Title = "Select JSON File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    using var dataFileStream = File.OpenRead(filePath);
                    var deserialized = JsonSerializer.Deserialize<FunctionsPointsJson>(dataFileStream);
                    return deserialized?.Data;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "Error");
                }
            }

            return null;
        }

        public bool SaveToFile(IEnumerable<FunctionPointViewModel> functionPoints)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                FilterIndex = 1,
                Title = "Create JSON File",
                FileName = "data.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                if (!Path.GetExtension(filePath).Equals(".json", StringComparison.OrdinalIgnoreCase))
                {
                    filePath = Path.ChangeExtension(filePath, ".json");
                }

                var jsonContent = JsonSerializer.Serialize(
                    new FunctionsPointsJson
                    {
                        Data = functionPoints
                    });

                File.WriteAllText(filePath, jsonContent);

                return true;
            }

            return false;
        }
    }
}
