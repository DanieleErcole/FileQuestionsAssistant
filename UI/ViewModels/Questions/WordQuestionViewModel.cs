using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.FileHandling;
using Microsoft.Extensions.DependencyInjection;
using UI.Services;
using ApplicationException = Core.Utils.Errors.ApplicationException;

namespace UI.ViewModels.Questions;

public abstract class WordQuestionViewModel(IServiceProvider services) : SingleQuestionViewModel(services) {
    
    protected override FilePickerFileType FileType => new("Word OpenXML files") {
        Patterns = new [] { "*.docx" },
        AppleUniformTypeIdentifiers = new [] { "org.openxmlformats.wordprocessingml.document" },
        MimeTypes = new [] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
    };
    
    public override async void UploadFiles() {
        try {
            var openFiles = await _services.GetRequiredService<IStorageProvider>().OpenFilePickerAsync(new FilePickerOpenOptions {
                AllowMultiple = true,
                FileTypeFilter = new[] { FileType }
            });

            if (!openFiles.Any()) return;

            var files = await Task.WhenAll(openFiles
                .Select(async f => new WordFile(f.Name, await f.OpenReadAsync()))
                .ToArray()
            );
            _services.GetRequiredService<Evaluator<WordFile>>().SetFiles(Index, files);
            FileCount = files.Length.ToString();
        } catch (ApplicationException e) {
            UIException ex = e;
            _services.GetRequiredService<DialogService>().ShowMessageDialog(ex.ToString());
        }
    }

    public override void ClearFiles() {
        _services.GetRequiredService<Evaluator<WordFile>>().SetFiles(Index);
        FileCount = "0";
    }
    
}