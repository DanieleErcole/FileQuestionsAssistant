using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.FileHandling;
using Core.Utils.Errors;
using Microsoft.Extensions.DependencyInjection;
using ApplicationException = Core.Utils.Errors.ApplicationException;

namespace UI.ViewModels.Questions;

public abstract class WordQuestionViewModel(string name, string desc, IServiceProvider services) : SingleQuestionViewModel(name, desc, services) {
    
    protected override FilePickerFileType FileType => new("Word OpenXML files") {
        Patterns = ["*.docx"],
        AppleUniformTypeIdentifiers = ["org.openxmlformats.wordprocessingml.document"],
        MimeTypes = ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"]
    };
    
    public override async Task UploadFiles() {
        try {
            IFile[] files = await Task.WhenAll((await OpenFiles())
                .Select(async f => {
                    try {
                        return new WordFile(f.Name, await f.OpenReadAsync());
                    } catch (Exception e) { throw new FileError(f.Name, e); }
                })
                .ToArray()
            );
            if (files.Length == 0)
                return;

            _services.GetRequiredService<Evaluator>().AddFiles(Index, files);
        } catch (FileError e) {
            UIException ex = e;
            _services.GetRequiredService<WindowNotificationManager>().ShowError($"Error opening file: {e.Filename}", ex.ToString());
        }
    }
    
}