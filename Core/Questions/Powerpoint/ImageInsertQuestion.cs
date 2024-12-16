using System.Text.Json.Serialization;
using Core.Evaluation;
using Core.FileHandling;

namespace Core.Questions.Powerpoint;

public class ImageInsertQuestion : AbstractQuestion {

    public ImageInsertQuestion(string path, string name, string? desc, byte[] ogFile) : base(path, name, desc, ogFile) {
        
    }
    
    [JsonConstructor]
    public ImageInsertQuestion(string name, string desc, byte[] ogFile, Dictionary<string, object?> Params) 
        : base(name, desc, ogFile, Params) { }

    protected override void DeserializeParams() {
        throw new NotImplementedException();
    }
    
    public override IEnumerable<Result> Evaluate(IEnumerable<IFile> files) {
        throw new NotImplementedException();
    }
}