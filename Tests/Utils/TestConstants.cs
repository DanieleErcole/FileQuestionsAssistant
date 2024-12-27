namespace Tests.Utils;

public class TestConstants {
    #if OS_WINDOWS
        internal const string TestFilesDirectory = @"C:\Users\User\Documents\Documenti e lavori\Lavori\C#\FileQuestionsAssistant\Tests\Files\";
    #elif OS_LINUX
        internal const string TestFilesDirectory = @"/home/daniele/RiderProjects/FileQuestionsAssistant/Tests/Files/";
    #endif
}