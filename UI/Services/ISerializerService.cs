using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Questions;

namespace UI.Services;

public interface ISerializerService {
    Task UpdateTrackingFile();
    List<AbstractQuestion?>? LoadTrackedQuestions();
    Task Save(AbstractQuestion question);
    Task<AbstractQuestion?> Load(string path);
}