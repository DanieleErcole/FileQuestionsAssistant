using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Questions;
using UI.ViewModels.QuestionForms;
using UI.ViewModels.Questions;

namespace UI.Services;

public class QuestionTypeMapper {

    private readonly Dictionary<int, Type> _typeIndexMap;
    private readonly Dictionary<Type, Type> _viewModelTypeMap;
    private readonly Dictionary<Type, Type> _formTypeMap;

    public QuestionTypeMapper() {
        var uiTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .ToList();
        var qTypes = Assembly.GetAssembly(typeof(AbstractQuestion))!
            .GetTypes()
            .Where(t => typeof(AbstractQuestion).IsAssignableFrom(t) && !t.IsAbstract)
            .ToList();
        
        _viewModelTypeMap = qTypes.ToDictionary(q => q, q => GetViewModelType(uiTypes, q));
        _formTypeMap = qTypes.ToDictionary(q => q, q => GetFormTypes(uiTypes, q));
        _typeIndexMap = qTypes.ToDictionary(t => qTypes.FindIndex(q => t == q), t => t);
        return;

        Type GetViewModelType(List<Type> types, Type qType) => 
            types.Single(t => 
                !t.IsAbstract 
                && t.BaseType?.BaseType == typeof(QuestionViewModelBase)
                && t.GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Any(p => p.ParameterType.IsAssignableTo(qType))) is not null
            );
        
        Type GetFormTypes(List<Type> types, Type qType) => 
            types.Single(t => 
                !t.IsAbstract 
                && t.BaseType == typeof(QuestionFormVMBase) 
                && t.GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Any(p => p.ParameterType.IsAssignableTo(qType))) is not null
            );
    }

    public QuestionViewModelBase? ViewModel(Type qType, object?[] @params) =>
        _viewModelTypeMap[qType].GetConstructor([qType])?.Invoke(@params) as QuestionViewModelBase;
    
    public QuestionFormVMBase? FormViewModel(Type qType, object?[] @params) =>
        _formTypeMap[qType].GetConstructor([
            typeof(IErrorHandlerService),
            typeof(IStorageService),
            qType
        ])?.Invoke(@params) as QuestionFormVMBase;

    public Type TypeFromIndex(int index) =>
        _typeIndexMap[index];
    
    public int IndexFromType(Type t) =>
        _typeIndexMap.First(p => p.Value == t).Key;
    
}