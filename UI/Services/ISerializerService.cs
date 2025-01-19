using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Questions;

namespace UI.Services;

public interface ISerializerService {
    Task UpdateTrackingFile();
    List<IQuestion?>? LoadTrackedQuestions();
    Task Save(AbstractQuestion question);
    Task<IQuestion?> Load(string path);
}