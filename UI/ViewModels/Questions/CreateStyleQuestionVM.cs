using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions.Word;
using Microsoft.Extensions.DependencyInjection;

namespace UI.ViewModels.Questions;

public class CreateStyleQuestionVM : SingleQuestionViewModel {
    
    public override string Name => Lang.Lang.CreateStyleQuestionName;
    public override string Description => Lang.Lang.CreateStyleQuestionDesc;

    protected override FilePickerFileType FileType => new("Word OpenXML files") {
        Patterns = new [] { "*.docx" },
        AppleUniformTypeIdentifiers = new [] { "org.openxmlformats.wordprocessingml.document" },
        MimeTypes = new [] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
    };

    public CreateStyleQuestionVM(CreateStyleQuestion question, IServiceProvider services) : base(services) {
        var e = _services.GetRequiredService<Evaluator<WordFile>>();
        e.AddQuestion(question);
        Index = e.Questions.IndexOf(question);
    }
    
    public override async void UploadFiles() {
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
    }
    
}