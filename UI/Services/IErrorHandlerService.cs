using System;

namespace UI.Services;

public interface IErrorHandlerService {
    void ShowError(Exception ex);
}