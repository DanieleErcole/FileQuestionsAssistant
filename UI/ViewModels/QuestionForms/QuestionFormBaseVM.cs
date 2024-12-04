using System;
using System.Threading.Tasks;
using Core.Questions;

namespace UI.ViewModels.QuestionForms;

public abstract class QuestionFormBaseVM(IServiceProvider services) : ViewModelBase {
    
    public static readonly Func<double, string> IntFormat = input => Math.Max(0, (int) input).ToString();

    protected IServiceProvider _services = services;
    
    public abstract Task<AbstractQuestion?> CreateQuestion();

}