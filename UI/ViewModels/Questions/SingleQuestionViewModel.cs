using System;
using System.Threading.Tasks;
using Core.Evaluation;
using Microsoft.Extensions.DependencyInjection;

namespace UI.ViewModels.Questions;

public abstract class SingleQuestionViewModel : ViewModelBase {
    
    protected readonly IServiceProvider _services;
    
    public int Index { get; set; }
    public string Name { get; }
    public string Description { get; }
    public abstract string Path { get; }
    public abstract string Icon { get; }

    protected SingleQuestionViewModel(string name, string desc, IServiceProvider services) {
        _services = services;
        Name = name;
        Description = desc;
    }
    
    public abstract Task UploadFiles();
    
    public void ClearFiles() {
        _services.GetRequiredService<Evaluator>().SetFiles(Index);
    }

}