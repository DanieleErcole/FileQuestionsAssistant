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
using UI.Utils;

namespace UI.ViewModels.Questions;

public abstract class WordQuestionViewModel(string name, string desc, IServiceProvider services) : SingleQuestionViewModel(name, desc, services) {
    
    public override string Icon => "/Assets/docx.svg";
    
    public override async Task AddFiles() {
        try {
            var pickerFiles =  await _services.Get<IStorageProvider>().OpenFilePickerAsync(new FilePickerOpenOptions {
                AllowMultiple = true,
                FileTypeFilter = [FileTypesHelper.Word]
            });
            
            IFile[] files = await Task.WhenAll(pickerFiles
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
            _services.Get<ErrorHandler>().ShowError(e);
        }
    }
    
}