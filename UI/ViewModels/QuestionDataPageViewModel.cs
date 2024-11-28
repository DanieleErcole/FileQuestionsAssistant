using System;
using System.Drawing;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Questions.Word;
using Microsoft.Extensions.DependencyInjection;
using UI.Services;

namespace UI.ViewModels;

public class QuestionDataPageViewModel : ViewModelBase {
    
    private readonly IServiceProvider _services;

    public QuestionDataPageViewModel(IServiceProvider services) {
        _services = services;
    }
    
    public async Task AddQuestionTest() {
        var file = await _services.GetRequiredService<IStorageProvider>().SaveFilePickerAsync(new FilePickerSaveOptions {
            Title = "Save Text File",
            FileTypeChoices = [_services.GetRequiredService<QuestionSerializer>().FileType],
        });
        if (file is not null) {
            var q = new CreateStyleQuestion("Nome1", "Desc1", "", "CustomStyle", "Normal", 
                "Consolas", 12, Color.Fuchsia, "center");
            await _services.GetRequiredService<QuestionSerializer>().Create(file, q);
        }
    }
    
}