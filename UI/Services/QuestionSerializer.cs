using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.FileHandling;
using Core.Questions;
using Core.Questions.Word;
using Core.Utils.Errors;
using UI.ViewModels.Questions;

namespace UI.Services;

public class QuestionSerializer {
    
    private static readonly string TrackedDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FileQuestionAssistant");
    private static string TrackedFilePath => Path.Combine(TrackedDirectoryPath, "TrackedQuestions.txt");

    private IServiceProvider _services;

    private readonly JsonSerializerOptions? _options = new() {
        IncludeFields = true,
        WriteIndented = true
    };

    public void Init(IServiceProvider services) {
        _services = services;
    }

    private SingleQuestionViewModel? FromTypeNameToObj(string typeName, QuestionData? data) {
        if (data is null) return null;
        //TODO: add other question types
        return typeName switch {
            "CreateStyleQuestion" => new CreateStyleQuestionVM(new CreateStyleQuestion(data), _services),
            _ => throw new UnreachableException()
        };
    }

    public SingleQuestionViewModel[]? LoadTrackedQuestions() {
        if (!Directory.Exists(TrackedDirectoryPath))
            Directory.CreateDirectory(TrackedDirectoryPath);
        if (!File.Exists(TrackedFilePath)) {
            File.Create(TrackedFilePath);
            return null;
        }
        
        try {
            var paths = File.ReadAllLines(TrackedFilePath);
            return paths.Select(p => {
                try {
                    using var stream = File.OpenRead(p.Split("-")[1]);
                    using var streamReader = new StreamReader(stream);
                    return FromTypeNameToObj(p.Split("-")[0], JsonSerializer.Deserialize<QuestionData>(streamReader.ReadToEnd(), _options));
                } catch (Exception e) {
                    Console.WriteLine(e);
                    return null;
                }
            }).Where(q => q is not null).ToArray()!;
        } catch (Exception e) {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task Create<TQuestion, TFile>(IStorageFile file, TQuestion question) where TQuestion : AbstractQuestion<TFile> where TFile : IFile {
        await using var streamWriter = new StreamWriter(File.Open(TrackedFilePath, FileMode.Append));
        await streamWriter.WriteLineAsync(typeof(TQuestion).Name + "-" + Uri.UnescapeDataString(file.Path.AbsolutePath));
        await Save<TQuestion, TFile>(file, question);
        
    }
    
    public async Task Save<TQuestion, TFile>(IStorageFile file, TQuestion question) where TQuestion : AbstractQuestion<TFile> where TFile : IFile {
        try {
            using var stream = file.OpenWriteAsync();
            await using var streamWriter = new StreamWriter(await stream);
            await streamWriter.WriteLineAsync(JsonSerializer.Serialize(question.Data, _options));
        } catch (Exception e) {
            throw new FileError(file.Name, e);
        }
    }
    
    public async Task<SingleQuestionViewModel?> Load<TQuestion, TFile>(IStorageFile[] files) where TQuestion : AbstractQuestion<TFile> where TFile : IFile {
        try {
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            var data = JsonSerializer.Deserialize<QuestionData>(await streamReader.ReadToEndAsync(), _options);
            return FromTypeNameToObj(typeof(TQuestion).Name, data);
        } catch (Exception e) {
            throw new FileError(files[0].Name, e);
        }
    }

}