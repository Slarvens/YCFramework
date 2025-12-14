using YCSyntaxer;

namespace YCTable.Handlers.Header {
    public interface IHeaderBuilderCallback {
        void Report(Node node, global::YCTable.IHeader td);
    }
}
