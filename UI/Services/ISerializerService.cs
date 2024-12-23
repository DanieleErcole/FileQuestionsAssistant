using System.Threading.Tasks;
using Core.Questions;

namespace UI.Services;

public interface ISerializerService {
    Task UpdateTrackingFile();
    AbstractQuestion[]? LoadTrackedQuestions();
    Task Save(AbstractQuestion question);
    Task<AbstractQuestion?> Load(string path);
}