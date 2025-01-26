using Core.Utils;
using Core.Utils.Errors;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Core.FileHandling; 

public class WordFile : IFile {

    private readonly WordprocessingDocument _doc;
    private readonly MainDocumentPart _mainDoc;
    
    public string Name { get; }
    public string Path { get; }

    private Styles StylesPart {
        get {
            try {
                if (_mainDoc.StylesWithEffectsPart is { } s)
                    return s.Styles ?? throw new InvalidFileFormat(Name);
                return _mainDoc.StyleDefinitionsPart?.Styles ?? throw new InvalidFileFormat(Name);
            } catch (Exception e) when (e is not FileError) {
                throw new FileError(Name, e);
            }
        }
    }

    public IEnumerable<Style> Styles => StylesPart.Elements<Style>().Select(ResolveStyle);

    public IEnumerable<Font> Fonts {
        get {
            try {
                return _mainDoc.FontTablePart?.Fonts.Elements<Font>() ?? throw new InvalidFileFormat(Name);
            } catch (Exception e) when (e is not FileError) {
                throw new FileError(Name, e);
            }
        }
    }

    public IEnumerable<Paragraph> Paragraphs {
        get {
            try {
                return _mainDoc.Document.Body?.Elements<Paragraph>() ?? throw new InvalidFileFormat(Name);
            } catch (Exception e) when (e is not FileError) {
                throw new FileError(Name, e);
            }
        }
    }
    
    public WordFile(string name, Stream file) {
        Name = name;
        Path = file is FileStream fs ? fs.Name : Name;
        try {
            _doc = WordprocessingDocument.Open(file, false);
            _mainDoc = _doc.MainDocumentPart ?? throw new InvalidFileFormat(Name);
        } catch (Exception e) when (e is not FileError) {
            throw new FileError(Name, e);
        }
    }
    
    public static implicit operator WordFile(MemoryFile inMem) {
        return new WordFile(inMem.Name, new MemoryStream(inMem.Data));
    }
    
    /// <summary>
    /// Resolve missing style properties by retrieving them from the style inheritance chain
    /// </summary>
    /// <param name="style">the style with missing properties</param>
    /// <returns>The original style with all the inherited properties</returns>
    private Style ResolveStyle(Style style) {
        var currentStyle = (Style) style.Clone();
        var resolvedStyle = (Style) style.Clone();
        
        while (currentStyle.BasedOn?.Val?.Value is not null) {
            var parentStyleId = currentStyle.BasedOn.Val.Value!;
            var parentStyle = StylesPart.Elements<Style>().FirstOrDefault(s => s.StyleId == parentStyleId);

            if (parentStyle == null)
                break;
            
            Merge(resolvedStyle, parentStyle);
            currentStyle = parentStyle;
        }

        ApplyDocDefaults(resolvedStyle);
        return resolvedStyle;
    }
    
    private void Merge(Style targetStyle, Style sourceStyle) {
        if (sourceStyle.StyleRunProperties is not null) {
            targetStyle.StyleRunProperties ??= new StyleRunProperties();

            foreach (var property in sourceStyle.StyleRunProperties.Elements())
                if (targetStyle.StyleRunProperties.Elements().All(p => p.LocalName != property.LocalName))
                    targetStyle.StyleRunProperties.Append(property.CloneNode(true));
        }
        
        if (sourceStyle.StyleParagraphProperties is not null) {
            targetStyle.StyleParagraphProperties ??= new StyleParagraphProperties();

            foreach (var property in sourceStyle.StyleParagraphProperties.Elements())
                if (targetStyle.StyleParagraphProperties.Elements().All(p => p.LocalName != property.LocalName))
                    targetStyle.StyleParagraphProperties.Append(property.CloneNode(true));
        }
    }

    private void ApplyDocDefaults(Style target) {
        var docDefaults = StylesPart.DocDefaults;
        if (docDefaults == null) 
            return;
        
        var defaultRunProps = docDefaults.RunPropertiesDefault?.RunPropertiesBaseStyle;
        var defaultParProps = docDefaults.ParagraphPropertiesDefault?.ParagraphPropertiesBaseStyle;
        
        if (defaultRunProps is not null) {
            target.StyleRunProperties ??= new StyleRunProperties();

            foreach (var property in defaultRunProps.Elements())
                if (target.StyleRunProperties.Elements().All(p => p.LocalName != property.LocalName))
                    target.StyleRunProperties.Append(property.CloneNode(true));
        }
        
        if (defaultParProps is not null) {
            target.StyleParagraphProperties ??= new StyleParagraphProperties();

            foreach (var property in defaultParProps.Elements())
                if (target.StyleParagraphProperties.Elements().All(p => p.LocalName != property.LocalName))
                    target.StyleParagraphProperties.Append(property.CloneNode(true));
        }
    }

    public void Dispose() {
        if (_doc is not null) _doc.Dispose();
        GC.SuppressFinalize(this);
    }
    
    ~WordFile() => Dispose();
    
}