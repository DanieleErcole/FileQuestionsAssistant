using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Evaluation;
using Core.FileHandling;
using Core.Questions;
using Core.Utils.Errors;
using UI.Services;
using UI.Utils;
using ApplicationException = Core.Utils.Errors.ApplicationException;

namespace UI.ViewModels.Questions.Word;

public abstract class WordQuestionViewModel(AbstractQuestion q, Evaluator evaluator, IErrorHandlerService errorHandler, IStorageService storageService) 
    : SingleQuestionViewModel(q, evaluator, errorHandler, storageService) {
    public override string Icon => "/Assets/docx.svg";
    protected override FilePickerFileType FileType => FileTypesHelper.Word;
}