namespace YCAnalyzer.Analizer {
    public interface IAnalizer {
        bool scan(StringReader reader, AnalizerContext context);
        Token create_token(AnalizerContext context);
    }
}
