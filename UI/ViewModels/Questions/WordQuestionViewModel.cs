using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils.Errors;
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
            var files = await Task.WhenAll((await OpenFiles())
                .Select(async f => new WordFile(f.Name, await f.OpenReadAsync()))
                .ToArray()
            );
            if (files.Length == 0)
                return;

            _services.GetRequiredService<Evaluator<WordFile>>().AddFiles(Index, files);
            FileCount = files.Length.ToString();
        } catch (UnauthorizedAccessException e) {
            UIException ex = e;
            _services.GetRequiredService<WindowNotificationManager>().ShowError("Error opening file", ex.ToString());
        } catch (ApplicationException e) {
            UIException ex = e;
            _services.GetRequiredService<WindowNotificationManager>().ShowError($"Error opening file: {(e as FileError)?.Filename ?? ""}", ex.ToString());
        }
    }

    public override void ClearFiles() {
        _services.GetRequiredService<Evaluator<WordFile>>().SetFiles(Index);
        FileCount = "0";
    }
    
}