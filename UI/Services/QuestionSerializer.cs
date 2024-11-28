using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core.Questions;
using Core.Questions.Word;
using Core.Utils.Errors;
using UI.ViewModels.Questions;

namespace UI.Services;

public class QuestionSerializer {
    
    private static readonly string TrackedDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FileQuestionAssistant");
    private static string TrackedFilePath => Path.Combine(TrackedDirectoryPath, "TrackedQuestions.txt");
    
    public FilePickerFileType FileType { get; } = new("JSON file") {
        Patterns = ["*.json"],
        AppleUniformTypeIdentifiers = ["public.json"],
        MimeTypes = ["application/json"]
    };
    private readonly JsonSerializerOptions? _options = new() {
        IncludeFields = true,
        WriteIndented = true,
    };

    private static IQuestion? FromTypeNameToObj(QuestionData? data) {
        if (data is null) return null;
        //TODO: add other question types
        return data.Type switch {
            QuestionType.CreateStyleQuestion => new CreateStyleQuestion(data),
            _ => throw new UnreachableException()
        };
    }

    public async Task AddTrackedQuestion(IStorageFile file) {
        try {
            var filePath = Uri.UnescapeDataString(file.Path.AbsolutePath);
            var paths = new List<string>(await File.ReadAllLinesAsync(TrackedFilePath));

            if (paths.Any(line => line == filePath)) 
                throw new UnableToOpenQuestion();
            paths.Add(filePath); 
            
            await using var stream = File.Open(TrackedFilePath, FileMode.Create);
            await using var streamWriter = new StreamWriter(stream);
            foreach (var p in paths)
                await streamWriter.WriteLineAsync(p);
        } catch (Exception e) when (e is not UIException) {
            throw new FileError(file.Name, e);
        }
    }

    public async Task RemoveTrackedQuestion(SingleQuestionViewModel vm) {
        try {
            var paths = new List<string>(await File.ReadAllLinesAsync(TrackedFilePath));
            if (paths.Count == 0)
                return;
            paths.RemoveAt(vm.Index);
        
            await using var stream = File.Open(TrackedFilePath, FileMode.Create);
            await using var streamWriter = new StreamWriter(stream);
            foreach (var p in paths)
                await streamWriter.WriteLineAsync(p);
        } catch (Exception e) {
            throw new FileError("TrackedQuestions.txt", e);
        }
    }

    public IQuestion[]? LoadTrackedQuestions() {
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
                    using var stream = File.OpenRead(p);
                    using var streamReader = new StreamReader(stream);
                    return FromTypeNameToObj(JsonSerializer.Deserialize<QuestionData>(streamReader.ReadToEnd(), _options));
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

    public async Task<bool> Create(IStorageFile file, AbstractQuestion question) {
        var overwritten = false;
        try {
            await AddTrackedQuestion(file);
        } catch (Exception _) {overwritten = true;}
        
        await Save(file, question);
        return overwritten;
    }
    
    public async Task Save(IStorageFile file, AbstractQuestion question) {
        try {
            using var stream = file.OpenWriteAsync();
            await using var streamWriter = new StreamWriter(await stream);
            await streamWriter.WriteLineAsync(JsonSerializer.Serialize(question.Data, _options));
        } catch (Exception e) {
            throw new FileError(file.Name, e);
        }
    }
    
    public async Task<IQuestion?> Load(IStorageFile file) {
        try {
            await using var stream = await file.OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            return FromTypeNameToObj(JsonSerializer.Deserialize<QuestionData>(await streamReader.ReadToEndAsync(), _options));
        } catch (Exception e) {
            throw new FileError(file.Name, e);
        }
    }

}